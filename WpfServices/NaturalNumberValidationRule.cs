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
along with this program.If not, see<https://www.gnu.org/licenses/>.*/

using Services;
using System.Globalization;
using System.Windows.Controls;

namespace WpfServices
{
    public class NaturalNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is int number)
            {
                return number > 0 ? new ValidationResult(true, null) : new ValidationResult(false, "Value is not greater than 0");
            }
            if (!(value is string text))
            {
                return new ValidationResult(false, "Value must be text or int");
            }
            if (!NumberHelper.TryParsePositiveInteger(text, out _))
            {
                return new ValidationResult(false, "Value is not a natural number");
            }

            return new ValidationResult(true, null);
        }
    }
}
