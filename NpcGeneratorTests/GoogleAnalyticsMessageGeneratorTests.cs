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
along with this program. If not, see<https://www.gnu.org/licenses/>.*/

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

        [TestMethod]
        public void EmptyUserSettings()
        {
            bool callbackCalled = false;
            Messager messager = new Messager();
            GoogleAnalyticsMessageGenerator generator = new GoogleAnalyticsMessageGenerator(
                new TrackingProfile(),
                messager,
                new StubUserSettings(),
                Callback);

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
                new TrackingProfile(),
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
            Messager messager = new Messager();
            GoogleAnalyticsMessageGenerator generator = new GoogleAnalyticsMessageGenerator(
                new StubTrackingProfile(),
                messager,
                new StubUserSettings(),
                Callback);

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
                OSVersion = OS_VERSION,
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
            Messager messager = new Messager();
            GoogleAnalyticsMessageGenerator generator = new GoogleAnalyticsMessageGenerator(
                new StubTrackingProfile(),
                messager,
                new StubUserSettings(),
                Callback);

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
            Messager messager = new Messager();
            GoogleAnalyticsMessageGenerator generator = new GoogleAnalyticsMessageGenerator(
                new StubTrackingProfile(),
                messager,
                new StubUserSettings(),
                Callback);

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
            Messager messager = new Messager();
            GoogleAnalyticsMessageGenerator generator = new GoogleAnalyticsMessageGenerator(
                new StubTrackingProfile(),
                messager,
                new StubUserSettings(),
                Callback);

            const string LANGUAGE_CODE = "Martian";
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
            Messager messager = new Messager();
            GoogleAnalyticsMessageGenerator generator = new GoogleAnalyticsMessageGenerator(
                new StubTrackingProfile(),
                messager,
                new StubUserSettings(),
                Callback);

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

        [TestMethod]
        public void GenerateNpcs()
        {
            bool callbackCalled = false;
            Messager messager = new Messager();
            GoogleAnalyticsMessageGenerator generator = new GoogleAnalyticsMessageGenerator(
                new StubTrackingProfile(),
                messager,
                new StubUserSettings(),
                Callback);

            const int QUANTITY = 10;
            messager.Send(this, new GenerateNpcs(QUANTITY));

            void Callback(string messageBody)
            {
                callbackCalled = true;

                JsonBody body = JsonConvert.DeserializeObject<JsonBody>(messageBody);
                Assert.AreEqual(1, body.events.Count, "Wrong number of events serialized");
                Assert.AreEqual("generate_npcs", body.events[0].name, "Wrong event name");
                Assert.AreEqual(1, body.events[0].@params.Count, "Wrong number of params");
                Assert.AreEqual(QUANTITY.ToString(), body.events[0].@params["quantity"], "Wrong parameter");
            }

            Assert.IsTrue(callbackCalled, "GoogleAnalyticsMessageGenerator callback not called");
        }

        [TestMethod]
        public void SaveNpcs()
        {
            bool callbackCalled = false;
            Messager messager = new Messager();
            GoogleAnalyticsMessageGenerator generator = new GoogleAnalyticsMessageGenerator(
                new StubTrackingProfile(),
                messager,
                new StubUserSettings(),
                Callback);

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
            Messager messager = new Messager();
            GoogleAnalyticsMessageGenerator generator = new GoogleAnalyticsMessageGenerator(
                new StubTrackingProfile(),
                messager,
                new StubUserSettings(),
                Callback);

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
    }
}
