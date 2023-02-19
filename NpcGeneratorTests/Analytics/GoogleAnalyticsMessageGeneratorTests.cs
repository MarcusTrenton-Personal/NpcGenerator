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
using Newtonsoft.Json;
using NpcGenerator;
using NpcGenerator.Message;
using Services.Message;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class GoogleAnalyticsMessageGeneratorTests
    {
        //Easiest way to verify values is to deserialize the json body into a strongly-typed object.
        //So the names are for json, which the IDE dislikes or are C# keywords (chosen by Google).
#pragma warning disable IDE1006 // Naming rule violation
        public class JsonBody
        {
            public Guid client_id { get; set; }
            public UserProperties user_properties { get; set; }
            public IList<Event> events { get; set; }
        }

        public class UserProperties
        {
            public Property system_language { get; set; }
            public Property app_version { get; set; }
            public Property app_language { get; set; }
            public Property os_version { get; set; }
        }

        public class Property
        {
            public string value { get; set; }
        }

        public class Event
        {
            public string name;
            public Dictionary<string, string> @params { get; set; }
        }
#pragma warning restore IDE1006 // Naming rule violation 

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullTrackingProfile()
        {
            GoogleAnalyticsMessageGenerator generator = new GoogleAnalyticsMessageGenerator(
                trackingProfile: null,
                new StubMessager(),
                new StubUserSettings(),
                Callback);

            static void Callback(string messageBody)
            {
            }
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullMessager()
        {
            GoogleAnalyticsMessageGenerator generator = new GoogleAnalyticsMessageGenerator(
                new StubTrackingProfile(),
                messager: null,
                new StubUserSettings(),
                Callback);

            static void Callback(string messageBody)
            {
            }
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullUserSettings()
        {
            GoogleAnalyticsMessageGenerator generator = new GoogleAnalyticsMessageGenerator(
                new StubTrackingProfile(),
                new StubMessager(),
                userSettings: null,
                Callback);

            static void Callback(string messageBody)
            {
            }
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullMessageCallback()
        {
            new GoogleAnalyticsMessageGenerator(
                new StubTrackingProfile(),
                new StubMessager(),
                new StubUserSettings(),
                messageCallback: null);
        }

        [TestMethod]
        public void EmptyUserSettings()
        {
            bool callbackCalled = false;
            Messager messager = MakeMessagerUsedInGoogleAnalytics(Callback);

            messager.Send(this, new Login());

            void Callback(string messageBody)
            {
                callbackCalled = true;
            }

            Assert.IsTrue(callbackCalled, "GoogleAnalyticsMessageGenerator callback not called");
        }

        [TestMethod]
        public void TrackConsentObeyed()
        {
            bool callbackCalled = false;
            Messager messager = new Messager();
            GoogleAnalyticsMessageGenerator generator = new GoogleAnalyticsMessageGenerator(
                TrackingProfile.Load("fakePath"),
                messager,
                new StubUserSettings() { AnalyticsConsent = false },
                Callback);

            messager.Send(this, new Login());

            void Callback(string messageBody)
            {
                callbackCalled = true;
            }

            Assert.IsFalse(callbackCalled, "AnalyticsConsent violated");
        }

        [TestMethod]
        public void NullUserPropertiesNotWritten()
        {
            bool callbackCalled = false;
            Messager messager = MakeMessagerUsedInGoogleAnalytics(Callback);

            messager.Send(this, new Login());

            void Callback(string messageBody)
            {
                callbackCalled = true;

                JsonBody body = JsonConvert.DeserializeObject<JsonBody>(messageBody);
                Assert.IsNull(body.user_properties.system_language, "Null user property system_language not written as null");
                Assert.IsNull(body.user_properties.app_language, "Null user property app_language not written as null");
                Assert.IsNull(body.user_properties.app_version, "Null user property app_version not written as null");
                Assert.IsNull(body.user_properties.os_version, "Null user property os_version not written as null");
            }

            Assert.IsTrue(callbackCalled, "GoogleAnalyticsMessageGenerator callback not called");  
        }

        [TestMethod]
        public void AllUserPropertiesWritten()
        {
            bool callbackCalled = false;

            const string LANGUAGE_CODE = "martian";
            const string APP_VERSION = "1.0";
            Guid clientId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
            const string OS_VERSION = "Windows 9";
            const string SYSTEM_LANGUAGE = "atlantean";

            Messager messager = new Messager();
            StubUserSettings userSettings = new StubUserSettings()
            {
                AnalyticsConsent = true,
                LanguageCode = LANGUAGE_CODE
            };
            StubTrackingProfile trackingProfile = new StubTrackingProfile()
            {
                AppVersion = APP_VERSION,
                ClientId = clientId,
                OsVersion = OS_VERSION,
                SystemLanguage = SYSTEM_LANGUAGE
            };
            GoogleAnalyticsMessageGenerator generator = new GoogleAnalyticsMessageGenerator(
                trackingProfile, messager, userSettings, Callback);

            messager.Send(this, new Login());

            void Callback(string messageBody)
            {
                callbackCalled = true;

                JsonBody body = JsonConvert.DeserializeObject<JsonBody>(messageBody);
                Assert.AreEqual(clientId, body.client_id, "User Guid not serialized correct");
                Assert.AreEqual(LANGUAGE_CODE, body.user_properties.app_language.value, 
                    "User Language not serialized correct");
                Assert.AreEqual(APP_VERSION, body.user_properties.app_version.value, 
                    "App Version not serialized correct");
                Assert.AreEqual(OS_VERSION, body.user_properties.os_version.value, 
                    "User OS not serialized correct");
                Assert.AreEqual(SYSTEM_LANGUAGE, body.user_properties.system_language.value, 
                    "System Language not serialized correct");
            }

            Assert.IsTrue(callbackCalled, "GoogleAnalyticsMessageGenerator callback not called");
        }

        [TestMethod]
        public void Login()
        {
            bool callbackCalled = false;
            Messager messager = MakeMessagerUsedInGoogleAnalytics(Callback);

            messager.Send(this, new Login());

            void Callback(string messageBody)
            {
                callbackCalled = true;

                JsonBody body = JsonConvert.DeserializeObject<JsonBody>(messageBody);
                Assert.AreEqual(1, body.events.Count, "Wrong number of events serialized");
                Assert.AreEqual("login", body.events[0].name, "Wrong event name");
                Assert.AreEqual(1, body.events[0].@params.Count, "Wrong number of params");
                Assert.AreEqual("ClientApp", body.events[0].@params["method"], "Wrong parameter");
            }

            Assert.IsTrue(callbackCalled, "GoogleAnalyticsMessageGenerator callback not called");
        }

        [TestMethod]
        public void PageView()
        {
            bool callbackCalled = false;
            Messager messager = MakeMessagerUsedInGoogleAnalytics(Callback);

            const string TITLE = "EasterEgg";
            messager.Send(this, new PageView(TITLE));

            void Callback(string messageBody)
            {
                callbackCalled = true;

                JsonBody body = JsonConvert.DeserializeObject<JsonBody>(messageBody);
                Assert.AreEqual(1, body.events.Count, "Wrong number of events serialized");
                Assert.AreEqual("page_view", body.events[0].name, "Wrong event name");
                Assert.AreEqual(1, body.events[0].@params.Count, "Wrong number of params");
                Assert.AreEqual(TITLE, body.events[0].@params["title"], "Wrong parameter");
            }

            Assert.IsTrue(callbackCalled, "GoogleAnalyticsMessageGenerator callback not called");
        }

        [TestMethod]
        public void LanguageSelected()
        {
            bool callbackCalled = false;
            Messager messager = MakeMessagerUsedInGoogleAnalytics(Callback);

            const string LANGUAGE_CODE = "en-ma"; //Language code is regex-checked, so make it valid.
            messager.Send(this, new LanguageSelected(LANGUAGE_CODE));

            void Callback(string messageBody)
            {
                callbackCalled = true;

                JsonBody body = JsonConvert.DeserializeObject<JsonBody>(messageBody);
                Assert.AreEqual(1, body.events.Count, "Wrong number of events serialized");
                Assert.AreEqual("language_selected", body.events[0].name, "Wrong event name");
                Assert.AreEqual(1, body.events[0].@params.Count, "Wrong number of params");
                Assert.AreEqual(LANGUAGE_CODE, body.events[0].@params["language_code"], "Wrong parameter");
            }

            Assert.IsTrue(callbackCalled, "GoogleAnalyticsMessageGenerator callback not called");
        }

        [TestMethod]
        public void UserLanguageNotSupported()
        {
            bool callbackCalled = false;
            Messager messager = MakeMessagerUsedInGoogleAnalytics(Callback);

            const string LANGUAGE_CODE = "Martian";
            messager.Send(this, new UserLanguageNotSupported(LANGUAGE_CODE));

            void Callback(string messageBody)
            {
                callbackCalled = true;

                JsonBody body = JsonConvert.DeserializeObject<JsonBody>(messageBody);
                Assert.AreEqual(1, body.events.Count, "Wrong number of events serialized");
                Assert.AreEqual("unsupported_language", body.events[0].name, "Wrong event name");
                Assert.AreEqual(1, body.events[0].@params.Count, "Wrong number of params");
                Assert.AreEqual(LANGUAGE_CODE, body.events[0].@params["language_code"], "Wrong parameter");
            }

            Assert.IsTrue(callbackCalled, "GoogleAnalyticsMessageGenerator callback not called");
        }

        [DataTestMethod]
        [DataRow(TraitSchema.Features.None)]
        [DataRow(TraitSchema.Features.Weight)]
        [DataRow(TraitSchema.Features.MultipleSelection)]
        [DataRow(TraitSchema.Features.BonusSelection)]
        [DataRow(TraitSchema.Features.HiddenTrait)]
        [DataRow(TraitSchema.Features.HiddenCategory)]
        [DataRow(TraitSchema.Features.OutputCategoryName)]
        [DataRow(TraitSchema.Features.CategoryOrder)]
        [DataRow(TraitSchema.Features.Replacement)]
        [DataRow(TraitSchema.Features.CategoryRequirement)]
        [DataRow(TraitSchema.Features.TraitRequirement)]
        [DataRow(TraitSchema.Features.HiddenTrait | TraitSchema.Features.HiddenCategory)]
        [DataRow(TraitSchema.Features.Weight |
            TraitSchema.Features.MultipleSelection |
            TraitSchema.Features.BonusSelection |
            TraitSchema.Features.HiddenTrait |
            TraitSchema.Features.HiddenCategory |
            TraitSchema.Features.OutputCategoryName |
            TraitSchema.Features.CategoryOrder |
            TraitSchema.Features.Replacement |
            TraitSchema.Features.CategoryRequirement |
            TraitSchema.Features.TraitRequirement)]
        public void GenerateNpcs(TraitSchema.Features schemaFeaturesUsed)
        {
            bool callbackCalled = false;
            Messager messager = MakeMessagerUsedInGoogleAnalytics(Callback);

            const int QUANTITY = 10;
            messager.Send(this, new GenerateNpcs(QUANTITY, schemaFeaturesUsed));

            void Callback(string messageBody)
            {
                callbackCalled = true;

                JsonBody body = JsonConvert.DeserializeObject<JsonBody>(messageBody);
                Assert.AreEqual(1, body.events.Count, "Wrong number of events serialized");
                Assert.AreEqual("generate_npcs", body.events[0].name, "Wrong event name");
                int expectedParams = 1 + Enum.GetValues(typeof(TraitSchema.Features)).Length;
                Assert.AreEqual(expectedParams, body.events[0].@params.Count, "Wrong number of params");
                Assert.AreEqual(QUANTITY.ToString(), body.events[0].@params["quantity"], "Wrong parameter");

                foreach (int featureValue in Enum.GetValues(typeof(TraitSchema.Features)))
                {
                    bool schemaHasFeature = (featureValue & (int)schemaFeaturesUsed) > 0;
                    string featureName = Enum.GetName(typeof(TraitSchema.Features), featureValue);

                    string jsonFeatureString = body.events[0].@params[featureName];
                    string expectedJsonFeatureString = schemaHasFeature ? true.ToString().ToLower() : false.ToString().ToLower();
                    Assert.AreEqual(expectedJsonFeatureString, jsonFeatureString, "Wrong schema feature value");
                }
            }

            Assert.IsTrue(callbackCalled, "GoogleAnalyticsMessageGenerator callback not called");
        }

        [TestMethod]
        public void SaveNpcs()
        {
            bool callbackCalled = false;
            Messager messager = MakeMessagerUsedInGoogleAnalytics(Callback);

            const string FORMAT = "MorseCode";
            messager.Send(this, new SaveNpcs(FORMAT));

            void Callback(string messageBody)
            {
                callbackCalled = true;

                JsonBody body = JsonConvert.DeserializeObject<JsonBody>(messageBody);
                Assert.AreEqual(1, body.events.Count, "Wrong number of events serialized");
                Assert.AreEqual("save_npcs", body.events[0].name, "Wrong event name");
                Assert.AreEqual(1, body.events[0].@params.Count, "Wrong number of params");
                Assert.AreEqual(FORMAT, body.events[0].@params["format"], "Wrong parameter");
            }

            Assert.IsTrue(callbackCalled, "GoogleAnalyticsMessageGenerator callback not called");
        }

        [TestMethod]
        public void SelectConfiguration()
        {
            bool callbackCalled = false;
            Messager messager = MakeMessagerUsedInGoogleAnalytics(Callback);

            messager.Send(this, new SelectConfiguration());

            void Callback(string messageBody)
            {
                callbackCalled = true;

                JsonBody body = JsonConvert.DeserializeObject<JsonBody>(messageBody);
                Assert.AreEqual(1, body.events.Count, "Wrong number of events serialized");
                Assert.AreEqual("select_configuration", body.events[0].name, "Wrong event name");
                Assert.IsNull(body.events[0].@params, "There should not be any params");
            }

            Assert.IsTrue(callbackCalled, "GoogleAnalyticsMessageGenerator callback not called");
        }

        [TestMethod]
        public void Crash()
        {
            bool callbackCalled = false;
            Messager messager = MakeMessagerUsedInGoogleAnalytics(Callback);

            const string MESSAGE = "Test failure";
            Exception exception = new Exception(MESSAGE);
            messager.Send(this, new Crash(exception));

            void Callback(string messageBody)
            {
                callbackCalled = true;

                JsonBody body = JsonConvert.DeserializeObject<JsonBody>(messageBody);
                Assert.AreEqual(1, body.events.Count, "Wrong number of events serialized");
                Assert.AreEqual("crash", body.events[0].name, "Wrong event name");
                Assert.AreEqual(1, body.events[0].@params.Count, "Wrong number of params");
                Assert.AreEqual("System.Exception: " + MESSAGE, body.events[0].@params["message"], "Wrong parameter");
            }

            Assert.IsTrue(callbackCalled, "GoogleAnalyticsMessageGenerator callback not called");
        }

        private Messager MakeMessagerUsedInGoogleAnalytics(Action<string> callback)
        {
            Messager messager = new Messager();
            GoogleAnalyticsMessageGenerator generator = new GoogleAnalyticsMessageGenerator(
                new StubTrackingProfile(),
                messager,
                new StubUserSettings(),
                callback);
            return messager;
        }
    }
}
