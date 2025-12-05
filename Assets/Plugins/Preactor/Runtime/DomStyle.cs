using Preactor.Utils.Attributes;
using Puerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.UIElements.Cursor;

namespace Preactor {
    public class DomStyle {
        readonly Dom dom;

        public IStyle VisualElementStyle => dom.VisualElement.style;


        public object transform {
            get => $"{VisualElementStyle.translate} {VisualElementStyle.scale} {VisualElementStyle.rotate}";
            set {
                if (value is string s) {
                    var regex = new Regex(@"(\w+)\(([^)]+)\)");
                    var matches = regex.Matches(s);
                    foreach (Match match in matches) {
                        var transformType = match.Groups[1].Value.ToLower();
                        var transformValue = match.Groups[2].Value;

                        switch (transformType) {
                            case "translate":
                                if (TryParseStyleTranslate(transformValue, out var translateValue)) {
                                    VisualElementStyle.translate = translateValue;
                                }

                                break;
                            case "scale":
                                if (TryParseStyleScale(transformValue, out var scaleValue)) {
                                    VisualElementStyle.scale = scaleValue;
                                }

                                break;
                            case "rotate":
                                if (TryParseStyleRotate(transformValue, out var rotateValue)) {
                                    VisualElementStyle.rotate = rotateValue;
                                }

                                break;
                        }
                    }
                }
            }
        }

        public object transition {
            get =>
                $"{VisualElementStyle.transitionProperty} {VisualElementStyle.transitionDuration} {VisualElementStyle.transitionTimingFunction}";
            set {
                if (value is string s) {
                    var transitions = s.Split(',');

                    var properties = new List<StylePropertyName>();
                    var durations = new List<TimeValue>();
                    var easingFunctions = new List<EasingFunction>();

                    foreach (var transition in transitions) {
                        var parts = transition.Trim().Split(' ');
                        if (parts.Length < 2) {
                            continue;
                        }

                        properties.Add(new(parts[0]));

                        if (TryParseStyleListTimeValue(parts[1], out var duration)) {
                            durations.AddRange(duration.value);
                        }

                        if (parts.Length > 2 && TryParseStyleListEasingFunction(parts[2], out var easing)) {
                            easingFunctions.AddRange(easing.value);
                        }
                    }

                    VisualElementStyle.transitionProperty = new(properties);
                    VisualElementStyle.transitionDuration = new(durations);
                    VisualElementStyle.transitionTimingFunction = new(easingFunctions);
                }
            }
        }

        public DomStyle(Dom dom) {
            this.dom = dom;
        }

        [JsInterop]
        public object GetProperty(string key) {
            // TODO cache this
            var pi = GetType().GetProperty(key, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
            return pi != null ? pi.GetValue(this) : null;
        }

        [JsInterop]
        public void SetProperty(string key, object value) {
            var pi = GetType().GetProperty(key, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
            if (pi != null) {
                pi.SetValue(this, value);
            }
        }

        #region Style Properties

        public object alignContent {
            get => VisualElementStyle.alignContent;
            set {
                if (TryParseStyleEnum<Align>(value, out var styleEnum)) {
                    VisualElementStyle.alignContent = styleEnum;
                }
            }
        }

        public object alignItems {
            get => VisualElementStyle.alignItems;
            set {
                if (TryParseStyleEnum<Align>(value, out var styleEnum)) {
                    VisualElementStyle.alignItems = styleEnum;
                }
            }
        }

        public object alignSelf {
            get => VisualElementStyle.alignSelf;
            set {
                if (TryParseStyleEnum<Align>(value, out var styleEnum)) {
                    VisualElementStyle.alignSelf = styleEnum;
                }
            }
        }

        public object backgroundColor {
            get => VisualElementStyle.backgroundColor;
            set {
                if (TryParseStyleColor(value, out var styleColor)) {
                    VisualElementStyle.backgroundColor = styleColor;
                }
            }
        }

        public object backgroundImage {
            get => VisualElementStyle.backgroundImage;
            set {
                if (TryParseStyleBackground(value, out var styleBackground)) {
                    VisualElementStyle.backgroundImage = styleBackground;
                }
            }
        }

        public object backgroundSize {
            get => VisualElementStyle.backgroundSize;
            set {
                if (TryParseStyleBackgroundSize(value, out var styleBackgroundSize)) {
                    VisualElementStyle.backgroundSize = styleBackgroundSize;
                }
            }
        }

        public object backgroundRepeat {
            get => VisualElementStyle.backgroundRepeat;
            set {
                if (TryParseStyleBackgroundRepeat(value, out var styleBackgroundRepeat)) {
                    VisualElementStyle.backgroundRepeat = styleBackgroundRepeat;
                }
            }
        }

        public object backgroundPosition {
            get => (VisualElementStyle.backgroundPositionX, VisualElementStyle.backgroundPositionY);
            set => SetBackgroundPosition(value);
        }

        public object backgroundPositionX {
            get => VisualElementStyle.backgroundPositionX;
            set {
                if (TryParseStyleBackgroundPositionSingle(value, out var styleBackgroundPosition)) {
                    VisualElementStyle.backgroundPositionX = styleBackgroundPosition;
                }
            }
        }

        public object backgroundPositionY {
            get => VisualElementStyle.backgroundPositionY;
            set {
                if (TryParseStyleBackgroundPositionSingle(value, out var styleBackgroundPosition)) {
                    VisualElementStyle.backgroundPositionY = styleBackgroundPosition;
                }
            }
        }

        public object borderColor {
            get =>
                (VisualElementStyle.borderTopColor, VisualElementStyle.borderRightColor,
                    VisualElementStyle.borderBottomColor, VisualElementStyle.borderLeftColor);
            set => SetBorderColor(value);
        }

        public object borderWidth {
            get =>
                (VisualElementStyle.borderTopWidth, VisualElementStyle.borderRightWidth,
                    VisualElementStyle.borderBottomWidth, VisualElementStyle.borderLeftWidth);
            set => SetBorderWidth(value);
        }

        public object borderRadius {
            get =>
                (VisualElementStyle.borderTopLeftRadius, VisualElementStyle.borderTopRightRadius,
                    VisualElementStyle.borderBottomRightRadius,
                    VisualElementStyle.borderBottomLeftRadius);
            set => SetBorderRadius(value);
        }

        public object borderTopColor {
            get => VisualElementStyle.borderTopColor;
            set {
                if (TryParseStyleColor(value, out var styleColor)) {
                    VisualElementStyle.borderTopColor = styleColor;
                }
            }
        }

        public object borderRightColor {
            get => VisualElementStyle.borderRightColor;
            set {
                if (TryParseStyleColor(value, out var styleColor)) {
                    VisualElementStyle.borderRightColor = styleColor;
                }
            }
        }

        public object borderBottomColor {
            get => VisualElementStyle.borderBottomColor;
            set {
                if (TryParseStyleColor(value, out var styleColor)) {
                    VisualElementStyle.borderBottomColor = styleColor;
                }
            }
        }

        public object borderLeftColor {
            get => VisualElementStyle.borderLeftColor;
            set {
                if (TryParseStyleColor(value, out var styleColor)) {
                    VisualElementStyle.borderLeftColor = styleColor;
                }
            }
        }

        public object borderTopWidth {
            get => VisualElementStyle.borderTopWidth;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.borderTopWidth = styleLength.value.value;
                }
            }
        }

