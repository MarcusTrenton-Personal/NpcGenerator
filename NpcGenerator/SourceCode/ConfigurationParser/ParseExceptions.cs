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

using System;

namespace NpcGenerator
{
    public class MismatchedBonusSelectionException : FormatException
    {
        public MismatchedBonusSelectionException(string notFoundCategory, string sourceCategory, string sourceTrait)
        {
            NotFoundCategoryName = notFoundCategory;
            SourceCategory = sourceCategory;
            SourceTrait = sourceTrait;
        }

        public string SourceCategory { get; private set; }
        public string SourceTrait { get; private set; }
        public string NotFoundCategoryName { get; private set; }
    }

    public class MismatchedReplacementTraitException : FormatException
    {
        public MismatchedReplacementTraitException(string category, string trait)
        {
            Category = category;
            Trait = trait;
        }

        public string Trait { get; private set; }
        public string Category { get; private set; }
    }

    public class MismatchedReplacementCategoryException : FormatException
    {
        public MismatchedReplacementCategoryException(string category, string trait)
        {
            Category = category;
            Trait = trait;
        }

        public string Trait { get; private set; }
        public string Category { get; private set; }
    }

    public class DuplicateCategoryNameException : FormatException
    {
        public DuplicateCategoryNameException(string category)
        {
            Category = category;
        }

        public string Category { get; private set; }
    }

    public class DuplicateTraitNamesInCategoryException : FormatException
    {
        public DuplicateTraitNamesInCategoryException(string category, string trait)
        {
            Category = category;
            Trait = trait;
        }

        public string Category { get; private set; }
        public string Trait { get; private set; }
    }
}
