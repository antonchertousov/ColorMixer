using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ColorMixing.Converters
{
    /// <summary>
    /// Converts object value to corresponding string
    /// </summary>
    [ExcludeFromCodeCoverage]
    [ValueConversion(typeof(object), typeof(string))]
    public class ObjectToStringConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Invoked in order to convert the specified Ellipse object value into string
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var obj = value as Ellipse;

            if (obj == null)
            {
                return string.Empty;
            }

            var color = ((SolidColorBrush) obj.Fill).Color;
            return $"Object color: {color.ToString()}";
        }

        /// <summary>
        /// Convert string to object value is not supported
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}