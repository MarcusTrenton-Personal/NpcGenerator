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
    public class NpcHasTrait : ILogicalExpression
    {
        public NpcHasTrait(TraitId traitId, INpcProvider npcProvider)
        {
            m_traitId = traitId ?? throw new ArgumentNullException(nameof(traitId));
            m_npcProvider = npcProvider ?? throw new ArgumentNullException(nameof(npcProvider));
        }

        public bool Evaluate()
        {
            Npc npc = m_npcProvider.GetNpc() ?? throw new InvalidOperationException(nameof(npc) + " given by INpcProvider is null.");
            bool hasTrait = npc.HasTrait(m_traitId);
            return hasTrait;
        }

        private readonly TraitId m_traitId;
        private readonly INpcProvider m_npcProvider;
    }
}
