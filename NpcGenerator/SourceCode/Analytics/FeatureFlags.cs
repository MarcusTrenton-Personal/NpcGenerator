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
using System.Collections.Generic;
using System.Text;

namespace NpcGenerator
{
    [Flags]
    public enum NpcConfigurationFeatures
    {
        Basic =                 0,
        Weight =                1 << 0,
        MultipleNpcs =          1 << 1,
        MultipleSelection =     1 << 2,
        BonusSelection =        1 << 3,
        HiddenTrait =           1 << 4,
        HiddenCategory =        1 << 5,
        OutputCategoryName =    1 << 6,
        CategoryOrder =         1 << 7,
        Replacement =           1 << 8,
        CategoryRequirement =   1 << 9,
        TraitRequirement =      1 << 10,
    }
}
