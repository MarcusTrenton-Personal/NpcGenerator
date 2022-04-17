using System;
using System.Collections.Generic;
using System.Text;

namespace NpcGenerator
{
    static class NumberHelper
    {
        public static bool TryParsePositiveNumber(string text, out int result)
        {
            bool isInt = int.TryParse(text, out result);
            bool isNaturalNumber = isInt && result > 0;
            return isNaturalNumber;
        }

        public static bool TryParseDigit(string text, out int result)
        {
            bool isInt = int.TryParse(text, out result);
            bool isDigit = isInt && result >= 0 && result < 10;
            return isDigit;
        }
    }
}
