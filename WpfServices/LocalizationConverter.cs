/*Copyright(C) 2022 Marcus Trenton, marcus.trenton@gmail.com

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <https://www.gnu.org/licenses/>.*/

using Services;
using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfServices
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

            if (!(values[0] is ILocalization localization))
            {
                //At design-time, just return the localization id as a string.
                return parameter;
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
