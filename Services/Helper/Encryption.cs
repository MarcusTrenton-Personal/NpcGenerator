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

using System;
using System.Diagnostics;
using System.Text;

namespace Services
{
    public static class Encryption
    {
        //Take from https://www.codingame.com/playgrounds/11117/simple-encryption-using-c-and-xor-technique

        [DebuggerHidden] //Thwart basic debugger attack. Try harder, hacker!
        public static string XorEncryptDecrypt(string text, int key)
        {
            if(text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            StringBuilder inputSb = new StringBuilder(text);
            StringBuilder outputSb = new StringBuilder(text.Length);
            char character;
            for (int i = 0; i < text.Length; i++)
            {
                character = inputSb[i];
                character = (char)(character ^ key);
                outputSb.Append(character);
            }
            return outputSb.ToString();
        }
    }
}