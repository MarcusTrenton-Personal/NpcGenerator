﻿/*Copyright(C) 2022 Marcus Trenton, marcus.trenton@gmail.com

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

namespace NpcGenerator
{
    public class Replacement
    {
        public Replacement(in Trait originalTrait, string replacementTraitName, in TraitCategory category)
        {
            ParamUtil.VerifyNotNull(nameof(originalTrait), originalTrait);
            ParamUtil.VerifyHasContent(nameof(replacementTraitName), replacementTraitName);
            ParamUtil.VerifyNotNull(nameof(category), category);

            OriginalTrait = originalTrait;
            ReplacementTraitName = replacementTraitName;
            Category = category;
        }

        public Trait OriginalTrait { get; private set; }
        public string ReplacementTraitName { get; private set; }
        public TraitCategory Category { get; private set; }
    }
}