        public object borderRightWidth {
            get => VisualElementStyle.borderRightWidth;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.borderRightWidth = styleLength.value.value;
                }
            }
        }

        public object borderBottomWidth {
            get => VisualElementStyle.borderBottomWidth;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.borderBottomWidth = styleLength.value.value;
                }
            }
        }

        public object borderLeftWidth {
            get => VisualElementStyle.borderLeftWidth;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.borderLeftWidth = styleLength.value.value;
                }
            }
        }

        public object borderTopLeftRadius {
            get => VisualElementStyle.borderTopLeftRadius;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.borderTopLeftRadius = styleLength;
                }
            }
        }

        public object borderTopRightRadius {
            get => VisualElementStyle.borderTopRightRadius;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.borderTopRightRadius = styleLength;
                }
            }
        }

        public object borderBottomRightRadius {
            get => VisualElementStyle.borderBottomRightRadius;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.borderBottomRightRadius = styleLength;
                }
            }
        }

        public object borderBottomLeftRadius {
            get => VisualElementStyle.borderBottomLeftRadius;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.borderBottomLeftRadius = styleLength;
                }
            }
        }

        public object bottom {
            get => VisualElementStyle.bottom;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.bottom = styleLength;
                }
            }
        }

        public object color {
            get => VisualElementStyle.color;
            set {
                if (TryParseStyleColor(value, out var styleColor)) {
                    VisualElementStyle.color = styleColor;
                }
            }
        }

        public object cursor {
            get => VisualElementStyle.cursor;
            set {
                VisualElementStyle.cursor = value switch {
                    string ss when Enum.TryParse(ss, true, out StyleKeyword keyword) => new(keyword),
                    Cursor c => new(c),
                    _ => VisualElementStyle.cursor
                };
            }
        }

        public object display {
            get => VisualElementStyle.display;
            set {
                if (TryParseStyleEnum<DisplayStyle>(value, out var styleEnum)) {
                    VisualElementStyle.display = styleEnum;
                }
            }
        }

        public object flexBasis {
            get => VisualElementStyle.flexBasis;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.flexBasis = styleLength;
                }
            }
        }

        public object flexDirection {
            get => VisualElementStyle.flexDirection;
            set {
                if (TryParseStyleEnum<FlexDirection>(value, out var styleEnum)) {
                    VisualElementStyle.flexDirection = styleEnum;
                }
            }
        }

        public object flexGrow {
            get => VisualElementStyle.flexGrow;
            set {
                if (TryParseStyleFloat(value, out var styleFloat)) {
                    VisualElementStyle.flexGrow = styleFloat;
                }
            }
        }

        public object flexShrink {
            get => VisualElementStyle.flexShrink;
            set {
                if (TryParseStyleFloat(value, out var styleFloat)) {
                    VisualElementStyle.flexShrink = styleFloat;
                }
            }
        }

        public object flexWrap {
            get => VisualElementStyle.flexWrap;
            set {
                if (TryParseStyleEnum<Wrap>(value, out var styleEnum)) {
                    VisualElementStyle.flexWrap = styleEnum;
                }
            }
        }

        public object fontSize {
            get => VisualElementStyle.fontSize;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.fontSize = styleLength;
                }
            }
        }

        public object height {
            get => VisualElementStyle.height;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.height = styleLength;
                }
            }
        }

        public object justifyContent {
            get => VisualElementStyle.justifyContent;
            set {
                if (TryParseStyleEnum<Justify>(value, out var styleEnum)) {
                    VisualElementStyle.justifyContent = styleEnum;
                }
            }
        }

        public object left {
            get => VisualElementStyle.left;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.left = styleLength;
                }
            }
        }

        public object letterSpacing {
            get => VisualElementStyle.letterSpacing;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.letterSpacing = styleLength;
                }
            }
        }

        public object margin {
            get =>
                (VisualElementStyle.marginTop, VisualElementStyle.marginRight, VisualElementStyle.marginBottom,
                    VisualElementStyle.marginLeft);
            set => SetMargin(value);
        }

        public object marginTop {
            get => VisualElementStyle.marginTop;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.marginTop = styleLength;
                }
            }
        }

        public object marginRight {
            get => VisualElementStyle.marginRight;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.marginRight = styleLength;
                }
            }
        }

        public object marginBottom {
            get => VisualElementStyle.marginBottom;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.marginBottom = styleLength;
                }
            }
        }

        public object marginLeft {
            get => VisualElementStyle.marginLeft;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.marginLeft = styleLength;
                }
            }
        }

        public object maxHeight {
            get => VisualElementStyle.maxHeight;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.maxHeight = styleLength;
                }
            }
        }

        public object maxWidth {
            get => VisualElementStyle.maxWidth;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.maxWidth = styleLength;
                }
            }
        }

        public object minHeight {
            get => VisualElementStyle.minHeight;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.minHeight = styleLength;
                }
            }
        }

        public object minWidth {
            get => VisualElementStyle.minWidth;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.minWidth = styleLength;
                }
            }
        }

        public object opacity {
            get => VisualElementStyle.opacity;
            set {
                if (TryParseStyleFloat(value, out var styleFloat)) {
                    VisualElementStyle.opacity = styleFloat;
                }
            }
        }

        public object overflow {
            get => VisualElementStyle.overflow;
            set {
                if (TryParseStyleEnum<Overflow>(value, out var styleEnum)) {
                    VisualElementStyle.overflow = styleEnum;
                }
            }
        }

        public object padding {
            get =>
                (VisualElementStyle.paddingTop, VisualElementStyle.paddingRight, VisualElementStyle.paddingBottom,
                    VisualElementStyle.paddingLeft);
            set => SetPadding(value);
        }

        public object paddingTop {
            get => VisualElementStyle.paddingTop;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.paddingTop = styleLength;
                }
            }
        }

        public object paddingRight {
            get => VisualElementStyle.paddingRight;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.paddingRight = styleLength;
                }
            }
        }

        public object paddingBottom {
            get => VisualElementStyle.paddingBottom;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.paddingBottom = styleLength;
                }
            }
        }

        public object paddingLeft {
            get => VisualElementStyle.paddingLeft;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.paddingLeft = styleLength;
                }
            }
        }

        public object position {
            get => VisualElementStyle.position;
            set {
                if (TryParseStyleEnum<Position>(value, out var styleEnum)) {
                    VisualElementStyle.position = styleEnum;
                }
            }
        }

        public object right {
            get => VisualElementStyle.right;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.right = styleLength;
                }
            }
        }

        public object rotate {
            get => VisualElementStyle.rotate;
            set {
                if (TryParseStyleRotate(value, out var styleRotate)) {
                    VisualElementStyle.rotate = styleRotate;
                }
            }
        }

        public object scale {
            get => VisualElementStyle.scale;
            set {
                if (TryParseStyleScale(value, out var styleScale)) {
                    VisualElementStyle.scale = styleScale;
                }
            }
        }

        public object textOverflow {
            get => VisualElementStyle.textOverflow;
            set {
                if (TryParseStyleEnum<TextOverflow>(value, out var styleEnum)) {
                    VisualElementStyle.textOverflow = styleEnum;
                }
            }
        }

        public object textShadow {
            get => VisualElementStyle.textShadow;
            set {
                if (TryParseStyleTextShadow(value, out var styleTextShadow)) {
                    VisualElementStyle.textShadow = styleTextShadow;
                }
            }
        }

        public object top {
            get => VisualElementStyle.top;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.top = styleLength;
                }
            }
        }

        public object transformOrigin {
            get => VisualElementStyle.transformOrigin;
            set {
                if (TryParseStyleTransformOrigin(value, out var styleTransformOrigin)) {
                    VisualElementStyle.transformOrigin = styleTransformOrigin;
                }
            }
        }

        public object transitionDelay {
            get => VisualElementStyle.transitionDelay;
            set {
                if (TryParseStyleListTimeValue(value, out var timeValues)) {
                    VisualElementStyle.transitionDelay = timeValues;
                }
            }
        }

        public object transitionDuration {
            get => VisualElementStyle.transitionDuration;
            set {
                if (TryParseStyleListTimeValue(value, out var timeValues)) {
                    VisualElementStyle.transitionDuration = timeValues;
                }
            }
        }

        public object transitionProperty {
            get => VisualElementStyle.transitionProperty;
            set {
                // NOTE: make sure to use StylePropertyName and not PropertyName, as the latter is not
                // from UIElements
                if (TryParseStyleListPropertyName(value, out var propertyNames)) {
                    VisualElementStyle.transitionProperty = propertyNames;
                }
            }
        }

        public object transitionTimingFunction {
            get => VisualElementStyle.transitionTimingFunction;
            set {
                if (TryParseStyleListEasingFunction(value, out var timingFunctions)) {
                    VisualElementStyle.transitionTimingFunction = timingFunctions;
                }
            }
        }

        public object translate {
            get => VisualElementStyle.translate;
            set {
                if (TryParseStyleTranslate(value, out var styleTranslate)) {
                    VisualElementStyle.translate = styleTranslate;
                }
            }
        }

        public object unityBackgroundImageTintColor {
            get => VisualElementStyle.unityBackgroundImageTintColor;
            set {
                if (TryParseStyleColor(value, out var styleColor)) {
                    VisualElementStyle.unityBackgroundImageTintColor = styleColor;
                }
            }
        }

        public object unityBackgroundScaleMode {
            get => VisualElementStyle.unityBackgroundScaleMode;
            set {
                if (TryParseStyleEnum<ScaleMode>(value, out var styleEnum)) {
                    VisualElementStyle.unityBackgroundScaleMode = styleEnum;
                }
            }
        }

        public object unityFont {
            get => VisualElementStyle.unityFont;
            set {
                if (TryParseStyleFont(value, out var styleFont)) {
                    VisualElementStyle.unityFont = styleFont;
                }
            }
        }

        public object unityFontDefinition {
            get => VisualElementStyle.unityFontDefinition;
            set {
                if (TryParseStyleFontDefinition(value, out var styleFontDefinition)) {
                    VisualElementStyle.unityFontDefinition = styleFontDefinition;
                }
            }
        }

        public object unityFontStyleAndWeight {
            get => VisualElementStyle.unityFontStyleAndWeight;
            set {
                if (TryParseStyleEnum<FontStyle>(value, out var styleEnum)) {
                    VisualElementStyle.unityFontStyleAndWeight = styleEnum;
                }
            }
        }

        public object unityOverflowClipBox {
            get => VisualElementStyle.unityOverflowClipBox;
            set {
                if (TryParseStyleEnum<OverflowClipBox>(value, out var styleEnum)) {
                    VisualElementStyle.unityOverflowClipBox = styleEnum;
                }
            }
        }

        public object unityParagraphSpacing {
            get => VisualElementStyle.unityParagraphSpacing;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.unityParagraphSpacing = styleLength;
                }
            }
        }

        public object unitySliceBottom {
            get => VisualElementStyle.unitySliceBottom;
            set {
                if (TryParseStyleInt(value, out var styleInt)) {
                    VisualElementStyle.unitySliceBottom = styleInt;
                }
            }
        }

        public object unitySliceLeft {
            get => VisualElementStyle.unitySliceLeft;
            set {
                if (TryParseStyleInt(value, out var styleInt)) {
                    VisualElementStyle.unitySliceLeft = styleInt;
                }
            }
        }

        public object unitySliceRight {
            get => VisualElementStyle.unitySliceRight;
            set {
                if (TryParseStyleInt(value, out var styleInt)) {
                    VisualElementStyle.unitySliceRight = styleInt;
                }
            }
        }

        public object unitySliceTop {
            get => VisualElementStyle.unitySliceTop;
            set {
                if (TryParseStyleInt(value, out var styleInt)) {
                    VisualElementStyle.unitySliceTop = styleInt;
                }
            }
        }

        public object unitySliceScale {
            get => VisualElementStyle.unitySliceScale;
            set {
                if (TryParseStyleFloat(value, out var styleFloat)) {
                    VisualElementStyle.unitySliceScale = styleFloat;
                }
            }
        }

        public object unityTextAlign {
            get => VisualElementStyle.unityTextAlign;
            set {
                if (TryParseStyleEnum<TextAnchor>(value, out var styleEnum)) {
                    VisualElementStyle.unityTextAlign = styleEnum;
                }
            }
        }

        public object unityTextOutlineColor {
            get => VisualElementStyle.unityTextOutlineColor;
            set {
                if (TryParseStyleColor(value, out var styleColor)) {
                    VisualElementStyle.unityTextOutlineColor = styleColor;
                }
            }
        }

        public object unityTextOutlineWidth {
            get => VisualElementStyle.unityTextOutlineWidth;
            set {
                if (TryParseStyleFloat(value, out var styleFloat)) {
                    VisualElementStyle.unityTextOutlineWidth = styleFloat;
                }
            }
        }

        public object unityTextOverflowPosition {
            get => VisualElementStyle.unityTextOverflowPosition;
            set {
                if (TryParseStyleEnum<TextOverflowPosition>(value, out var styleEnum)) {
                    VisualElementStyle.unityTextOverflowPosition = styleEnum;
                }
            }
        }

        public object visibility {
            get => VisualElementStyle.visibility;
            set {
                if (TryParseStyleEnum<Visibility>(value, out var styleEnum)) {
                    VisualElementStyle.visibility = styleEnum;
                }
            }
        }

        public object whiteSpace {
            get => VisualElementStyle.whiteSpace;
            set {
                if (TryParseStyleEnum<WhiteSpace>(value, out var styleEnum)) {
                    VisualElementStyle.whiteSpace = styleEnum;
                }
            }
        }

        public object width {
            get => VisualElementStyle.width;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.width = styleLength;
                }
            }
        }

        public object wordSpacing {
            get => VisualElementStyle.wordSpacing;
            set {
                if (TryParseStyleLength(value, out var styleLength)) {
                    VisualElementStyle.wordSpacing = styleLength;
                }
            }
        }

        #endregion


        #region ParseStyles

        static bool TryParseStyleEnum<T>(object value, out StyleEnum<T> styleEnum) where T : struct, IConvertible {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleEnum = new(keyword);
                    return true;
                case null:
                    styleEnum = new(StyleKeyword.Null);
                    return true;
                case StyleEnum<T> se:
                    styleEnum = se;
                    return true;
                case T t:
                    styleEnum = new(t);
                    return true;
                case double d when Enum.IsDefined(typeof(T), (int)d):
                    styleEnum = new((T)(object)(int)d);
                    return true;
                case string s when Enum.TryParse(typeof(T), s, true, out var e):
                    styleEnum = new((T)e);
                    return true;
                default:
                    styleEnum = default;
                    return false;
            }
        }

        static bool TryParseStyleColor(object value, out StyleColor styleColor) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleColor = new(keyword);
                    return true;
                case null:
                    styleColor = new(StyleKeyword.Null);
                    return true;
                case StyleColor sc:
                    styleColor = sc;
                    return true;
                case string s: {
                    var c = ColorUtility.TryParseHtmlString(s, out var color) ? color : Color.white;
                    styleColor = new(c);
                    return true;
                }
                case Color c:
                    styleColor = new(c);
                    return true;
                default:
                    styleColor = default;
                    return false;
            }
        }

        bool TryParseStyleBackground(object value, out StyleBackground styleBackground) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleBackground = new(keyword);
                    return true;
                case null:
                    styleBackground = new(StyleKeyword.Null);
                    return true;
                case StyleBackground sb:
                    styleBackground = sb;
                    return true;
                case Background b:
                    styleBackground = new(b);
                    return true;
                case Texture2D t:
                    styleBackground = new(Background.FromTexture2D(t));
                    return true;
                case Sprite sp:
                    styleBackground = new(Background.FromSprite(sp));
                    return true;
                case RenderTexture rt:
                    styleBackground = new(Background.FromRenderTexture(rt));
                    return true;
                case VectorImage vi:
                    styleBackground = new(Background.FromVectorImage(vi));
                    return true;
                case string s: {
                    var texture = dom.Document.LoadImage(s);
                    if (texture != null) {
                        styleBackground = new(Background.FromSprite(texture));
                        return true;
                    }

                    break;
                }
            }

            styleBackground = default;
            return false;
        }

        static bool TryParseStyleBackgroundSize(object value, out StyleBackgroundSize styleBackgroundSize) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleBackgroundSize = new(keyword);
                    return true;
                case null:
                    styleBackgroundSize = new(StyleKeyword.Null);
                    return true;
                case StyleBackgroundSize sbs:
                    styleBackgroundSize = sbs;
                    return true;
                case BackgroundSize bs:
                    styleBackgroundSize = new(bs);
                    return true;
                case string str: {
                    var parts = str.ToLower().Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (GetLength(parts[0], out var x)) {
                        if (parts.Length > 1 && GetLength(parts[1], out var y)) {
                            styleBackgroundSize = new BackgroundSize(x, y);
                            return true;
                        }

                        styleBackgroundSize = new BackgroundSize(
                            x,
                            x
                        ); // If only one value is provided, use it for both x and y
                        return true;
                    }

                    break;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 1: {
                    var l = jsObj.Get<float>("0");
                    styleBackgroundSize = new BackgroundSize(l, l);
                    return true;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 2: {
                    var x = jsObj.Get<float>("0");
                    var y = jsObj.Get<float>("1");
                    styleBackgroundSize = new BackgroundSize(x, y);
                    return true;
                }
            }

            styleBackgroundSize = default;
            return false;
        }

        static bool TryParseStyleBackgroundRepeat(object value, out StyleBackgroundRepeat styleBackgroundRepeat) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleBackgroundRepeat = new(keyword);
                    return true;
                case null:
                    styleBackgroundRepeat = new(StyleKeyword.Null);
                    return true;
                case StyleBackgroundRepeat sbr:
                    styleBackgroundRepeat = sbr;
                    return true;
                case BackgroundRepeat br:
                    styleBackgroundRepeat = new(br);
                    return true;
                case string str: {
                    var parts = str.ToLower().Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    switch (parts.Length) {
                        case 1 when Enum.TryParse(parts[0], true, out Repeat repeat):
                            styleBackgroundRepeat = new BackgroundRepeat(repeat, repeat);
                            return true;
                        case 2 when Enum.TryParse(parts[0], true, out Repeat x)
                            && Enum.TryParse(parts[1], true, out Repeat y):
                            styleBackgroundRepeat = new BackgroundRepeat(x, y);
                            return true;
                    }

                    break;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 1: {
                    var r = jsObj.Get<Repeat>("0");
                    styleBackgroundRepeat = new BackgroundRepeat(r, r);
                    return true;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 2: {
                    var x = jsObj.Get<Repeat>("0");
                    var y = jsObj.Get<Repeat>("1");
                    styleBackgroundRepeat = new BackgroundRepeat(x, y);
                    return true;
                }
            }

            styleBackgroundRepeat = default;
            return false;
        }

        void SetBackgroundPosition(object value) {
            switch (value) {
                case string s when Enum.TryParse(s, true, out StyleKeyword keyword):
                    dom.VisualElement.style.backgroundPositionX = new(keyword);
                    dom.VisualElement.style.backgroundPositionY = new(keyword);
                    return;
                case null:
                    dom.VisualElement.style.backgroundPositionX = new(StyleKeyword.Null);
                    dom.VisualElement.style.backgroundPositionY = new(StyleKeyword.Null);
                    return;
                case StyleBackgroundPosition sbp:
                    dom.VisualElement.style.backgroundPositionX = sbp;
                    dom.VisualElement.style.backgroundPositionY = sbp;
                    return;
                case BackgroundPosition bp:
                    dom.VisualElement.style.backgroundPositionX = new(bp);
                    dom.VisualElement.style.backgroundPositionY = new(bp);
                    return;
                case string str: {
                    var parts = str.ToLower().Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    switch (parts.Length) {
                        case 1: {
                            if (Enum.TryParse(parts[0], true, out BackgroundPositionKeyword x)) {
                                dom.VisualElement.style.backgroundPositionX = new BackgroundPosition(x);
                                dom.VisualElement.style.backgroundPositionY = new BackgroundPosition(x);
                            }

                            break;
                        }
                        case 2: {
                            if (Enum.TryParse(parts[0], true, out BackgroundPositionKeyword x)
                                && GetLength(parts[1], out var l)) {
                                dom.VisualElement.style.backgroundPositionX = new BackgroundPosition(x, l);
                            } else if (Enum.TryParse(parts[0], true, out x)
                                       && Enum.TryParse(parts[1], true, out BackgroundPositionKeyword y)) {
                                dom.VisualElement.style.backgroundPositionX = new BackgroundPosition(x, 0);
                                dom.VisualElement.style.backgroundPositionY = new BackgroundPosition(y, 0);
                            }

                            break;
                        }
                        case 4: {
                            if (Enum.TryParse(parts[0], true, out BackgroundPositionKeyword x)
                                && GetLength(parts[1], out var lx)
                                && Enum.TryParse(parts[2], true, out BackgroundPositionKeyword y)
                                && GetLength(parts[3], out var ly)) {
                                dom.VisualElement.style.backgroundPositionX = new BackgroundPosition(x, lx);
                                dom.VisualElement.style.backgroundPositionY = new BackgroundPosition(y, ly);
                            }

                            break;
                        }
                    }

                    break;
                }
            }
        }

        static bool TryParseStyleBackgroundPositionSingle(
            object value,
            out StyleBackgroundPosition styleBackgroundPosition
        ) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleBackgroundPosition = new(keyword);
                    return true;
                case null:
                    styleBackgroundPosition = new(StyleKeyword.Null);
                    return true;
                case StyleBackgroundPosition sbp:
                    styleBackgroundPosition = sbp;
                    return true;
                case BackgroundPosition bp:
                    styleBackgroundPosition = new(bp);
                    return true;
                case string s: {
                    var parts = s.ToLower().Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    switch (parts.Length) {
                        case 1 when Enum.TryParse(parts[0], true, out BackgroundPositionKeyword posKeyword):
                            styleBackgroundPosition = new BackgroundPosition(posKeyword);
                            return true;
                        case 2 when Enum.TryParse(parts[0], true, out BackgroundPositionKeyword posKeyword)
                            && GetLength(parts[1], out var l):
                            styleBackgroundPosition = new BackgroundPosition(posKeyword, l);
                            return true;
                    }

                    break;
                }
            }

            styleBackgroundPosition = default;
            return false;
        }

        void SetBorderColor(object value) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    SetBorderColorKeyword(dom, keyword);
                    return;
                case null:
                    SetBorderColorKeyword(dom, StyleKeyword.Null);
                    return;
                case StyleColor sc:
                    SetBorderColors(dom, sc.value, sc.value, sc.value, sc.value);
                    return;
                case string s: {
                    var parts = s.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    switch (parts.Length) {
                        case 1: {
                            if (TryParseColorString(parts[0], out var c)) {
                                SetBorderColors(dom, c, c, c, c);
                            }

                            break;
                        }
                        case 2: {
                            if (TryParseColorString(parts[0], out var tb)
                                && TryParseColorString(parts[1], out var lr)) {
                                SetBorderColors(dom, tb, lr, tb, lr);
                            }

                            break;
                        }
                        case 3: {
                            if (TryParseColorString(parts[0], out var t)
                                && TryParseColorString(parts[1], out var lr)
                                && TryParseColorString(parts[2], out var b)) {
                                SetBorderColors(dom, t, lr, b, lr);
                            }

                            break;
                        }
                        case 4: {
                            if (TryParseColorString(parts[0], out var t)
                                && TryParseColorString(parts[1], out var r)
                                && TryParseColorString(parts[2], out var b)
                                && TryParseColorString(parts[3], out var l)) {
                                SetBorderColors(dom, t, r, b, l);
                            }

                            break;
                        }
                    }

                    break;
                }
                case Color c:
                    SetBorderColors(dom, c, c, c, c);
                    break;
                case JSObject jsObj when jsObj.Get<int>("length") == 1: {
                    var cc = jsObj.Get<Color>("0");
                    SetBorderColors(dom, cc, cc, cc, cc);
                    break;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 2: {
                    var tb = jsObj.Get<Color>("0");
                    var lr = jsObj.Get<Color>("1");
                    SetBorderColors(dom, tb, lr, tb, lr);
                    break;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 3: {
                    var t = jsObj.Get<Color>("0");
                    var lr = jsObj.Get<Color>("1");
                    var b = jsObj.Get<Color>("2");
                    SetBorderColors(dom, t, lr, b, lr);
                    break;
                }
                case JSObject jsObj: {
                    if (jsObj.Get<int>("length") == 4) {
                        var t = jsObj.Get<Color>("0");
                        var r = jsObj.Get<Color>("1");
                        var b = jsObj.Get<Color>("2");
                        var l = jsObj.Get<Color>("3");
                        SetBorderColors(dom, t, r, b, l);
                    }

                    break;
                }
            }
        }

        void SetBorderWidth(object value) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    SetBorderWidthKeyword(dom, keyword);
                    return;
                case null:
                    SetBorderWidthKeyword(dom, StyleKeyword.Null);
                    return;
                case StyleFloat sf:
                    SetBorderWidths(dom, sf.value, sf.value, sf.value, sf.value);
                    return;
                case string s: {
                    var parts = s.Replace("px", "")
                        .Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    switch (parts.Length) {
                        case 1: {
                            if (float.TryParse(parts[0], out var l)) {
                                SetBorderWidths(dom, l, l, l, l);
                            }

                            break;
                        }
                        case 2: {
                            if (float.TryParse(parts[0], out var tb) && float.TryParse(parts[1], out var lr)) {
                                SetBorderWidths(dom, tb, lr, tb, lr);
                            }

                            break;
                        }
                        case 3: {
                            if (float.TryParse(parts[0], out var t)
                                && float.TryParse(parts[1], out var lr)
                                && float.TryParse(parts[2], out var b)) {
                                SetBorderWidths(dom, t, lr, b, lr);
                            }

                            break;
                        }
                        case 4: {
                            if (float.TryParse(parts[0], out var t)
                                && float.TryParse(parts[1], out var r)
                                && float.TryParse(parts[2], out var b)
                                && float.TryParse(parts[3], out var l)) {
                                SetBorderWidths(dom, t, r, b, l);
                            }

                            break;
                        }
                    }

                    break;
                }
                case double d:
                    SetBorderWidths(dom, (float)d, (float)d, (float)d, (float)d);
                    break;
                case JSObject jsObj when jsObj.Get<int>("length") == 1: {
                    var l = jsObj.Get<float>("0");
                    SetBorderWidths(dom, l, l, l, l);
                    break;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 2: {
                    var tb = jsObj.Get<float>("0");
                    var lr = jsObj.Get<float>("1");
                    SetBorderWidths(dom, tb, lr, tb, lr);
                    break;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 3: {
                    var t = jsObj.Get<float>("0");
                    var lr = jsObj.Get<float>("1");
                    var b = jsObj.Get<float>("2");
                    SetBorderWidths(dom, t, lr, b, lr);
                    break;
                }
                case JSObject jsObj: {
                    if (jsObj.Get<int>("length") == 4) {
                        var t = jsObj.Get<float>("0");
                        var r = jsObj.Get<float>("1");
                        var b = jsObj.Get<float>("2");
                        var l = jsObj.Get<float>("3");
                        SetBorderWidths(dom, t, r, b, l);
                    }

                    break;
                }
            }
        }

        void SetBorderRadius(object value) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    SetBorderRadiusKeyword(dom, keyword);
                    return;
                case null:
                    SetBorderRadiusKeyword(dom, StyleKeyword.Null);
                    return;
                case StyleLength sl:
                    SetBorderRadii(dom, sl.value, sl.value, sl.value, sl.value);
                    return;
                case Length ll:
                    SetBorderRadii(dom, ll, ll, ll, ll);
                    return;
                case string s: {
                    var parts = s.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    switch (parts.Length) {
                        case 1: {
                            if (GetLength(parts[0], out var l)) {
                                SetBorderRadii(dom, l, l, l, l);
                            }

                            break;
                        }
                        case 2: {
                            if (GetLength(parts[0], out var tlbr) && GetLength(parts[1], out var trbl)) {
                                SetBorderRadii(dom, tlbr, trbl, tlbr, trbl);
                            }

                            break;
                        }
                        case 3: {
                            if (GetLength(parts[0], out var tl)
                                && GetLength(parts[1], out var trbl)
                                && GetLength(parts[2], out var br)) {
                                SetBorderRadii(dom, tl, trbl, br, trbl);
                            }

                            break;
                        }
                        case 4: {
                            if (GetLength(parts[0], out var tl)
                                && GetLength(parts[1], out var tr)
                                && GetLength(parts[2], out var br)
                                && GetLength(parts[3], out var bl)) {
                                SetBorderRadii(dom, tl, tr, br, bl);
                            }

                            break;
                        }
                    }

                    break;
                }
                case double d: {
                    var l = new Length((float)d);
                    SetBorderRadii(dom, l, l, l, l);
                    break;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 1: {
                    var l = new Length(jsObj.Get<float>("0"));
                    SetBorderRadii(dom, l, l, l, l);
                    break;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 2: {
                    var tlbr = new Length(jsObj.Get<float>("0"));
                    var trbl = new Length(jsObj.Get<float>("1"));
                    SetBorderRadii(dom, tlbr, trbl, trbl, tlbr);
                    break;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 3: {
                    var tl = new Length(jsObj.Get<float>("0"));
                    var trbl = new Length(jsObj.Get<float>("1"));
                    var br = new Length(jsObj.Get<float>("2"));
                    SetBorderRadii(dom, tl, trbl, br, trbl);
                    break;
                }
                case JSObject jsObj: {
                    if (jsObj.Get<int>("length") == 4) {
                        var tl = new Length(jsObj.Get<float>("0"));
                        var tr = new Length(jsObj.Get<float>("1"));
                        var br = new Length(jsObj.Get<float>("2"));
                        var bl = new Length(jsObj.Get<float>("3"));
                        SetBorderRadii(dom, tl, tr, br, bl);
                    }

                    break;
                }
            }
        }

        static bool TryParseStyleLength(object value, out StyleLength styleLength) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleLength = new(keyword);
                    return true;
                case null:
                    styleLength = new(StyleKeyword.Null);
                    return true;
                case StyleLength sl:
                    styleLength = sl;
                    return true;
                case Length ll:
                    styleLength = new(ll);
                    return true;
                case string s when GetLength(s, out var length):
                    styleLength = new(length);
                    return true;
                case double d:
                    styleLength = new((float)d);
                    return true;
                default:
                    styleLength = default;
                    return false;
            }
        }

        static bool TryParseStyleFloat(object value, out StyleFloat styleFloat) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleFloat = new(keyword);
                    return true;
                case null:
                    styleFloat = new(StyleKeyword.Null);
                    return true;
                case StyleFloat sf:
                    styleFloat = sf;
                    return true;
                case double d:
                    styleFloat = new((float)d);
                    return true;
                default:
                    styleFloat = default;
                    return false;
            }
        }

        void SetMargin(object value) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    SetMarginKeyword(dom, keyword);
                    return;
                case null:
                    SetMarginKeyword(dom, StyleKeyword.Null);
                    return;
                case StyleLength sl:
                    SetMargins(dom, sl.value, sl.value, sl.value, sl.value);
                    return;
                case Length ll:
                    SetMargins(dom, ll, ll, ll, ll);
                    return;
                case string s: {
                    var parts = s.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    switch (parts.Length) {
                        case 1: {
                            if (GetLength(parts[0], out var l)) {
                                SetMargins(dom, l, l, l, l);
                            }

                            break;
                        }
                        case 2: {
                            if (GetLength(parts[0], out var tb) && GetLength(parts[1], out var lr)) {
                                SetMargins(dom, tb, lr, tb, lr);
                            }

                            break;
                        }
                        case 3: {
                            if (GetLength(parts[0], out var t)
                                && GetLength(parts[1], out var lr)
                                && GetLength(parts[2], out var b)) {
                                SetMargins(dom, t, lr, b, lr);
                            }

                            break;
                        }
                        case 4: {
                            if (GetLength(parts[0], out var t)
                                && GetLength(parts[1], out var r)
                                && GetLength(parts[2], out var b)
                                && GetLength(parts[3], out var l)) {
                                SetMargins(dom, t, r, b, l);
                            }

                            break;
                        }
                    }

                    break;
                }
                case double d: {
                    var l = new Length((float)d);
                    SetMargins(dom, l, l, l, l);
                    break;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 1: {
                    var l = new Length(jsObj.Get<float>("0"));
                    SetMargins(dom, l, l, l, l);
                    break;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 2: {
                    var tb = new Length(jsObj.Get<float>("0"));
                    var lr = new Length(jsObj.Get<float>("1"));
                    SetMargins(dom, tb, lr, tb, lr);
                    break;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 3: {
                    var t = new Length(jsObj.Get<float>("0"));
                    var lr = new Length(jsObj.Get<float>("1"));
                    var b = new Length(jsObj.Get<float>("2"));
                    SetMargins(dom, t, lr, b, lr);
                    break;
                }
                case JSObject jsObj: {
                    if (jsObj.Get<int>("length") == 4) {
                        var t = new Length(jsObj.Get<float>("0"));
                        var r = new Length(jsObj.Get<float>("1"));
                        var b = new Length(jsObj.Get<float>("2"));
                        var l = new Length(jsObj.Get<float>("3"));
                        SetMargins(dom, t, r, b, l);
                    }

                    break;
                }
            }
        }

        void SetPadding(object value) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    SetPaddingKeyword(dom, keyword);
                    return;
                case null:
                    SetPaddingKeyword(dom, StyleKeyword.Null);
                    return;
                case StyleLength sl:
                    SetPaddings(dom, sl.value, sl.value, sl.value, sl.value);
                    return;
                case Length ll:
                    SetPaddings(dom, ll, ll, ll, ll);
                    return;
                case string s: {
                    var parts = s.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    switch (parts.Length) {
                        case 1: {
                            if (GetLength(parts[0], out var l)) {
                                SetPaddings(dom, l, l, l, l);
                            }

                            break;
                        }
                        case 2: {
                            if (GetLength(parts[0], out var tb) && GetLength(parts[1], out var lr)) {
                                SetPaddings(dom, tb, lr, tb, lr);
                            }

                            break;
                        }
                        case 3: {
                            if (GetLength(parts[0], out var t)
                                && GetLength(parts[1], out var lr)
                                && GetLength(parts[2], out var b)) {
                                SetPaddings(dom, t, lr, b, lr);
                            }

                            break;
                        }
                        case 4: {
                            if (GetLength(parts[0], out var t)
                                && GetLength(parts[1], out var r)
                                && GetLength(parts[2], out var b)
                                && GetLength(parts[3], out var l)) {
                                SetPaddings(dom, t, r, b, l);
                            }

                            break;
                        }
                    }

                    break;
                }
                case double d: {
                    var l = new Length((float)d);
                    SetPaddings(dom, l, l, l, l);
                    break;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 1: {
                    var l = new Length(jsObj.Get<float>("0"));
                    SetPaddings(dom, l, l, l, l);
                    break;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 2: {
                    var tb = new Length(jsObj.Get<float>("0"));
                    var lr = new Length(jsObj.Get<float>("1"));
                    SetPaddings(dom, tb, lr, tb, lr);
                    break;
                }
                case JSObject jsObj when jsObj.Get<int>("length") == 3: {
                    var t = new Length(jsObj.Get<float>("0"));
                    var lr = new Length(jsObj.Get<float>("1"));
                    var b = new Length(jsObj.Get<float>("2"));
                    SetPaddings(dom, t, lr, b, lr);
                    break;
                }
                case JSObject jsObj: {
                    if (jsObj.Get<int>("length") == 4) {
                        var t = new Length(jsObj.Get<float>("0"));
                        var r = new Length(jsObj.Get<float>("1"));
                        var b = new Length(jsObj.Get<float>("2"));
                        var l = new Length(jsObj.Get<float>("3"));
                        SetPaddings(dom, t, r, b, l);
                    }

                    break;
                }
            }
        }

        readonly Regex rotateRegex = new(@"(-?\d+\.?\d*|\.\d+)(deg|grad|rad|turn)", RegexOptions.IgnoreCase);

        bool TryParseStyleRotate(object value, out StyleRotate styleRotate) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleRotate = new(keyword);
                    return true;
                case null:
                    styleRotate = new(StyleKeyword.Null);
                    return true;
                case StyleRotate sr:
                    styleRotate = sr;
                    return true;
                case Rotate r:
                    styleRotate = new(r);
                    return true;
                case Angle a:
                    styleRotate = new(new Rotate(a));
                    return true;
                case string s: {
                    var match = rotateRegex.Match(s);
                    if (match.Success) {
                        var f = float.Parse(match.Groups[1].Value);
                        var unit = match.Groups[2].Value.ToLower();

                        var angleUnit = unit switch {
                            "deg" => AngleUnit.Degree,
                            "grad" => AngleUnit.Gradian,
                            "rad" => AngleUnit.Radian,
                            "turn" => AngleUnit.Turn,
                            _ => AngleUnit.Degree
                        };

                        styleRotate = new Rotate(new Angle(f, angleUnit));
                        return true;
                    }

                    break;
                }
                case double d:
                    styleRotate = new(new Rotate((float)d));
                    return true;
                case JSObject jsObj: {
                    var f = jsObj.Get<float>("0");
                    styleRotate = new(new Rotate(f));
                    return true;
                }
            }

            styleRotate = default;
            return false;
        }

        static bool TryParseStyleScale(object value, out StyleScale styleScale) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleScale = new(keyword);
                    return true;
                case null:
                    styleScale = new(StyleKeyword.Null);
                    return true;
                case StyleScale ssss:
                    styleScale = ssss;
                    return true;
                case Scale sss:
                    styleScale = new(sss);
                    return true;
                case string s: {
                    var parts = s.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    switch (parts.Length) {
                        case 1 when float.TryParse(parts[0], out var l):
                            styleScale = new Scale(new Vector2(l, l));
                            return true;
                        case 2 when float.TryParse(parts[0], out var x) && float.TryParse(parts[1], out var y):
                            styleScale = new Scale(new Vector2(x, y));
                            return true;
                    }

                    break;
                }
                case double d:
                    styleScale = new(new Scale(new Vector2((float)d, (float)d)));
                    return true;
                case JSObject jsObj when jsObj.Get<int>("length") == 2: {
                    var x = jsObj.Get<float>("0");
                    var y = jsObj.Get<float>("1");
                    styleScale = new(new Scale(new Vector2(x, y)));
                    return true;
                }
            }

            styleScale = default;
            return false;
        }

        static bool TryParseStyleTextShadow(object value, out StyleTextShadow styleTextShadow) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleTextShadow = new(keyword);
                    return true;
                case null:
                    styleTextShadow = new(StyleKeyword.Null);
                    return true;
                case StyleTextShadow sts:
                    styleTextShadow = sts;
                    return true;
                case TextShadow ts:
                    styleTextShadow = new(ts);
                    return true;
                case string s: {
                    var parts = s.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    switch (parts.Length) {
                        case 3 when GetLength(parts[0], out var x)
                            && GetLength(parts[1], out var y)
                            && GetLength(parts[2], out var blur):
                            styleTextShadow =
                                new TextShadow { offset = new(x.value, y.value), blurRadius = blur.value };
                            return true;
                        case 4 when GetLength(parts[0], out var x)
                            && GetLength(parts[1], out var y)
                            && GetLength(parts[2], out var blur)
                            && TryParseColorString(parts[3], out var color):
                            styleTextShadow = new TextShadow {
                                offset = new(x.value, y.value), blurRadius = blur.value, color = color
                            };
                            return true;
                    }

                    break;
                }
            }

            styleTextShadow = default;
            return false;
        }

        static bool TryParseStyleTransformOrigin(object value, out StyleTransformOrigin styleTransformOrigin) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleTransformOrigin = new(keyword);
                    return true;
                case null:
                    styleTransformOrigin = new(StyleKeyword.Null);
                    return true;
                case StyleTransformOrigin sto:
                    styleTransformOrigin = sto;
                    return true;
                case TransformOrigin to:
                    styleTransformOrigin = new(to);
                    return true;
                case string s: {
                    var parts = s.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    switch (parts.Length) {
                        case 1 when GetLength(parts[0], out var l):
                            styleTransformOrigin = new TransformOrigin(l, l);
                            return true;
                        case 2 when GetLength(parts[0], out var x) && GetLength(parts[1], out var y):
                            styleTransformOrigin = new TransformOrigin(x, y);
                            return true;
                    }

                    break;
                }
                case double d:
                    styleTransformOrigin = new(new TransformOrigin((float)d, (float)d));
                    return true;
                case JSObject jsObj when jsObj.Get<int>("length") == 2: {
                    var x = jsObj.Get<float>("0");
                    var y = jsObj.Get<float>("1");
                    styleTransformOrigin = new(new TransformOrigin(x, y));
                    return true;
                }
            }

            styleTransformOrigin = default;
            return false;
        }

        readonly Regex timeRegex = new(@"(-?\d+\.?\d*)(s|ms)", RegexOptions.IgnoreCase);

        bool TryParseStyleListTimeValue(object value, out StyleList<TimeValue> styleListTimeValue) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleListTimeValue = new(keyword);
                    return true;
                case null:
                    styleListTimeValue = new(StyleKeyword.Null);
                    return true;
                case StyleList<TimeValue> sltv:
                    styleListTimeValue = sltv;
                    return true;
                case List<TimeValue> ltv:
                    styleListTimeValue = new(ltv);
                    return true;
                case string s: {
                    var parts = s.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var timeValues = new List<TimeValue>();
                    foreach (var part in parts) {
                        var match = timeRegex.Match(part);
                        if (match.Success) {
                            var f = float.Parse(match.Groups[1].Value);
                            var unit = match.Groups[2].Value.ToLower();

                            var timeUnit = unit switch {
                                "s" => TimeUnit.Second,
                                "ms" => TimeUnit.Millisecond,
                                _ => TimeUnit.Second
                            };

                            timeValues.Add(new(f, timeUnit));
                        }
                    }

                    styleListTimeValue = new(timeValues);
                    return true;
                }
                case TimeValue tv:
                    styleListTimeValue = new(new List<TimeValue> { tv });
                    return true;
                case JSObject jsObj: {
                    var timeValues = new List<TimeValue>();
                    for (var i = 0; i < jsObj.Get<int>("length"); i++) {
                        var match = timeRegex.Match(jsObj.Get<string>(i.ToString()));
                        if (match.Success) {
                            var f = float.Parse(match.Groups[1].Value);
                            var unit = match.Groups[2].Value.ToLower();

                            var timeUnit = unit switch {
                                "s" => TimeUnit.Second,
                                "ms" => TimeUnit.Millisecond,
                                _ => TimeUnit.Second
                            };

                            timeValues.Add(new(f, timeUnit));
                        }
                    }

                    styleListTimeValue = new(timeValues);
                    return true;
                }
            }

            styleListTimeValue = default;
            return false;
        }

        static bool TryParseStyleListPropertyName(
            object value,
            out StyleList<StylePropertyName> styleListPropertyName
        ) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleListPropertyName = new(keyword);
                    return true;
                case null:
                    styleListPropertyName = new(StyleKeyword.Null);
                    return true;
                case StyleList<StylePropertyName> slpn:
                    styleListPropertyName = slpn;
                    return true;
                case List<StylePropertyName> lpn:
                    styleListPropertyName = new(lpn);
                    return true;
                case string s: {
                    var parts = s.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var propertyNames = parts.Select(part => (StylePropertyName)part).ToList();

                    styleListPropertyName = new(propertyNames);
                    return true;
                }
                case StylePropertyName spn:
                    styleListPropertyName = new(new List<StylePropertyName> { spn });
                    return true;
                case JSObject jsObj: {
                    var propertyNames = new List<StylePropertyName>();
                    for (var i = 0; i < jsObj.Get<int>("length"); i++) {
                        propertyNames.Add(jsObj.Get<string>(i.ToString()));
                    }

                    styleListPropertyName = new(propertyNames);
                    return true;
                }
                default:
                    styleListPropertyName = default;
                    return false;
            }
        }

        static bool TryParseStyleListEasingFunction(
            object value,
            out StyleList<EasingFunction> styleListEasingFunction
        ) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleListEasingFunction = new(keyword);
                    return true;
                case null:
                    styleListEasingFunction = new(StyleKeyword.Null);
                    return true;
                case StyleList<EasingFunction> slef:
                    styleListEasingFunction = slef;
                    return true;
                case List<EasingFunction> lef:
                    styleListEasingFunction = new(lef);
                    return true;
                case string s: {
                    var parts = s.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var easingFunctions = new List<EasingFunction>();
                    foreach (var part in parts) {
                        if (Enum.TryParse(part.Replace("_", "").Replace("-", ""), true, out EasingMode easing)) {
                            easingFunctions.Add(easing);
                        }
                    }

                    styleListEasingFunction = new(easingFunctions);
                    return true;
                }
                case EasingFunction ef:
                    styleListEasingFunction = new(new List<EasingFunction> { ef });
                    return true;
                case JSObject jsObj: {
                    var easingFunctions = new List<EasingFunction>();
                    for (var i = 0; i < jsObj.Get<int>("length"); i++) {
                        if (Enum.TryParse(jsObj.Get<string>(i.ToString()), true, out EasingMode easing)) {
                            easingFunctions.Add(easing);
                        }
                    }

                    styleListEasingFunction = new(easingFunctions);
                    return true;
                }
                default:
                    styleListEasingFunction = default;
                    return false;
            }
        }

        static bool TryParseStyleTranslate(object value, out StyleTranslate styleTranslate) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleTranslate = new(keyword);
                    return true;

                case null:
                    styleTranslate = new(StyleKeyword.Null);
                    return true;

                case Translate translate:
                    styleTranslate = translate;
                    return true;

                case StyleTranslate st:
                    styleTranslate = st;
                    return true;

                case string s: {
                    var parts = s.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    switch (parts.Length) {
                        case 1 when GetLength(parts[0], out var l):
                            styleTranslate = new Translate(l, l);
                            return true;
                        case 2 when GetLength(parts[0], out var x) && GetLength(parts[1], out var y):
                            styleTranslate = new Translate(x, y);
                            return true;
                    }

                    break;
                }

                case double d:
                    styleTranslate = new(new Translate((float)d, (float)d));
                    return true;

                case Vector2 v2:
                    styleTranslate = new(new Translate(v2.x, v2.y));
                    return true;

                case Vector3 v3:
                    styleTranslate = new(new Translate(v3.x, v3.y));
                    return true;

                case JSObject jsObj when jsObj.Get<int>("length") == 2: {
                    var x = jsObj.Get<float>("0");
                    var y = jsObj.Get<float>("1");
                    styleTranslate = new(new Translate(x, y));
                    return true;
                }
            }

            styleTranslate = default;
            return false;
        }

        bool TryParseStyleFont(object value, out StyleFont styleFont) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleFont = new(keyword);
                    return true;
                case null:
                    styleFont = new(StyleKeyword.Null);
                    return true;
                case StyleFont sf:
                    styleFont = sf;
                    return true;
                case Font f:
                    styleFont = new(f);
                    return true;
                case string s: {
                    var font = dom.Document.LoadFont(s);
                    if (font != null) {
                        styleFont = new(font);
                        return true;
                    }

                    break;
                }
            }

            styleFont = default;
            return false;
        }

        bool TryParseStyleFontDefinition(object value, out StyleFontDefinition styleFontDefinition) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleFontDefinition = new(keyword);
                    return true;
                case null:
                    styleFontDefinition = new(StyleKeyword.Null);
                    return true;
                case StyleFontDefinition sfd:
                    styleFontDefinition = sfd;
                    return true;
                case FontDefinition fd:
                    styleFontDefinition = new(fd);
                    return true;
                case string s: {
                    var fontDefinition = dom.Document.LoadFontDefinition(s);
                    if (fontDefinition != null) {
                        styleFontDefinition = new(fontDefinition);
                        return true;
                    }

                    break;
                }
            }

            styleFontDefinition = default;
            return false;
        }

        static bool TryParseStyleInt(object value, out StyleInt styleInt) {
            switch (value) {
                case string ss when Enum.TryParse(ss, true, out StyleKeyword keyword):
                    styleInt = new(keyword);
                    return true;

                case null:
                    styleInt = new(StyleKeyword.Null);
                    return true;

                case StyleInt si:
                    styleInt = si;
                    return true;

                case double d:
                    styleInt = new((int)d);
                    return true;

                case string str:
                    var match = new Regex(@"^(\d+)").Match(str);
                    if (match.Success) {
                        styleInt = new(int.Parse(match.Groups[1].Value));
                        return true;
                    }

                    break;
            }

            styleInt = default;
            return false;
        }

        #endregion

        #region Static Utils

        static bool GetLength(object value, out Length lengthValue) {
            switch (value) {
                case string s: {
                    var str = s.EndsWith("px") ? s[..^2] : s.EndsWith("%") ? s[..^1] : s;
                    if (float.TryParse(str, out var percentValue)) {
                        lengthValue = new(percentValue, s.EndsWith("%") ? LengthUnit.Percent : LengthUnit.Pixel);
                        return true;
                    }

                    break;
                }
                case double doubleValue:
                    lengthValue = new((float)doubleValue);
                    return true;
            }

            lengthValue = default;
            return false;
        }

        static bool TryParseColorString(string s, out Color color) => ColorUtility.TryParseHtmlString(s, out color);

        static void SetBorderColors(Dom dom, Color t, Color r, Color b, Color l) {
            dom.VisualElement.style.borderTopColor = new(t);
            dom.VisualElement.style.borderRightColor = new(r);
            dom.VisualElement.style.borderBottomColor = new(b);
            dom.VisualElement.style.borderLeftColor = new(l);
        }

        static void SetBorderColorKeyword(Dom dom, StyleKeyword keyword) {
            dom.VisualElement.style.borderTopColor = new(keyword);
            dom.VisualElement.style.borderRightColor = new(keyword);
            dom.VisualElement.style.borderBottomColor = new(keyword);
            dom.VisualElement.style.borderLeftColor = new(keyword);
        }

        static void SetBorderWidths(Dom dom, float t, float r, float b, float l) {
            dom.VisualElement.style.borderTopWidth = new(t);
            dom.VisualElement.style.borderRightWidth = new(r);
            dom.VisualElement.style.borderBottomWidth = new(b);
            dom.VisualElement.style.borderLeftWidth = new(l);
        }

        static void SetBorderWidthKeyword(Dom dom, StyleKeyword keyword) {
            dom.VisualElement.style.borderTopWidth = new(keyword);
            dom.VisualElement.style.borderRightWidth = new(keyword);
            dom.VisualElement.style.borderBottomWidth = new(keyword);
            dom.VisualElement.style.borderLeftWidth = new(keyword);
        }

        static void SetBorderRadii(Dom dom, Length tl, Length tr, Length br, Length bl) {
            dom.VisualElement.style.borderTopLeftRadius = new(tl);
            dom.VisualElement.style.borderTopRightRadius = new(tr);
            dom.VisualElement.style.borderBottomRightRadius = new(br);
            dom.VisualElement.style.borderBottomLeftRadius = new(bl);
        }

        static void SetBorderRadiusKeyword(Dom dom, StyleKeyword keyword) {
            dom.VisualElement.style.borderTopLeftRadius = new(keyword);
            dom.VisualElement.style.borderTopRightRadius = new(keyword);
            dom.VisualElement.style.borderBottomRightRadius = new(keyword);
            dom.VisualElement.style.borderBottomLeftRadius = new(keyword);
        }

        static void SetMargins(Dom dom, Length t, Length r, Length b, Length l) {
            dom.VisualElement.style.marginTop = new(t);
            dom.VisualElement.style.marginRight = new(r);
            dom.VisualElement.style.marginBottom = new(b);
            dom.VisualElement.style.marginLeft = new(l);
        }

        static void SetMarginKeyword(Dom dom, StyleKeyword keyword) {
            dom.VisualElement.style.marginTop = new(keyword);
            dom.VisualElement.style.marginRight = new(keyword);
            dom.VisualElement.style.marginBottom = new(keyword);
            dom.VisualElement.style.marginLeft = new(keyword);
        }

        static void SetPaddings(Dom dom, Length t, Length r, Length b, Length l) {
            dom.VisualElement.style.paddingTop = new(t);
            dom.VisualElement.style.paddingRight = new(r);
            dom.VisualElement.style.paddingBottom = new(b);
            dom.VisualElement.style.paddingLeft = new(l);
        }

        static void SetPaddingKeyword(Dom dom, StyleKeyword keyword) {
            dom.VisualElement.style.paddingTop = new(keyword);
            dom.VisualElement.style.paddingRight = new(keyword);
            dom.VisualElement.style.paddingBottom = new(keyword);
            dom.VisualElement.style.paddingLeft = new(keyword);
        }

        #endregion
    }
}
