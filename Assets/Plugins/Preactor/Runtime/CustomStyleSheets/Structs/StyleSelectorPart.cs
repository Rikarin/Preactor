using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Preactor.CustomStyleSheets.Structs {
    public enum StyleSelectorType {
        Unknown,
        Wildcard,
        Type,
        Class,
        PseudoClass,
        RecursivePseudoClass,
        ID,
        Predicate
    }

    public struct StyleSelectorPart {
        [SerializeField] string m_Value;
        [SerializeField] StyleSelectorType m_Type;
        
        internal object tempData;

        public string value {
            get => m_Value;
            internal set => m_Value = value;
        }

        public StyleSelectorType type {
            get => m_Type;
            internal set => m_Type = value;
        }

        public override string ToString() => $"[StyleSelectorPart: value={value}, type={type}]";

        public static StyleSelectorPart CreateClass(string className) =>
            new() { m_Type = StyleSelectorType.Class, m_Value = className };

        public static StyleSelectorPart CreatePseudoClass(string className) =>
            new() { m_Type = StyleSelectorType.PseudoClass, m_Value = className };

        public static StyleSelectorPart CreateId(string Id) => new() { m_Type = StyleSelectorType.ID, m_Value = Id };

        public static StyleSelectorPart CreateType(Type t) =>
            new() { m_Type = StyleSelectorType.Type, m_Value = t.Name };

        public static StyleSelectorPart CreateType(string typeName) =>
            new() { m_Type = StyleSelectorType.Type, m_Value = typeName };

        public static StyleSelectorPart CreatePredicate(object predicate) =>
            new() { m_Type = StyleSelectorType.Predicate, tempData = predicate };

        public static StyleSelectorPart CreateWildCard() => new() { m_Type = StyleSelectorType.Wildcard };

        public object ToOriginal() {
            var type = typeof(VisualElement).Assembly.GetType("UnityEngine.UIElements.StyleSelectorPart");
            var obj = Activator.CreateInstance(type);

            type.GetField("m_Value", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, m_Value);
            type.GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, (int)m_Type);
            type.GetField("tempData", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, tempData);

            return obj;
        }
    }
}
