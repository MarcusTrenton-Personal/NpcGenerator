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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;

namespace Tests
{
    [TestClass]
    public class RegexUtilTests
    {
        //Based on https://www.codeproject.com/Tips/216238/Regular-Expression-to-Validate-File-Path-and-Exten
        [DataTestMethod]
        [DataRow(@"\\192.168.0.1\folder\file.pdf")]
        [DataRow(@"\\192.168.0.1\my folder\folder.2\file.gif")]
        [DataRow(@"c:\my folder\abc abc.docx")]
        [DataRow(@"c:\my-folder\another_folder\abc.v2.docx")]
        [DataRow(@"c:\my-folder\ANOTHER_FOLDER\abc.v2.docx")]
        [DataRow(@"c:\my-folder\ANOTHER_FOLDER\abc.v2.mp3")]
        public void AbsoluteFilePathRegexPasses(string pattern)
        {
            bool isMatch = RegexUtil.ABSOLUTE_FILE_PATH.IsMatch(pattern);
            Assert.IsTrue(isMatch);
        }

        [DataTestMethod]
        [DataRow("", DisplayName = "Empty")]
        [DataRow("\n", DisplayName = "New line only")]
        [DataRow(@"\\192.168.0.1\folder\fi<le.pdf")]
        [DataRow(@"\\192.168.0.1\folder\\file.pdf")]
        [DataRow(@"\\192.168.0.1\my folder\folder.2\.gif")]
        [DataRow(@"c:\my folder\another_folder\.docx")]
        [DataRow(@"c:\my folder\\another_folder\abc.docx")]
        [DataRow(@"c:\my folder\another_folder\ab*c.v2.docx")]
        [DataRow(@"c:\my?folder\another_folder\abc.v2.docx")]
        [DataRow(@"file.abcde")]
        [DataRow(@"music\movies\imperial_march.mp3")]
        public void AbsoluteFilePathRegexFails(string pattern)
        {
            bool isMatch = RegexUtil.ABSOLUTE_FILE_PATH.IsMatch(pattern);
            Assert.IsFalse(isMatch);
        }

        [DataTestMethod]
        [DataRow(@"file.abcde")]
        [DataRow(@"music\movies\imperial_march.mp3")]
        [DataRow(@"../directory/catbus.gif")]
        [DataRow(@"directory/something\photo.jpeg")]
        [DataRow(@".htaccess")]
        public void RelativeFilePathRegexPasses(string pattern)
        {
            bool isMatch = RegexUtil.RELATIVE_FILE_PATH.IsMatch(pattern);
            Assert.IsTrue(isMatch);
        }

        [DataTestMethod]
        [DataRow("", DisplayName = "Empty")]
        [DataRow("\n", DisplayName = "New line only")]
        [DataRow(@"../directory/catbus.<gif")]
        [DataRow(@"directory?/something\photo.jpeg")]
        [DataRow(@"htaccess")]
        [DataRow(@"\\192.168.0.1\folder\file.pdf")]
        [DataRow(@"c:\my folder\abc abc.docx")]
        public void RelativeFilePathRegexFails(string pattern)
        {
            bool isMatch = RegexUtil.RELATIVE_FILE_PATH.IsMatch(pattern);
            Assert.IsFalse(isMatch);
        }

        [DataTestMethod]
        [DataRow(@"file.abcde")]
        [DataRow(@"music\movies\imperial_march.mp3")]
        [DataRow(@"../directory/catbus.gif")]
        [DataRow(@"directory/something\photo.jpeg")]
        [DataRow(@".htaccess")]
        [DataRow(@"\\192.168.0.1\folder\file.pdf")]
        [DataRow(@"\\192.168.0.1\my folder\folder.2\file.gif")]
        [DataRow(@"c:\my folder\abc abc.docx")]
        [DataRow(@"c:\my-folder\another_folder\abc.v2.docx")]
        [DataRow(@"c:\my-folder\ANOTHER_FOLDER\abc.v2.docx")]
        [DataRow(@"c:\my-folder\ANOTHER_FOLDER\abc.v2.mp3")]
        public void AnyFilePathRegexPasses(string pattern)
        {
            bool isMatch = RegexUtil.ANY_FILE_PATH.IsMatch(pattern);
            Assert.IsTrue(isMatch);
        }

