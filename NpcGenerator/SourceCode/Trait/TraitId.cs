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
    public class TraitId : IEquatable<TraitId>
    {
        public TraitId(string categoryName, string traitName)
        {
            CategoryName = categoryName;
            TraitName = traitName;
        }

        //Taken from Microsoft example at
        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type
        public bool Equals(TraitId other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            return (CategoryName == other.CategoryName) && (TraitName == other.TraitName);
        }

        public override bool Equals(object obj)
        {
            if (obj is TraitId other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode() => (CategoryName, TraitName).GetHashCode();

        public static bool operator ==(TraitId lhs, TraitId rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }

                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(TraitId lhs, TraitId rhs) => !(lhs == rhs);

        public string CategoryName { get; private set; }
        public string TraitName { get; private set; }
    }
}
