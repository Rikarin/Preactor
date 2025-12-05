using ExCSS;
using Preactor.CustomStyleSheets.Structs;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using StyleSheet = UnityEngine.UIElements.StyleSheet;

namespace Preactor.CustomStyleSheets {
    public class CustomStyleSheetImporterImpl : StyleValueImporter {
        public void BuildStyleSheet(StyleSheet asset, string contents) {
            var styleSheet = m_Parser.Parse(contents);
            ImportParserStyleSheet(asset, styleSheet, m_Parser.errors);
            var hash = new Hash128();
            var bytes = Encoding.UTF8.GetBytes(contents);
            if (bytes.Length != 0) {
                HashUtilities.ComputeHash128(bytes, ref hash);
            }

            asset.contentHash = hash.GetHashCode();
        }

        void AddUssParserError(TokenizerError error) {
            var parseError = (ParseError)error.Code;
            var arg = error.Message;
            var flag = parseError == ParseError.InvalidBlockStart;
            if (flag) {
                arg = "Invalid block start, no selector found before the opening curly bracket.";
            }

            var arg2 = $"{(ParseError)error.Code} : {arg}";
            m_Errors.AddSyntaxError(
                string.Format(glossary.ussParsingError, arg2),
                error.Position.Line,
                error.Position.Column
            );
        }

        int GetPropertyLine(Property property) => property.DeclaredValue.Original[0].Position.Line;

        void VisitSheet(Stylesheet styleSheet) {
            foreach (var styleRule in styleSheet.StyleRules) {
                m_Builder.BeginRule(styleRule.StylesheetText.Range.Start.Line);
                m_CurrentLine = styleRule.StylesheetText.Range.Start.Line;
                VisitBaseSelector(styleRule.Selector);
                foreach (var property in styleRule.Style.Declarations) {
                    var propertyLine = GetPropertyLine(property);
                    m_CurrentLine = propertyLine;
                    ValidateProperty(property);
                    m_Builder.BeginProperty(property.Name, propertyLine);
                    VisitValue(property);
                    m_Builder.EndProperty();
                }

                m_Builder.EndRule();
            }
        }

        void VisitBaseSelector(ISelector selector) {
            if (selector is not AllSelector allSelector) {
                if (selector is not ClassSelector classSelector) {
                    if (selector is not ComplexSelector complexSelector) {
                        if (selector is not CompoundSelector compoundSelector) {
                            if (selector is not IdSelector idSelector) {
                                if (selector is not ListSelector listSelector) {
                                    if (selector is not PseudoClassSelector pseudoClassSelector) {
                                        if (selector is not TypeSelector typeSelector) {
                                            if (selector is not UnknownSelector unknownSelector) {
                                                m_Errors.AddSemanticError(
                                                    StyleSheetImportErrorCode.UnsupportedSelectorFormat,
                                                    string.Format(
                                                        glossary.unsupportedSelectorFormat,
                                                        selector.GetType().Name + ": `" + selector.Text + "`"
                                                    ),
                                                    m_CurrentLine
                                                );
                                            } else {
                                                VisitUnknownSelector(unknownSelector);
                                            }
                                        } else {
                                            VisitSelectorParts(
                                                new[] { StyleSelectorPart.CreateType(typeSelector.Name) },
                                                typeSelector
                                            );
                                        }
                                    } else {
                                        ValidatePsuedoClassName(pseudoClassSelector.Class, pseudoClassSelector.Text);
                                        VisitSelectorParts(
                                            new[] { StyleSelectorPart.CreatePseudoClass(pseudoClassSelector.Class) },
                                            pseudoClassSelector
                                        );
                                    }
                                } else {
                                    foreach (var selector2 in listSelector) {
                                        VisitBaseSelector(selector2);
                                    }
                                }
                            } else {
                                VisitSelectorParts(new[] { StyleSelectorPart.CreateId(idSelector.Id) }, idSelector);
                            }
                        } else {
                            StyleSelectorPart[] parts;
                            var flag = TryExtractSelectorsParts(compoundSelector, out parts);
                            if (flag) {
                                VisitSelectorParts(parts, compoundSelector);
                            }
                        }
                    } else {
                        VisitComplexSelector(complexSelector);
                    }
                } else {
                    VisitSelectorParts(new[] { StyleSelectorPart.CreateClass(classSelector.Class) }, classSelector);
                }
            } else {
                VisitSelectorParts(new[] { StyleSelectorPart.CreateWildCard() }, allSelector);
            }
        }

