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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WpfServices;

namespace Tests
{
    [TestClass]
    public class AboutModelTests
    {
        [TestMethod]
        public void VersionText()
        {
            string version = m_model.Version;

            string[] parts = version.Split('.');
            Assert.IsTrue(parts.Length == 3 || parts.Length == 4, "Version should be 3 or 4 parts separated by .");
            for(int i = 0; i < parts.Length; i++)
            {
                Assert.IsTrue(int.TryParse(parts[i], out int number), "Part " + i + " must be an integer");
                Assert.IsTrue(number >= 0, "Part " + i + " must be non-negative");
            }
        }

        [TestMethod]
        public void BadUriRejected()
        {
            bool canExecute = m_model.OpenBrowserToUri.CanExecute("This is not a Uri");
            Assert.IsFalse(canExecute, "Bad uri is accepted for OpenBrowserToUri");
        }

        [TestMethod]
        public void GoodUriAccepted()
        {
            bool canExecute = m_model.OpenBrowserToUri.CanExecute(new Uri("https://www.fake.com"));
            Assert.IsTrue(canExecute, "Good uri is rejected from OpenBrowserToUri");
        }

        private readonly AboutModel m_model = new AboutModel(website: null, donation: null);
    }


}
