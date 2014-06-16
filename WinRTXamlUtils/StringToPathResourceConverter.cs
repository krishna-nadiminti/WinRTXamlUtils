using System;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace WinRTXamlUtils
{
    public sealed class StringToPathResourceConverter : IValueConverter
    {
        public string ResourceDictionaryName { get; set; }
        public Color PathFillColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            var pathResourceKey = value.ToString();

            if (parameter != null)
            {
                pathResourceKey = string.Format(parameter.ToString(), value);
            }
            var path = ImageLoader.LoadPathFrom(pathResourceKey, ResourceDictionaryName, PathFillColor);
            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