        void ValidatePsuedoClassName(string name, string selector) {
            var flag = !disableValidation && !PseudoClassSelectorFactory.Selectors.ContainsKey(name);
            if (flag) {
                m_Errors.AddValidationWarning(
                    string.Format(glossary.unknownPseudoClass, name, selector),
                    m_CurrentLine
                );
            }
        }

        // Token: 0x0600CC20 RID: 52256 RVA: 0x003BC304 File Offset: 0x003BA504
        void VisitUnknownSelector(UnknownSelector unknownSelector) {
            var text = unknownSelector.Text;
            var flag = text.StartsWith(".") && text.Length > 1;
            if (flag) {
                var flag2 = char.IsDigit(text[1]) || (text.Length >= 2 && text[1] == '-' && char.IsDigit(text[2]));
                if (flag2) {
                    m_Errors.AddSemanticError(
                        StyleSheetImportErrorCode.UnsupportedSelectorFormat,
                        string.Format(glossary.selectorStartsWithDigitFormat, unknownSelector.Text),
                        m_CurrentLine
                    );
                    return;
                }
            }

            m_Errors.AddSemanticError(
                StyleSheetImportErrorCode.UnsupportedSelectorFormat,
                string.Format(glossary.unsupportedSelectorFormat, unknownSelector.Text),
                m_CurrentLine
            );
        }

        void VisitSelectorParts(StyleSelectorPart[] parts, ISelector selector) {
            var selectorSpecificity = CSSSpec.GetSelectorSpecificity(parts);
            var flag = selectorSpecificity == 0;
            if (flag) {
                m_Errors.AddInternalError(
                    string.Format(glossary.internalError, "Failed to calculate selector specificity " + selector.Text),
                    m_CurrentLine
                );
            } else {
                using (m_Builder.BeginComplexSelector(selectorSpecificity)) {
                    m_Builder.AddSimpleSelector(parts, 0);
                }
            }
        }

        bool TryExtractSelectorsParts(Selectors selectors, out StyleSelectorPart[] parts) {
            parts = new StyleSelectorPart[selectors.Length];
            for (var i = 0; i < selectors.Length; i++) {
                var selector = selectors[i];
                var selector2 = selector;
                if (!(selector2 is AllSelector)) {
                    var idSelector = selector2 as IdSelector;
                    if (idSelector == null) {
                        var classSelector = selector2 as ClassSelector;
                        if (classSelector == null) {
                            var pseudoClassSelector = selector2 as PseudoClassSelector;
                            if (pseudoClassSelector == null) {
                                var typeSelector = selector2 as TypeSelector;
                                if (typeSelector == null) {
                                    if (!(selector2 is FirstChildSelector)) {
                                        var array = parts;
                                        var num = i;
                                        var styleSelectorPart = default(StyleSelectorPart);
                                        styleSelectorPart.type = 0;
                                        array[num] = styleSelectorPart;
                                    } else {
                                        var array2 = parts;
                                        var num2 = i;
                                        var styleSelectorPart = default(StyleSelectorPart);
                                        styleSelectorPart.type = (StyleSelectorType)5;
                                        array2[num2] = styleSelectorPart;
                                    }
                                } else {
                                    parts[i] = StyleSelectorPart.CreateType(typeSelector.Name);
                                }
                            } else {
                                var flag = pseudoClassSelector.Class.Contains("(");
                                if (flag) {
                                    m_Errors.AddSemanticError(
                                        StyleSheetImportErrorCode.RecursiveSelectorDetected,
                                        string.Format(glossary.unsupportedSelectorFormat, selectors.Text),
                                        m_CurrentLine
                                    );
                                    return false;
                                }

                                parts[i] = StyleSelectorPart.CreatePseudoClass(pseudoClassSelector.Class);
                            }
                        } else {
                            parts[i] = StyleSelectorPart.CreateClass(classSelector.Class);
                        }
                    } else {
                        parts[i] = StyleSelectorPart.CreateId(idSelector.Id);
                    }
                } else {
                    parts[i] = StyleSelectorPart.CreateWildCard();
                }
            }

            return true;
        }

