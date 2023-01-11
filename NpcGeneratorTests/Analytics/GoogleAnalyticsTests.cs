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
using NpcGenerator;
using System;

namespace Tests
{
    [TestClass]
    public class GoogleAnalyticsTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullAppSettings()
        {
            new GoogleAnalytics(
                appSettings: null, 
                new StubTrackingProfile(), 
                new StubMessager(), 
                new StubUserSettings(), 
                dryRunValidation: true);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullTrackingProfile()
        {
            new GoogleAnalytics(
                new StubAppSettings(), 
                trackingProfile: null,
                new StubMessager(),
                new StubUserSettings(),
                dryRunValidation: true);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullMessager()
        {
            new GoogleAnalytics(
                new StubAppSettings(),
                new StubTrackingProfile(),
                messager: null,
                new StubUserSettings(),
                dryRunValidation: true);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullUserSettings()
        {
            new GoogleAnalytics(
                new StubAppSettings(),
                new StubTrackingProfile(),
                new StubMessager(),
                userSettings: null,
                dryRunValidation: true);
        }

        [TestMethod]
        public void ConstructDryRunTrue()
        {
            new GoogleAnalytics(
                new StubAppSettings(),
                new StubTrackingProfile(),
                new StubMessager(),
                new StubUserSettings(),
                dryRunValidation: true);
        }

        [TestMethod]
        public void ConstructDryRunFalse()
        {
            new GoogleAnalytics(
                new StubAppSettings(),
                new StubTrackingProfile(),
                new StubMessager(),
                new StubUserSettings(),
                dryRunValidation: false);
        }
    }
}
