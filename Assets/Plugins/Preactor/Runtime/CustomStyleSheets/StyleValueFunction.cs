using System;

namespace Preactor.CustomStyleSheets {
    public enum StyleValueFunction {
        Unknown,
        Var,
        Env,
        LinearGradient,
        NoneFilter,
        CustomFilter,
        FilterTint,
        FilterOpacity,
        FilterInvert,
        FilterGrayscale,
        FilterSepia,
        FilterBlur
    }

    public static class StyleValueFunctionExtension {
        public const string k_Var = "var";
        public const string k_Env = "env";
        public const string k_LinearGradient = "linear-gradient";
        public const string k_NoneFilter = "none";
        public const string k_CustomFilter = "filter";
        public const string k_FilterTint = "tint";
        public const string k_FilterOpacity = "opacity";
        public const string k_FilterInvert = "invert";
        public const string k_FilterGrayscale = "grayscale";
        public const string k_FilterSepia = "sepia";
        public const string k_FilterBlur = "blur";

        public static StyleValueFunction FromUssString(string ussValue) {
#pragma warning disable CA1308
            ussValue = ussValue.ToLowerInvariant();
#pragma warning restore CA1308
            return ussValue switch {
                k_Var => StyleValueFunction.Var,
                k_Env => StyleValueFunction.Env,
                k_LinearGradient => StyleValueFunction.LinearGradient,
                k_NoneFilter => StyleValueFunction.NoneFilter,
                k_FilterTint => StyleValueFunction.FilterTint,
                k_FilterOpacity => StyleValueFunction.FilterOpacity,
                k_FilterInvert => StyleValueFunction.FilterInvert,
                k_FilterGrayscale => StyleValueFunction.FilterGrayscale,
                k_FilterSepia => StyleValueFunction.FilterSepia,
                k_FilterBlur => StyleValueFunction.FilterBlur,
                _ => throw new ArgumentOutOfRangeException(nameof(ussValue), ussValue, "Unknown function name")
            };
        }

        public static string ToUssString(this StyleValueFunction svf) {
            return svf switch {
                StyleValueFunction.Var => k_Var,
                StyleValueFunction.Env => k_Env,
                StyleValueFunction.LinearGradient => k_LinearGradient,
                StyleValueFunction.NoneFilter => k_NoneFilter,
                StyleValueFunction.CustomFilter => k_CustomFilter,
                StyleValueFunction.FilterTint => k_FilterTint,
                StyleValueFunction.FilterOpacity => k_FilterOpacity,
                StyleValueFunction.FilterInvert => k_FilterInvert,
                StyleValueFunction.FilterGrayscale => k_FilterGrayscale,
                StyleValueFunction.FilterSepia => k_FilterSepia,
                StyleValueFunction.FilterBlur => k_FilterBlur,
                _ => throw new ArgumentOutOfRangeException(nameof(svf), svf, $"Unknown {nameof(StyleValueFunction)}")
            };
        }
    }
}
