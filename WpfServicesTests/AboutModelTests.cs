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
using System;
using WpfServices;

namespace Tests
{
    [TestClass]
    public class AboutModelTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullWebsite()
        {
            new AboutModel(website: null, donation: new Uri("https://www.fake.com"), supportEmail: "abc@fake.com");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullDonation()
        {
            new AboutModel(website: new Uri("https://www.fake.com"), donation: null, supportEmail: "abc@fake.com");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullSupportEmail()
        {
            new AboutModel(website: new Uri("https://www.fake.com"), donation: new Uri("https://www.fake.com"), supportEmail: null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void EmptySupportEmail()
        {
            new AboutModel(website: new Uri("https://www.fake.com"), donation: new Uri("https://www.fake.com"), supportEmail: "");
        }

        [TestMethod]
        public void VersionText()
        {
            AboutModel model = new AboutModel(website: new Uri("https://www.fake.com"), 
                donation: new Uri("https://www.fake.com"), 
                supportEmail: "abc@fake.com");
            string version = model.Version;

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
            AboutModel model = new AboutModel(website: new Uri("https://www.fake.com"),
                donation: new Uri("https://www.fake.com"),
                supportEmail: "abc@fake.com");
            bool canExecute = model.OpenBrowserToUri.CanExecute("This is not a Uri");
            Assert.IsFalse(canExecute, "Bad uri is accepted for OpenBrowserToUri");
        }

        [TestMethod]
        public void GoodUriAccepted()
        {
            AboutModel model = new AboutModel(website: new Uri("https://www.fake.com"),
                donation: new Uri("https://www.fake.com"),
                supportEmail: "abc@fake.com");
            bool canExecute = model.OpenBrowserToUri.CanExecute(new Uri("https://www.fake.com"));
            Assert.IsTrue(canExecute, "Good uri is rejected from OpenBrowserToUri");
        }
    }
}
