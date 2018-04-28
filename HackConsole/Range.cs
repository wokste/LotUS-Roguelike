using System;
using System.ComponentModel;

namespace HackConsole
{
    [TypeConverter(typeof(RangeTypeConverter))]
    public struct Range
    {
        public readonly int Min, Max;

        public Range(string str)
        {
            var tokens = str.Split('-');

            Min = Int32.Parse(tokens[0]);
            Max = Int32.Parse(tokens[tokens.Length - 1]);
        }

        public Range(int min, int max)
        {
            if (min <= max)
                (Min, Max) = (min, max);
            else
                (Min, Max) = (max, min);
        }

        public override string ToString()
        {
            return (Min == Max) ? $"{Min}" : $"{Min}-{Max}";
        }

        public int Rand(Random rnd)
        {
            return rnd.Next(Min, Max+1);
        }

        internal double RandDouble(Random rnd)
        {
            return rnd.NextDouble() * (Max - Min) + Min;
        }
    }

    public class RangeTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Range);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string s)
                return new Range(s);

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Range r)
                return r.ToString();

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