        [DataTestMethod]
        [DataRow("", DisplayName = "Empty")]
        [DataRow("\n", DisplayName = "New line only")]
        [DataRow(@"\\192.168.0.1\folder\fi<le.pdf")]
        [DataRow(@"\\192.168.0.1\folder\\file.pdf")]
        [DataRow(@"\\192.168.0.1\my folder\folder.2\.gif")]
        [DataRow(@"c:\my folder\another_folder\.docx")]
        [DataRow(@"c:\my folder\\another_folder\abc.docx")]
        [DataRow(@"c:\my folder\another_folder\ab*c.v2.docx")]
        [DataRow(@"c:\my?folder\another_folder\abc.v2.docx")]
        public void AnyFilePathRegexFails(string pattern)
        {
            bool isMatch = RegexUtil.ANY_FILE_PATH.IsMatch(pattern);
            Assert.IsFalse(isMatch);
        }

        [DataTestMethod]
        [DataRow(@".mp3")]
        [DataRow(@".gif")]
        [DataRow(@".x")]
        [DataRow(@".htaccess")]
        public void FileExtensionWithDotPathRegexPasses(string pattern)
        {
            bool isMatch = RegexUtil.FILE_EXTENSION_WITH_DOT.IsMatch(pattern);
            Assert.IsTrue(isMatch);
        }

        [DataTestMethod]
        [DataRow("", DisplayName = "Empty")]
        [DataRow("\n", DisplayName = "New line only")]
        [DataRow(@"mp3")]
        [DataRow(@"..mp3")]
        [DataRow(@".!mp3")]
        [DataRow(@".mp3<")]
        [DataRow(@".mp$3")]
        [DataRow(@".mp*3")]
        public void FileExtensionWithDotPathRegexFails(string pattern)
        {
            bool isMatch = RegexUtil.FILE_EXTENSION_WITH_DOT.IsMatch(pattern);
            Assert.IsFalse(isMatch);
        }

        [DataTestMethod]
        [DataRow(@"mp3")]
        [DataRow(@"gif")]
        [DataRow(@"x")]
        [DataRow(@"htaccess")]
        public void FileExtensionWithoutDotPathRegexPasses(string pattern)
        {
            bool isMatch = RegexUtil.FILE_EXTENSION_WITHOUT_DOT.IsMatch(pattern);
            Assert.IsTrue(isMatch);
        }

        [DataTestMethod]
        [DataRow("", DisplayName = "Empty")]
        [DataRow("\n", DisplayName = "New line only")]
        [DataRow(@".mp3")]
        [DataRow(@"..mp3")]
        [DataRow(@"!mp3")]
        [DataRow(@"mp3<")]
        [DataRow(@"mp$3")]
        [DataRow(@"mp*3")]
        public void FileExtensionWithoutDotPathRegexFails(string pattern)
        {
            bool isMatch = RegexUtil.FILE_EXTENSION_WITHOUT_DOT.IsMatch(pattern);
            Assert.IsFalse(isMatch);
        }

        [DataTestMethod]
        [DataRow(@"en-ca")]
        [DataRow(@"EN-CA")]
        [DataRow(@"En-Ca")]
        public void LanguageCodeRegexPasses(string pattern)
        {
            bool isMatch = RegexUtil.LANGUAGE_CODE.IsMatch(pattern);
            Assert.IsTrue(isMatch);
        }

        [DataTestMethod]
        [DataRow("", DisplayName = "Empty")]
        [DataRow("\n", DisplayName = "New line only")]
        [DataRow("en")]
        [DataRow("en-usa")]
        [DataRow("english")]
        [DataRow("e2-ca")]
        [DataRow("en-c2")]
        [DataRow(@"en.ca")]
        [DataRow(@"en ca")]
        public void LanguageCodeRegexFails(string pattern)
        {
            bool isMatch = RegexUtil.LANGUAGE_CODE.IsMatch(pattern);
            Assert.IsFalse(isMatch);
        }
    }
}
