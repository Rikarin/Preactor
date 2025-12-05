using Preactor.CustomStyleSheets.Structs;
using System;
using System.Text.RegularExpressions;

namespace Preactor.CustomStyleSheets {
    public static class CSSSpec {
        // Token: 0x04001D3F RID: 7487
        const int typeSelectorWeight = 1;

        // Token: 0x04001D40 RID: 7488
        const int classSelectorWeight = 10;

        // Token: 0x04001D41 RID: 7489
        const int idSelectorWeight = 100;

        // Token: 0x04001D3E RID: 7486
        static readonly Regex rgx = new(
            "(?<id>#[-]?\\w[\\w-]*)|(?<class>\\.[\\w-]+)|(?<pseudoclass>:[\\w-]+(\\((?<param>.+)\\))?)|(?<type>([^\\-]\\w+|\\w+))|(?<wildcard>\\*)|\\s+",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public static int GetSelectorSpecificity(string selector) {
            var result = 0;
            StyleSelectorPart[] parts;
            var flag = ParseSelector(selector, out parts);
            if (flag) {
                result = GetSelectorSpecificity(parts);
            }

            return result;
        }

        // Token: 0x06003A6B RID: 14955 RVA: 0x000E4930 File Offset: 0x000E2B30
        public static int GetSelectorSpecificity(StyleSelectorPart[] parts) {
            var num = 1;
            for (var i = 0; i < parts.Length; i++) {
                switch (parts[i].type) {
                    case StyleSelectorType.Type:
                        num++;
                        break;
                    case StyleSelectorType.Class:
                    case StyleSelectorType.PseudoClass:
                        num += 10;
                        break;
                    case StyleSelectorType.RecursivePseudoClass:
                        throw new ArgumentException("Recursive pseudo classes are not supported");
                    case StyleSelectorType.ID:
                        num += 100;
                        break;
                }
            }

            return num;
        }

        // Token: 0x06003A6C RID: 14956 RVA: 0x000E49AC File Offset: 0x000E2BAC
        public static bool ValidateSelector(string selector) => rgx.Matches(selector).Count > 0;

        // Token: 0x06003A6D RID: 14957 RVA: 0x000E49D4 File Offset: 0x000E2BD4
        public static bool ParseSelector(string selector, out StyleSelectorPart[] parts) {
            var matchCollection = rgx.Matches(selector);
            var count = matchCollection.Count;
            var flag = count < 1;
            bool result;
            if (flag) {
                parts = null;
                result = false;
            } else {
                parts = new StyleSelectorPart[count];
                for (var i = 0; i < count; i++) {
                    var match = matchCollection[i];
                    var type = StyleSelectorType.Unknown;
                    var value = string.Empty;
                    var flag2 = !string.IsNullOrEmpty(match.Groups["wildcard"].Value);
                    if (flag2) {
                        value = "*";
                        type = StyleSelectorType.Wildcard;
                    } else {
                        var flag3 = !string.IsNullOrEmpty(match.Groups["id"].Value);
                        if (flag3) {
                            value = match.Groups["id"].Value.Substring(1);
                            type = StyleSelectorType.ID;
                        } else {
                            var flag4 = !string.IsNullOrEmpty(match.Groups["class"].Value);
                            if (flag4) {
                                value = match.Groups["class"].Value.Substring(1);
                                type = StyleSelectorType.Class;
                            } else {
                                var flag5 = !string.IsNullOrEmpty(match.Groups["pseudoclass"].Value);
                                if (flag5) {
                                    var value2 = match.Groups["param"].Value;
                                    var flag6 = !string.IsNullOrEmpty(value2);
                                    if (flag6) {
                                        value = value2;
                                        type = StyleSelectorType.RecursivePseudoClass;
                                    } else {
                                        value = match.Groups["pseudoclass"].Value.Substring(1);
                                        type = StyleSelectorType.PseudoClass;
                                    }
                                } else {
                                    var flag7 = !string.IsNullOrEmpty(match.Groups["type"].Value);
                                    if (flag7) {
                                        value = match.Groups["type"].Value;
                                        type = StyleSelectorType.Type;
                                    }
                                }
                            }
                        }
                    }

                    parts[i] = new() { type = type, value = value };
                }

                result = true;
            }

            return result;
        }
    }
}
