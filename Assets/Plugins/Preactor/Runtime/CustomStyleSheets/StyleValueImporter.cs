using ExCSS;
using Preactor.CustomStyleSheets.Structs;
using Preactor.CustomStyleSheets.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace Preactor.CustomStyleSheets {
    public enum URIValidationResult {
        OK,
        InvalidURILocation,
        InvalidURIScheme,
        InvalidURIProjectAssetPath
    }

    public abstract class StyleValueImporter {
        protected readonly UnityStylesheetParser m_Parser;
        protected readonly StyleSheetBuilderWrapper m_Builder;
        protected readonly StyleValidatorWrapper m_Validator;
        protected string m_AssetPath;
        protected int m_CurrentLine;
        internal readonly StyleSheetImportErrors m_Errors;
        static StyleSheetImportGlossary s_Glossary;
        static readonly Dictionary<string, Dimension.Unit> s_UnitNameToDimensionUnit;
        static Dictionary<string, StyleValueKeyword> s_NameCache;
        readonly StringBuilder m_StringBuilder = new();
        static readonly string kThemePrefix;

        public bool disableValidation { get; set; }
        public StyleSheetImportErrors importErrors => m_Errors;
        public string assetPath => m_AssetPath;

        internal static StyleSheetImportGlossary glossary => s_Glossary ?? (s_Glossary = new());

        static StyleValueImporter() {
            s_UnitNameToDimensionUnit = new() {
                { UnitNames.Px, Dimension.Unit.Pixel },
                { UnitNames.Percent, Dimension.Unit.Percent },
                { UnitNames.S, Dimension.Unit.Second },
                { UnitNames.Ms, Dimension.Unit.Millisecond },
                { UnitNames.Deg, Dimension.Unit.Degree },
                { UnitNames.Grad, Dimension.Unit.Gradian },
                { UnitNames.Rad, Dimension.Unit.Radian },
                { UnitNames.Turn, Dimension.Unit.Turn }
            };
            kThemePrefix = "unity-theme://";
            PseudoClassSelectorFactory.Selectors["selected"] = PseudoClassSelector.Create("selected");
        }

        internal StyleValueImporter() {
            m_AssetPath = null;
            m_Parser = new();
            m_Builder = new();
            m_Errors = new();
            m_Validator = new();
        }

        void VisitUrlFunction(string path) {
            // TODO: refactor to use addressables
            throw new NotImplementedException();

            // var workingDir = _scriptEngine.WorkingDir;
            // var fullpath = Path.Combine(workingDir, path);
            // if (File.Exists(fullpath)) {
            //     // test path ends in .jpg or .png
            //     if (path.EndsWith(".jpg") || path.EndsWith(".png")) {
            //         Texture2D tex = new Texture2D(2, 2);
            //         tex.LoadImage(File.ReadAllBytes(fullpath));
            //         tex.filterMode = FilterMode.Bilinear;
            //
            //         m_Builder.AddValue(tex);
            //
            //         // TODO ScalableImage with @2x
            //     } else if (path.EndsWith(".ttf")) {
            //         Font font = new Font(fullpath);
            //         m_Builder.AddValue(font);
            //     } else {
            //         m_Errors.AddSemanticError(StyleSheetImportErrorCode.InvalidURILocation,
            //             string.Format(StyleValueImporter.glossary.invalidUriLocation, path), m_CurrentLine);
            //     }
            // }
        }

        bool ValidateFunction(FunctionToken functionToken, out StyleValueFunction func) {
            func = StyleValueFunction.Unknown;
            TextPosition position;
            if (!functionToken.ArgumentTokens.Any()) {
                var errors = m_Errors;
                var message = string.Format(glossary.missingFunctionArgument, functionToken.Data);
                position = functionToken.Position;
                var line = position.Line;
                position = functionToken.Position;
                errors.AddSemanticError(
                    StyleSheetImportErrorCode.MissingFunctionArgument,
                    message,
                    line,
                    position.Column
                );
                return false;
            }

            if (functionToken.Data == "var") {
                func = StyleValueFunction.Var;
                return ValidateVarFunction(functionToken);
            }

            try {
                func = StyleValueFunctionExtension.FromUssString(functionToken.Data);
            } catch (Exception) {
                var currentProperty = m_Builder.currentProperty;
                var errors2 = m_Errors;
                var message2 = string.Format(glossary.unknownFunction, functionToken.Data, currentProperty);
                position = functionToken.Position;
                var line2 = position.Line;
                position = functionToken.Position;
                errors2.AddValidationWarning(message2, line2, position.Column);
                return false;
            }

            return true;
        }

        bool ValidateVarFunction(FunctionToken functionToken) {
            var flag = false;
            var flag2 = false;
            var list = Enumerable.ToList(functionToken.ArgumentTokens);

            list.Trim();
            for (var i = 0; i < list.Count; i++) {
                var val = list[i];
                if ((int)val.Type == 26) {
                    continue;
                }

                TextPosition position;
                if (!flag) {
                    var text = val.ToValue();
                    if (string.IsNullOrEmpty(text)) {
                        var missingVariableName = glossary.missingVariableName;
                        position = val.Position;
                        var line = position.Line;
                        position = val.Position;
                        m_Errors.AddSemanticError(
                            StyleSheetImportErrorCode.InvalidVarFunction,
                            missingVariableName,
                            line,
                            position.Column
                        );
                        return false;
                    }

                    if (!text.StartsWith("--")) {
                        var message = string.Format(glossary.missingVariablePrefix, text);
                        position = val.Position;
                        var line2 = position.Line;
                        position = val.Position;
                        m_Errors.AddSemanticError(
                            StyleSheetImportErrorCode.InvalidVarFunction,
                            message,
                            line2,
                            position.Column
                        );
                        return false;
                    }

                    if (text.Length < 3) {
                        var emptyVariableName = glossary.emptyVariableName;
                        position = val.Position;
                        var line3 = position.Line;
                        position = val.Position;
                        m_Errors.AddSemanticError(
                            StyleSheetImportErrorCode.InvalidVarFunction,
                            emptyVariableName,
                            line3,
                            position.Column
                        );
                        return false;
                    }

                    flag = true;
                } else if ((int)val.Type == 24) {
                    if (flag2) {
                        var tooManyFunctionArguments = glossary.tooManyFunctionArguments;
                        position = val.Position;
                        var line4 = position.Line;
                        position = val.Position;
                        m_Errors.AddSemanticError(
                            StyleSheetImportErrorCode.InvalidVarFunction,
                            tooManyFunctionArguments,
                            line4,
                            position.Column
                        );
                        return false;
                    }

                    flag2 = true;
                    i++;
                    if (i >= list.Count) {
                        var emptyFunctionArgument = glossary.emptyFunctionArgument;
                        position = val.Position;
                        var line5 = position.Line;
                        position = val.Position;
                        m_Errors.AddSemanticError(
                            StyleSheetImportErrorCode.InvalidVarFunction,
                            emptyFunctionArgument,
                            line5,
                            position.Column
                        );
                        return false;
                    }
                } else if (!flag2) {
                    var arg = "";
                    while ((int)val.Type == 26 && i + 1 < list.Count) {
                        val = list[++i];
                    }

                    if ((int)val.Type != 26) {
                        arg = val.Data;
                    }

                    var message2 = string.Format(glossary.unexpectedTokenInFunction, arg);
                    position = val.Position;
                    var line6 = position.Line;
                    position = val.Position;
                    m_Errors.AddSemanticError(
                        StyleSheetImportErrorCode.InvalidVarFunction,
                        message2,
                        line6,
                        position.Column
                    );
                    return false;
                }
            }

            return true;
        }

        void VisitToken(Token token) {
            var val = (ColorToken)(token is ColorToken ? token : null);
            TextPosition position;
            
            if (val == null) {
                var val2 = (FunctionToken)(token is FunctionToken ? token : null);
                if (val2 == null) {
                    var val3 = (KeywordToken)(token is KeywordToken ? token : null);
                    if (val3 == null) {
                        var val4 = (NumberToken)(token is NumberToken ? token : null);
                        if (val4 == null) {
                            var val5 = (StringToken)(token is StringToken ? token : null);
                            if (val5 == null) {
                                var val6 = (UnitToken)(token is UnitToken ? token : null);
                                Dimension.Unit value;
                                if (val6 == null) {
                                    var val7 = (UrlToken)(token is UrlToken ? token : null);
                                    if (val7 != null) {
                                        VisitUrlFunction(val7.Data);
                                        return;
                                    }

                                    var type = token.Type;
                                    switch ((int)type - 23) {
                                        case 0:
                                        case 3:
                                            return;
                                        case 1:
                                            m_Builder.AddCommaSeparator();
                                            return;
                                    }

                                    var errors = m_Errors;
                                    var message = string.Format(glossary.unsupportedTerm, token.Data, token.Type);
                                    position = token.Position;
                                    var line = position.Line;
                                    position = token.Position;
                                    errors.AddSemanticError(
                                        StyleSheetImportErrorCode.UnsupportedTerm,
                                        message,
                                        line,
                                        position.Column
                                    );
                                } else if (s_UnitNameToDimensionUnit.TryGetValue(val6.Unit, out value)) {
                                    m_Builder.AddValue(new Dimension(val6.Value, value));
                                } else {
                                    var errors2 = m_Errors;
                                    var message2 = string.Format(glossary.unsupportedUnit, val6.ToValue());
                                    position = val6.Position;
                                    var line2 = position.Line;
                                    position = val6.Position;
                                    errors2.AddSemanticError(
                                        StyleSheetImportErrorCode.UnsupportedUnit,
                                        message2,
                                        line2,
                                        position.Column
                                    );
                                }
                            } else {
                                m_Builder.AddValue(val5.Data, StyleValueType.String);
                            }
                        } else {
                            m_Builder.AddValue(val4.Value);
                        }
                    } else if ((int)val3.Type == 6) {
                        if (TryParseKeyword(val3.Data, out var value2)) {
                            m_Builder.AddValue(value2);
                        } else if (val3.Data.StartsWith("--")) {
                            m_Builder.AddValue(val3.Data, StyleValueType.Variable);
                        } else {
                            m_Builder.AddValue(val3.Data, StyleValueType.Enum);
                        }
                    } else {
                        var errors3 = m_Errors;
                        var message3 = string.Format(glossary.unsupportedTerm, val3.Data, val3.Type);
                        position = val3.Position;
                        var line3 = position.Line;
                        position = val3.Position;
                        errors3.AddSemanticError(
                            StyleSheetImportErrorCode.UnsupportedTerm,
                            message3,
                            line3,
                            position.Column
                        );
                    }
                } else {
                    VisitFunctionToken(val2);
                }
            } else if (ColorUtility.TryParseHtmlString("#" + val.Data, out var color)) {
                m_Builder.AddValue(color);
            } else {
                var message4 = "Could not parse color token: " + val.Data;
                position = val.Position;
                var line4 = position.Line;
                position = val.Position;
                m_Errors.AddSyntaxError(message4, line4, position.Column);
            }
        }

        void VisitFunctionToken(FunctionToken functionToken) {
            //IL_0128: Unknown result type (might be due to invalid IL or missing references)
            //IL_012d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0137: Unknown result type (might be due to invalid IL or missing references)
            //IL_013c: Unknown result type (might be due to invalid IL or missing references)
            switch (functionToken.Data) {
                case "rgb": {
                    if (TryCreateColorFromFunctionToken(functionToken, 3, out var color)) {
                        m_Builder.AddValue(color);
                    }

                    return;
                }
                case "rgba": {
                    if (TryCreateColorFromFunctionToken(functionToken, 4, out var color2)) {
                        m_Builder.AddValue(color2);
                    }

                    return;
                }
                case "resource": {
                    var obj = functionToken.ArgumentTokens.FirstOrDefault();
                    var val = (StringToken)(obj is StringToken ? obj : null);
                    if (val != null) {
                        m_Builder.AddValue(val.Data, StyleValueType.ResourcePath);
                        return;
                    }

                    var value = BuildStringFromTokens(functionToken.ArgumentTokens);
                    if (!string.IsNullOrEmpty(value)) {
                        m_Builder.AddValue(m_StringBuilder.ToString(), StyleValueType.ResourcePath);
                        m_StringBuilder.Clear();
                        return;
                    }

                    var data = functionToken.Data;
                    var position = functionToken.Position;
                    var line = position.Line;
                    position = functionToken.Position;
                    m_Errors.AddSemanticError(
                        StyleSheetImportErrorCode.MissingFunctionArgument,
                        data,
                        line,
                        position.Column
                    );
                    return;
                }
                case "none":
                    m_Builder.AddValue((StyleValueFunction)4);
                    VisitCustomFilter(functionToken);
                    return;
                case "filter":
                    m_Builder.AddValue((StyleValueFunction)5);
                    VisitCustomFilter(functionToken);
                    return;
            }

            if (!ValidateFunction(functionToken, out var func)) {
                return;
            }

            m_Builder.AddValue(func);
            m_Builder.AddValue(functionToken.ArgumentTokens.Count(t => (int)t.Type != 26));
            foreach (var argumentToken in functionToken.ArgumentTokens) {
                VisitToken(argumentToken);
            }
        }

        bool IsTokenString(IEnumerable<Token> tokens) {
            //IL_0022: Unknown result type (might be due to invalid IL or missing references)
            //IL_002a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0031: Invalid comparison between Unknown and I4
            //IL_0034: Unknown result type (might be due to invalid IL or missing references)
            //IL_003a: Invalid comparison between Unknown and I4
            if (tokens.Count() > 1) {
                return tokens.All(token => (int)token.Type == 0 || (int)token.Type == 15 || (int)token.Type == 6);
            }

            return false;
        }

        string BuildStringFromTokens(IEnumerable<Token> tokens) {
            //IL_0020: Unknown result type (might be due to invalid IL or missing references)
            //IL_0027: Invalid comparison between Unknown and I4
            m_StringBuilder.Clear();
            foreach (var token in tokens) {
                if ((int)token.Type != 26) {
                    m_StringBuilder.Append(token.Data);
                }
            }

            return m_StringBuilder.ToString();
        }

        void VisitCustomFilter(FunctionToken functionToken) {
            var list = Enumerable.ToList(functionToken.ArgumentTokens);

            m_Builder.AddValue(list.Count(a => (int)a.Type != 26));
            if (list.Count > 0) {
                VisitUrlFunction(list[0].Data);
            }

            for (var i = 1; i < list.Count; i++) {
                VisitToken(list[i]);
            }
        }

        bool TryCreateColorFromFunctionToken(FunctionToken functionToken, int expectedChannels, out Color color) {
            //IL_00cd: Unknown result type (might be due to invalid IL or missing references)
            //IL_00d2: Unknown result type (might be due to invalid IL or missing references)
            //IL_00dc: Unknown result type (might be due to invalid IL or missing references)
            //IL_00e1: Unknown result type (might be due to invalid IL or missing references)
            var flag = true;
            color = new(0f, 0f, 0f, 1f);
            var num = 0;
            foreach (var argumentToken in functionToken.ArgumentTokens) {
                var val = (NumberToken)(argumentToken is NumberToken ? argumentToken : null);
                if (val != null) {
                    color[num++] = val.Value;
                    if (!val.IsInteger && num != 4) {
                        flag = false;
                    }

                    if (num == 4) {
                        break;
                    }
                }
            }

            if (num != expectedChannels) {
                var message = string.Format(glossary.missingFunctionArgument, functionToken.Data);
                var position = functionToken.Position;
                var line = position.Line;
                position = functionToken.Position;
                m_Errors.AddSemanticError(
                    StyleSheetImportErrorCode.MissingFunctionArgument,
                    message,
                    line,
                    position.Column
                );
                return false;
            }

            if (flag) {
                for (var i = 0; i < Mathf.Min(3, expectedChannels); i++) {
                    color[i] /= 255f;
                }
            }

            return true;
        }

        static bool TryParseKeyword(string rawStr, out StyleValueKeyword value) {
            if (s_NameCache == null) {
                s_NameCache = new();
                foreach (StyleValueKeyword value2 in Enum.GetValues(typeof(StyleValueKeyword))) {
                    s_NameCache[value2.ToString().ToLowerInvariant()] = value2;
                }
            }

            return s_NameCache.TryGetValue(rawStr.ToLowerInvariant(), out value);
        }

        internal static (StyleSheetImportErrorCode, string) ConvertErrorCode(URIValidationResult result) {
            return result switch {
                URIValidationResult.InvalidURILocation => (StyleSheetImportErrorCode.InvalidURILocation,
                    glossary.invalidUriLocation),
                URIValidationResult.InvalidURIScheme => (StyleSheetImportErrorCode.InvalidURIScheme,
                    glossary.invalidUriScheme),
                URIValidationResult.InvalidURIProjectAssetPath => (StyleSheetImportErrorCode.InvalidURIProjectAssetPath,
                    glossary.invalidAssetPath),
                _ => (StyleSheetImportErrorCode.Internal, glossary.internalErrorWithStackTrace)
            };
        }

        protected class UnityStylesheetParser : StylesheetParser {
            public readonly List<TokenizerError> errors = new();

            public UnityStylesheetParser() : base(true, true, true, true, false, false, true, false) {
                ErrorHandler = HandleError;
            }

            public override Stylesheet Parse(string content) {
                errors.Clear();
                return base.Parse(content);
            }

            void HandleError(object sender, TokenizerError tokenizerError) {
                errors.Add(tokenizerError);
            }
        }

        struct StoredAsset {
            public Object resource;
            public ScalableImage si;
            public int index;
        }

        protected void VisitValue(Property property) {
            if (IsTokenString(property.DeclaredValue.Original)) {
                var value = BuildStringFromTokens(property.DeclaredValue.Original);
                if (!string.IsNullOrEmpty(value)) {
                    m_Builder.AddValue(value, StyleValueType.String);
                    return;
                }
            }

            foreach (var item in property.DeclaredValue.Original) {
                VisitToken(item);
            }
        }
    }
}
