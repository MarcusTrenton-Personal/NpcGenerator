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
along with this program.If not, see<https://www.gnu.org/licenses/>.*/

namespace NpcGenerator
{
    public interface IFileIO
    {
        //Cache a copy of a file so it can already be open with a read/write lock at the same time 
        //it is read by this program.
        public string CacheFile(string originalPath);

        public void SaveToPickedFile(string content, string fileType);
    }
}
