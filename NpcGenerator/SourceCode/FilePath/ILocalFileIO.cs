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

namespace NpcGenerator
{
    public struct FileContentProvider
    {
        public string FileExtensionWithoutDot { get; set; }
        public Func<string> GetContent { get; set; }
    }

    public interface ILocalFileIO
    {
        //Cache a copy of a file so it can already be open with a read/write lock at the same time 
        //it is read by this program.
        public string CacheFile(string originalPath);

        public bool SaveToPickedFile(IReadOnlyList<FileContentProvider> contentProviders, out string pickedFileExtension);
    }
}
