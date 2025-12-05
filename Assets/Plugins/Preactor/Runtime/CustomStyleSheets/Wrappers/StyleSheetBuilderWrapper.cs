using Preactor.CustomStyleSheets.Structs;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Preactor.CustomStyleSheets.Wrappers {
    public class StyleSheetBuilderWrapper {
        readonly Type type;
        readonly object instance;

        // MOD: Expose currentProperty StyleValueImporter
        public object currentProperty {
            // StyleProperty is internal
            get {
                var propertyInfo = type.GetProperty("currentProperty");
                var original = propertyInfo.GetValue(instance);
                return original;
            }
        }

        public StyleSheetBuilderWrapper() {
            type = typeof(VisualElement).Assembly.GetType("UnityEngine.UIElements.StyleSheets.StyleSheetBuilder");
            instance = Activator.CreateInstance(type);
        }

        public void BuildTo(StyleSheet styleSheet) {
            type.InvokeMember(
                "BuildTo",
                BindingFlags.InvokeMethod,
                null,
                instance,
                new object[] { styleSheet }
            );
        }

        public void AddCommaSeparator() {
            type.InvokeMember(
                "AddCommaSeparator",
                BindingFlags.InvokeMethod,
                null,
                instance,
                new object[] { }
            );
        }

        public IDisposable BeginComplexSelector(int specificity) {
            return (IDisposable)type.InvokeMember(
                "BeginComplexSelector",
                BindingFlags.InvokeMethod,
                null,
                instance,
                new object[] { specificity }
            );
        }

        public void AddSimpleSelector(StyleSelectorPart[] parts, StyleSelectorRelationship previousRelationsip) {
            var t1 = type.Assembly.GetType("UnityEngine.UIElements.StyleSelectorPart");
            var t1Array = t1.MakeArrayType();

            var t2 = type.Assembly.GetType("UnityEngine.UIElements.StyleSelectorRelationship");
            var methodInfo = type.GetMethod("AddSimpleSelector", new[] { t1Array, t2 });

            var newParts = parts.Select(p => p.ToOriginal()).ToArray();
            var length = parts.Length;
            var destinationArray = Array.CreateInstance(t1, length);
            Array.Copy(newParts, destinationArray, length);

            methodInfo.Invoke(instance, new object[] { destinationArray, (int)previousRelationsip });
        }

        public void BeginRule(int line) {
            type.InvokeMember(
                "BeginRule",
                BindingFlags.InvokeMethod,
                null,
                instance,
                new object[] { line }
            );
        }

        public void EndRule() {
            type.InvokeMember(
                "EndRule",
                BindingFlags.InvokeMethod,
                null,
                instance,
                new object[] { }
            );
        }

        public void BeginProperty(string name, int line) {
            type.InvokeMember(
                "BeginProperty",
                BindingFlags.InvokeMethod,
                null,
                instance,
                new object[] { name, line }
            );
        }

        public void EndProperty() {
            type.InvokeMember(
                "EndProperty",
                BindingFlags.InvokeMethod,
                null,
                instance,
                new object[] { }
            );
        }

        public void AddValue(StyleValueFunction func) {
            var methodInfo = type.GetMethod(
                "AddValue",
                new[] { type.Assembly.GetType("UnityEngine.UIElements.StyleValueFunction") }
            );
            methodInfo.Invoke(instance, new object[] { (int)func });
        }

        public void AddValue(float val) {
            var methodInfo = type.GetMethod(
                "AddValue",
                new[] { typeof(float) }
            );
            methodInfo.Invoke(instance, new object[] { val });
        }

        public void AddValue(Color color) {
            var methodInfo = type.GetMethod(
                "AddValue",
                new[] { typeof(Color) }
            );
            methodInfo.Invoke(instance, new object[] { color });
        }

        public void AddValue(Object value) {
            var methodInfo = type.GetMethod(
                "AddValue",
                new[] { typeof(Object) }
            );
            methodInfo.Invoke(instance, new object[] { value });
        }

        public void AddValue(Dimension dimension) {
            var methodInfo = type.GetMethod(
                "AddValue",
                new[] { type.Assembly.GetType("UnityEngine.UIElements.StyleSheets.Dimension") }
            );
            methodInfo.Invoke(instance, new[] { dimension.ToOriginal() });
        }

        public void AddValue(StyleValueKeyword keyword) {
            var methodInfo = type.GetMethod(
                "AddValue",
                new[] { type.Assembly.GetType("UnityEngine.UIElements.StyleValueKeyword") }
            );
            methodInfo.Invoke(instance, new object[] { (int)keyword });
        }

        public void AddValue(string value, StyleValueType type) {
            var methodInfo = this.type.GetMethod(
                "AddValue",
                new[] { typeof(string), this.type.Assembly.GetType("UnityEngine.UIElements.StyleValueType") }
            );
            methodInfo.Invoke(instance, new object[] { value, (int)type });
        }
    }
}
