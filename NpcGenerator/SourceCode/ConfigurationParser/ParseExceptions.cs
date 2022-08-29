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
        public MismatchedBonusSelectionException(string notFoundCategory, TraitId sourceTraitId)
        {
            NotFoundCategoryName = notFoundCategory;
            SourceTraitId = sourceTraitId;
        }

        public TraitId SourceTraitId { get; private set; }
        public string NotFoundCategoryName { get; private set; }
    }

    public class MismatchedReplacementTraitException : FormatException
    {
        public MismatchedReplacementTraitException(TraitId traitId)
        {
            TraitId = traitId;
        }

        public TraitId TraitId { get; private set; }
    }

    public class MismatchedReplacementCategoryException : FormatException
    {
        public MismatchedReplacementCategoryException(TraitId traitId)
        {
            TraitId = traitId;
        }

        public TraitId TraitId { get; private set; }
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
        public DuplicateTraitNamesInCategoryException(TraitId traitId)
        {
            TraitId = traitId;
        }

        public TraitId TraitId { get; private set; }
    }

    public class RequirementTraitIdNotFoundException : FormatException
    {
        public RequirementTraitIdNotFoundException(string requirementCategory, TraitId traitIdNotFound)
        {
            RequirementCategory = requirementCategory;
            TraitIdNotFound = traitIdNotFound;
        }

        public string RequirementCategory { get; private set; }
        public TraitId TraitIdNotFound { get; private set; }
    }

    public class UnknownLogicalOperatorException : FormatException
    {
        public UnknownLogicalOperatorException(string requirementCategory, string operatorName)
        {
            RequirementCategory = requirementCategory;
            OperatorName = operatorName;
        }

        public string RequirementCategory { get; private set; }
        public string OperatorName { get; private set; }
    }

    public class SelfRequiringCategoryException : FormatException
    {
        public SelfRequiringCategoryException(string category)
        {
            Category = category;
        }

        public string Category { get; private set; }
    }
}