        void VisitComplexSelector(ComplexSelector complexSelector) {
            var selectorSpecificity = CSSSpec.GetSelectorSpecificity(complexSelector.Text);
            var flag = selectorSpecificity == 0;
            if (flag) {
                m_Errors.AddInternalError(
                    string.Format(
                        glossary.internalError,
                        "Failed to calculate selector specificity "
                        + (complexSelector != null ? complexSelector.ToString() : null)
                    ),
                    m_CurrentLine
                );
            } else {
                using (m_Builder.BeginComplexSelector(selectorSpecificity)) {
                    StyleSelectorRelationship styleSelectorRelationship = 0;
                    var num = complexSelector.Length - 1;
                    var num2 = -1;
                    foreach (var combinatorSelector in complexSelector) {
                        num2++;
                        var text = combinatorSelector.Selector.Text;
                        var flag2 = string.IsNullOrEmpty(text);
                        if (flag2) {
                            m_Errors.AddInternalError(
                                string.Format(
                                    glossary.internalError,
                                    "Expected simple selector inside complex selector " + text
                                ),
                                m_CurrentLine
                            );
                            break;
                        }

                        StyleSelectorPart[] array;
                        var flag3 = CheckSimpleSelector(text, out array);
                        if (!flag3) {
                            break;
                        }

                        m_Builder.AddSimpleSelector(array, styleSelectorRelationship);
                        var flag4 = num2 != num;
                        if (flag4) {
                            var flag5 = combinatorSelector.Delimiter == Combinators.Child;
                            if (flag5) {
                                styleSelectorRelationship = (StyleSelectorRelationship)1;
                            } else {
                                var flag6 = combinatorSelector.Delimiter == Combinators.Descendent;
                                if (!flag6) {
                                    m_Errors.AddSemanticError(
                                        StyleSheetImportErrorCode.InvalidComplexSelectorDelimiter,
                                        string.Format(glossary.invalidComplexSelectorDelimiter, complexSelector.Text),
                                        m_CurrentLine
                                    );
                                    break;
                                }

                                styleSelectorRelationship = (StyleSelectorRelationship)2;
                            }
                        }
                    }
                }
            }
        }

        bool CheckSimpleSelector(string selector, out StyleSelectorPart[] parts) {
            var flag = !CSSSpec.ParseSelector(selector, out parts);
            bool result;
            if (flag) {
                m_Errors.AddSemanticError(
                    StyleSheetImportErrorCode.UnsupportedSelectorFormat,
                    string.Format(glossary.unsupportedSelectorFormat, selector),
                    m_CurrentLine
                );
                result = false;
            } else {
                var flag2 = parts.Any(p => p.type == 0);
                if (flag2) {
                    m_Errors.AddSemanticError(
                        StyleSheetImportErrorCode.UnsupportedSelectorFormat,
                        string.Format(glossary.unsupportedSelectorFormat, selector),
                        m_CurrentLine
                    );
                    result = false;
                } else {
                    var flag3 = parts.Any(p => p.type == (StyleSelectorType)5);
                    if (flag3) {
                        m_Errors.AddSemanticError(
                            StyleSheetImportErrorCode.RecursiveSelectorDetected,
                            string.Format(glossary.unsupportedSelectorFormat, selector),
                            m_CurrentLine
                        );
                        result = false;
                    } else {
                        var flag4 = !disableValidation;
                        if (flag4) {
                            foreach (var styleSelectorPart in parts) {
                                var flag5 = styleSelectorPart.type == (StyleSelectorType)4;
                                if (flag5) {
                                    ValidatePsuedoClassName(styleSelectorPart.value, selector);
                                }
                            }
                        }

                        result = true;
                    }
                }
            }

            return result;
        }

        void ValidateProperty(Property property) {
            var flag = !disableValidation;
            if (flag) {
                var name = property.Name;
                var value = property.Value;
                var styleValidationResult = m_Validator.ValidateProperty(name, value);
                var flag2 = !styleValidationResult.success;
                if (flag2) {
                    var text = string.Concat(
                        styleValidationResult.message,
                        "\n    ",
                        name,
                        ": ",
                        value
                    );
                    var flag3 = !string.IsNullOrEmpty(styleValidationResult.hint);
                    if (flag3) {
                        text = text + " -> " + styleValidationResult.hint;
                    }

                    m_Errors.AddValidationWarning(text, GetPropertyLine(property));
                }
            }
        }

        protected void ImportParserStyleSheet(StyleSheet asset, Stylesheet styleSheet, List<TokenizerError> errors) {
            m_Errors.assetPath = assetPath;
            if (errors.Count > 0) {
                foreach (var error in errors) {
                    AddUssParserError(error);
                }
            } else {
                VisitSheet(styleSheet);
            }

            var hasErrors = m_Errors.hasErrors;
            if (!hasErrors) {
                m_Builder.BuildTo(asset);
            }
        }
    }
}
