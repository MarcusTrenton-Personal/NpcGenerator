using Services;
using System;
using System.Globalization;
using System.Windows.Data;

namespace NpcGenerator
{
    public class LocalizationConverter : IMultiValueConverter
    {
        //Parameter is string textId
        //value[0] is an ILocalization
        //value[1+] are the values for the formatted translated string
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values.Length < 1)
            {
                throw new ArgumentException("'values' contain at least a localization object");
            }

            ILocalization localization = values[0] as ILocalization;
            if (localization == null)
            {
                throw new ArgumentException("'values' index 0 must contain a non-null localization object");
            }

            string textId = parameter.ToString();
            if(values.Length == 1)
            {
                return localization.GetText(textId);
            }

            int formatParameterCount = values.Length - 1;
            string[] formatParameters = new string[formatParameterCount];
            for(int i = 0; i < formatParameterCount; ++i)
            {
                formatParameters[i] = values[i+1].ToString();
            }

            return localization.GetText(textId, formatParameters);
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
