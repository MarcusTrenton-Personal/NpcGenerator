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
along with this program. If not, see<https://www.gnu.org/licenses/>.*/

using Services;
using System;

namespace NpcGenerator
{
    public class Requirement : INpcProvider
    {
        public void Initialize(ILogicalExpression logicalExpression)
        {
            m_logicalExpression = logicalExpression ?? throw new ArgumentNullException(nameof(logicalExpression));
        }

        public Npc GetNpc()
        {
            return m_npc;
        }

        public bool IsUnlockedFor(Npc npc)
        {
            if (m_logicalExpression is null)
            {
                throw new InvalidOperationException(nameof(ILogicalExpression) + " is needed. Call Initalize() first.");
            }

            m_npc = npc ?? throw new ArgumentNullException(nameof(npc));
            bool result = m_logicalExpression.Evaluate();
            m_npc = null;
            return result;
        }

        private ILogicalExpression m_logicalExpression;
        private Npc m_npc = null;
    }
}
