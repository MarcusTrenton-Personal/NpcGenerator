/*Copyright(C) 2023 Marcus Trenton, marcus.trenton@gmail.com

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
using System;
using System.Text;

namespace Tests
{
    [TestClass]
    public class UriHelperTests
    {
        //There is no way to test a successful WriteEmail(). But, there are many ways to test failure.

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void WriteEmailNullEmailAddress()
        {
            UriHelper.WriteEmail(emailAddress: null, subject: "Peace", body: "Our last, best hope");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void WriteEmailEmptyEmailAddress()
        {
            UriHelper.WriteEmail(emailAddress: string.Empty, subject: "Peace", body: "Our last, best hope");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void WriteEmailNullSubject()
        {
            UriHelper.WriteEmail(emailAddress: "Babylon@Year2259.net", subject: null, body: "Our last, best hope");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void WriteEmailEmptySubject()
        {
            UriHelper.WriteEmail(emailAddress: "Babylon@Year2259.net", subject: string.Empty, body: "Our last, best hope");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void WriteEmailNullBody()
        {
            UriHelper.WriteEmail(emailAddress: "Babylon@Year2259.net", subject: "Peace", body: null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void WriteEmailEmptyBody()
        {
            UriHelper.WriteEmail(emailAddress: "Babylon@Year2259.net", subject: "Peace", body: string.Empty);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void WriteEmailTooLong()
        {
            const int TOO_MANY_CHARACTERS_COUNT = UriHelper.MAX_EMAIL_MESSAGE_LENGTH + 100;
            StringBuilder builder = new StringBuilder(TOO_MANY_CHARACTERS_COUNT);
            builder.Append('b', TOO_MANY_CHARACTERS_COUNT);
            UriHelper.WriteEmail(emailAddress: "Babylon@Year2259.net", subject: "Peace", body: builder.ToString());
        }
    }
}
