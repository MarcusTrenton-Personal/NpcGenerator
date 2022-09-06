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
using System.Collections.Generic;

namespace NpcGenerator
{
    public class Requirement
    {
        public Requirement (ILogicalExpression logicalExpression, NpcHolder npcHolder)
        {
            m_logicalExpression = logicalExpression ?? throw new ArgumentNullException(nameof(logicalExpression));
            m_npcHolder = npcHolder ?? throw new ArgumentNullException(nameof(npcHolder));
        }

        public bool IsUnlockedFor(Npc npc)
        {
            if (m_logicalExpression is null)
            {
                throw new InvalidOperationException(nameof(ILogicalExpression) + " is needed. Call Initalize() first.");
            }
            if (npc is null)
            {
                throw new ArgumentNullException(nameof(npc));
            }

            m_npcHolder.Npc = npc;
            bool result = m_logicalExpression.Evaluate();
            m_npcHolder.Npc = null;
            return result;
        }

        public HashSet<string> DependentCategories()
        {
            HashSet<string> dependentCategories = new HashSet<string>();
            HashSet<TraitId> dependentTraits = Dependencies();
            foreach (TraitId traitId in dependentTraits)
            {
                dependentCategories.Add(traitId.CategoryName);
            }

            return dependentCategories;
        }

        private HashSet<TraitId> Dependencies()
        {
            HashSet<TraitId> dependencies = new HashSet<TraitId>();
            AddDependencyTo(m_logicalExpression, dependencies);
            return dependencies;
        }

        private void AddDependencyTo(ILogicalExpression expression, HashSet<TraitId> dependencies)
        {
            if (expression is NpcHasTrait)
            {
                NpcHasTrait npcHasTrait = expression as NpcHasTrait;
                dependencies.Add(npcHasTrait.TraitId);
            }
            else if (expression is ILogicalOperator)
            {
                ILogicalOperator logicalOperator = expression as ILogicalOperator;
                IReadOnlyList<ILogicalExpression> subExpressions = logicalOperator.SubExpressions;
                foreach(ILogicalExpression subExpression in subExpressions)
                {
                    AddDependencyTo(subExpression, dependencies);
                }
            }
            else
            {
                throw new InvalidOperationException("Unknown ILogicalExpression. Add dependency gathering logic.");
            }
        }

        private readonly ILogicalExpression m_logicalExpression;
        private readonly NpcHolder m_npcHolder;
    }
}
