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
using Services;
using WpfServices;

namespace Tests
{
    [TestClass]
    public class TrackingModelTests
    {
        private class MockAnalyticsConsent : IAnalyticsConsent
        {
            public bool AnalyticsConsent { get; set; }
        }

        [TestMethod]
        public void UserSettingsReflectChanges()
        {
            IAnalyticsConsent userSettings = new MockAnalyticsConsent
            {
                AnalyticsConsent = false
            };
            TrackingModel trackingModel = new TrackingModel(userSettings);
            Assert.IsFalse(trackingModel.TrackingConsent, "UserSettings doesn't match TrackingModel.TrackingConsent");

            trackingModel.TrackingConsent = true;
            Assert.IsTrue(trackingModel.TrackingConsent, "UserSettings doesn't match TrackingModel.TrackingConsent");
        }
    }
}
