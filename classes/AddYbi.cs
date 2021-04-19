using System;
using System.Windows.Data;

namespace FstecThreatsToInformationSecurity.classes
{
    [ValueConversion(typeof(int), typeof(string))]
    public class AddYbi : IValueConverter  // Добавление УБИ. в таблице
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return ("УБИ." + ((int)value).ToString("#", culture));
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value.ToString().Replace("УБИ.", "");
        }
    }
}
