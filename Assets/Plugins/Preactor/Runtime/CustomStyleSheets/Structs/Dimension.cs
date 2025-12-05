using System;
using System.Globalization;
using UnityEngine.UIElements;

namespace Preactor.CustomStyleSheets.Structs {
    public struct Dimension : IEquatable<Dimension> {
        public Unit unit;
        public float value;

        public Dimension(float value, Unit unit) {
            this.unit = unit;
            this.value = value;
        }

        public Length ToLength() =>
            new(
                value,
                unit == Unit.Percent ? LengthUnit.Percent : LengthUnit.Pixel
            );

        public TimeValue ToTime() =>
            new(
                value,
                unit == Unit.Millisecond ? TimeUnit.Millisecond : TimeUnit.Second
            );

        public Angle ToAngle() {
            return unit switch {
                Unit.Degree => new(value, AngleUnit.Degree),
                Unit.Gradian => new(value, AngleUnit.Gradian),
                Unit.Radian => new(value, AngleUnit.Radian),
                Unit.Turn => new(value, AngleUnit.Turn),
                _ => new(value, AngleUnit.Degree)
            };
        }

        public static bool operator ==(Dimension lhs, Dimension rhs) =>
            lhs.value == (double)rhs.value && lhs.unit == rhs.unit;

        public static bool operator !=(Dimension lhs, Dimension rhs) => !(lhs == rhs);

        public bool Equals(Dimension other) => other == this;

        public override bool Equals(object obj) => obj is Dimension dimension && dimension == this;

        public override int GetHashCode() =>
            (unchecked(-799583767 * -1521134295) + unit.GetHashCode()) * -1521134295 + value.GetHashCode();

        public override string ToString() {
            var str = unit switch {
                Unit.Unitless => string.Empty,
                Unit.Pixel => "px",
                Unit.Percent => "%",
                Unit.Second => "s",
                Unit.Millisecond => "ms",
                Unit.Degree => "deg",
                Unit.Gradian => "grad",
                Unit.Radian => "rad",
                Unit.Turn => "turn",
                _ => string.Empty
            };

            return value.ToString(CultureInfo.InvariantCulture.NumberFormat) + str;
        }

        public object ToOriginal() {
            var type = typeof(VisualElement).Assembly.GetType("UnityEngine.UIElements.StyleSheets.Dimension");
            var obj = Activator.CreateInstance(type);
            type.GetField("unit").SetValue(obj, (int)unit);
            type.GetField("value").SetValue(obj, value);
            return obj;
        }

        public enum Unit {
            Unitless,
            Pixel,
            Percent,
            Second,
            Millisecond,
            Degree,
            Gradian,
            Radian,
            Turn
        }
    }
}
