using Preactor.ContentProviders;
using Preactor.Utils;
using Preactor.Utils.Attributes;
using Puerts;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Preactor {
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(UIDocument))]
    [AddComponentMenu("Preactor/Preact Renderer")]
    public class PreactRenderer : MonoBehaviour, IScriptEngine {
        [field: SerializeField] public TextAsset EntryFile { get; private set; }

        [field: PairMapping("obj", "name")]
        [field: SerializeField]
        public ObjectMappingPair[] GlobalObjects { get; private set; }

        [field: SerializeField] public TextAsset[] Preloads { get; private set; }
        [field: SerializeField] public StyleSheet[] StyleSheets { get; private set; }

        [Tooltip("Watch entry file for changes and reload. Ignored outside of editor.")]
        [field: SerializeField]
        public bool LiveReload { get; private set; }

        [Tooltip("How often to check for changes in the entry file in milliseconds.")]
        [field: SerializeField]
        public int PollingInterval { get; private set; } = 300;

        [field: SerializeField] public bool ClearLogs { get; private set; } = true;
        [field: SerializeField] public bool EnableRemoteDebugging { get; private set; }
        [field: SerializeField] public int DebuggerPort { get; private set; } = 9222;

        int fileHash;
        float lastCheckTime;
        UIDocument uiDocument;
        static IContentProvider contentProviderCache;

        public JsEnv JsEnv { get; private set; }

        void Awake() {
            contentProviderCache ??= new AddressablesContentProvider();
            ContentProvider = contentProviderCache;
            uiDocument = GetComponent<UIDocument>();
        }

        void Start() => Initialize();

        void Update() {
            JsEnv.Tick();

#if UNITY_EDITOR
            if (!LiveReload || Time.time - lastCheckTime < PollingInterval / 1000f) {
                return;
            }

            lastCheckTime = Time.time;
            var newHash = EntryFile.text.GetHashCode();
            if (newHash != fileHash) {
                fileHash = newHash;

                if (ClearLogs) {
                    ClearLog();
                }

                Debug.Log("Reloading...");
                Reload();
            }
#endif
        }

        void OnDestroy() {
            Shutdown();
            // ContentProvider.Dispose();
        }

        public IContentProvider ContentProvider { get; private set; }

        public event Action OnDestroyed;

        public void Shutdown() {
            if (JsEnv != null) {
                OnDestroyed?.Invoke();
                OnDestroyed = null;
                JsEnv.Dispose();
            }

            ClearVisualElement();
        }

        public void Reload() {
            OnDestroyed?.Invoke();
            OnDestroyed = null;
            Initialize();
        }

        void Initialize() {
            JsEnv?.Dispose();

            if (EnableRemoteDebugging) {
                JsEnv = new(new DefaultLoader(), DebuggerPort);
            } else {
                JsEnv = new();
            }

            JsEnv.UsingAction<Action>();
            JsEnv.UsingAction<bool>();
            JsEnv.UsingAction<Vector2>();
            JsEnv.UsingAction<Vector3>();
            ClearVisualElement();

            var global = FindFirstObjectByType<PreactGlobal>();
            if (global) {
                foreach (var preload in global.Preloads) {
                    JsEnv.Eval(preload.text);
                }

                foreach (var styleSheet in global.StyleSheets) {
                    uiDocument.rootVisualElement.styleSheets.Add(styleSheet);
                }
            }

            foreach (var preload in Preloads) {
                JsEnv.Eval(preload.text);
            }

            foreach (var styleSheet in StyleSheets) {
                uiDocument.rootVisualElement.styleSheets.Add(styleSheet);
            }

            var addToGlobal = JsEnv.Eval<Action<string, object>>("__addToGlobal");
            addToGlobal("__document", new Document(uiDocument.rootVisualElement, this));
            addToGlobal("interop", new InteropHost(this));
            addToGlobal("gameObject", gameObject);

            foreach (var obj in GlobalObjects) {
                addToGlobal(obj.name, obj.obj);
            }

            if (global != null) {
                foreach (var obj in global.GlobalObjects) {
                    addToGlobal(obj.name, obj.obj);
                }
            }

            fileHash = EntryFile.text.GetHashCode();
            Eval(EntryFile.text);
        }

        public void Eval(string code) {
            try {
                JsEnv.Eval(code);
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        void ClearVisualElement() {
            if (uiDocument.rootVisualElement != null) {
                uiDocument.rootVisualElement.Clear();
                uiDocument.rootVisualElement.styleSheets.Clear();
            }
        }

        void ClearLog() {
#if UNITY_EDITOR
            var assembly = Assembly.GetAssembly(typeof(Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method?.Invoke(new(), null);
#endif
        }
    }
}
