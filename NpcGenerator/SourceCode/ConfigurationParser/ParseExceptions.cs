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
using System.IO;

namespace NpcGenerator
{
    public class EmptyFileException : IOException
    {
        public EmptyFileException(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; private set; }
    }

    public class CategoryWeightMismatchException : FormatException
    {
    }

    public class EmptyCategoryNameException : FormatException
    {
    }

    public class TraitMissingCategoryException : FormatException
    {
        public TraitMissingCategoryException(string traitName)
        {
            TraitName = traitName;
        }

        public string TraitName { get; private set; }
    }

    public class MissingWeightException : FormatException
    {
        public MissingWeightException(TraitId traitId)
        {
            TraitId = traitId;
        }

        public TraitId TraitId { get; private set; }
    }

    public class WeightIsNotWholeNumberException : FormatException
    {
        public WeightIsNotWholeNumberException(TraitId traitId, string invalidWeight)
        {
            TraitId = traitId;
            InvalidWeight = invalidWeight;
        }

        public TraitId TraitId { get; private set; }
        public string InvalidWeight { get; private set; }
    }

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

    public class MissingReplacementTraitException : FormatException
    {
        public MissingReplacementTraitException(TraitId traitId)
        {
            TraitId = traitId;
        }

        public TraitId TraitId { get; private set; }
    }

    public class MissingReplacementCategoryException : FormatException
    {
        public MissingReplacementCategoryException(TraitId traitId)
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

    public class DuplicateTraitNameInCategoryException : FormatException
    {
        public DuplicateTraitNameInCategoryException(TraitId traitId)
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

    public class SelfRequiringTraitException : FormatException
    {
        public SelfRequiringTraitException(TraitId traitId)
        {
            TraitId = traitId;
        }

        public TraitId TraitId { get; private set; }
    }

    public class CircularRequirementsException : FormatException
    {
        public CircularRequirementsException(List<TraitSchema.Dependency> cycle)
        {
            Cycle = cycle;
        }

        public List<TraitSchema.Dependency> Cycle;
    }

    public class ConflictingCategoryVisibilityException : FormatException
    {
        public ConflictingCategoryVisibilityException(List<TraitCategory> conflicingCategories)
        {
            ConflictingCategories = conflicingCategories;
        }

        public List<TraitCategory> ConflictingCategories { get; private set; }
    }

    public class UnknownSortCriteriaException : FormatException
    {
        public UnknownSortCriteriaException(string sortBy)
        {
            SortCriteria = sortBy;
        }

        public string SortCriteria { get; private set; }
    }
}
