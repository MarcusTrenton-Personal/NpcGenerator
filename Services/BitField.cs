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

namespace Services
{
    public struct BitField
    {
        public BitField(int x)
        {
            m_field = x;
        }

        public void Set(int bitIndex, bool value)
        {
            ParamUtil.VerifyInRange(nameof(bitIndex), bitIndex, MIN_INDEX, MAX_INDEX);

            if (value)
            {
                int mask = 1 << bitIndex;
                m_field |= mask;
            }
            else
            {
                int clearMask = ~(1 << bitIndex);
                m_field &= clearMask;
            }
        }

        public void Set(int startBitIndex, int bitCount, int value)
        {
            VerifyBitRange(startBitIndex, bitCount);

            int setMask = BitMask(startBitIndex, bitCount);
            int clearMask = ~setMask;
            int shiftedValue = value << startBitIndex;
            m_field = (m_field & clearMask) | (shiftedValue & setMask);
        }

        public bool Get(int bitIndex)
        {
            ParamUtil.VerifyInRange(nameof(bitIndex), bitIndex, MIN_INDEX, MAX_INDEX);

            int mask = 1 << bitIndex;
            int resultInt = m_field & mask;
            return resultInt != 0;
        }

        public int Get(int startBitIndex, int bitCount)
        {
            VerifyBitRange(startBitIndex, bitCount);
            int mask = BitMask(startBitIndex, bitCount);
            int shiftedValue = m_field & mask;
            return shiftedValue >> startBitIndex;
        }

        private void VerifyBitRange(int startBitIndex, int bitCount)
        {
            ParamUtil.VerifyInRange(nameof(startBitIndex), startBitIndex, MIN_INDEX, MAX_INDEX);
            ParamUtil.VerifyPositiveNumber(nameof(bitCount), bitCount);
            if (startBitIndex + bitCount > MAX_INDEX)
            {
                throw new ArgumentException("Bit range overflows container");
            }
        }

        private int BitMask(int startBitIndex, int bitCount)
        {
            int mask = 1;
            for (int i = 1; i < bitCount; i++)
            {
                mask <<= 1;
                mask++;
            }
            mask <<= startBitIndex;
            return mask;
        }

        public override string ToString()
        {
            return m_field.ToString();
        }

        private const int MIN_INDEX = 0;
        private const int MAX_INDEX = 31;
        private int m_field;
    }
}
