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

using Newtonsoft.Json;
using Services.Message;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NpcGenerator
{
    public class GoogleAnalyticsMessageGenerator
    {
        public GoogleAnalyticsMessageGenerator(ITrackingProfile trackingProfile,
            IMessager messager,
            IUserSettings userSettings,
            Action<string> messageCallback)
        {
            m_trackingProfile = trackingProfile;
            m_messager = messager;
            m_userSettings = userSettings;
            m_messageCallback = messageCallback;

            //Each Google Analytics event must have a name 40 characters or less: 
            //https://developers.google.com/analytics/devguides/collection/protocol/ga4/sending-events?client_type=gtag#limitations
            m_messager.Subscribe<Services.Message.Login>(OnLogin);
            m_messager.Subscribe<Services.Message.PageView>(OnPageView);
            m_messager.Subscribe<Message.SelectConfiguration>(OnSelectConfiguration);
            m_messager.Subscribe<Message.GenerateNpcs>(OnGenerateNpcs);
            m_messager.Subscribe<Message.SaveNpcs>(OnSaveNpcs);
            m_messager.Subscribe<Services.Message.UserLanguageNotSupported>(OnUserLanguageNotSupported);
            m_messager.Subscribe<Services.Message.LanguageSelected>(OnLanguageSelected);
        }

        ~GoogleAnalyticsMessageGenerator()
        {
            m_messager.Unsubscribe<Services.Message.Login>(OnLogin);
            m_messager.Unsubscribe<Services.Message.PageView>(OnPageView);
            m_messager.Unsubscribe<Message.SelectConfiguration>(OnSelectConfiguration);
            m_messager.Unsubscribe<Message.GenerateNpcs>(OnGenerateNpcs);
            m_messager.Unsubscribe<Message.SaveNpcs>(OnSaveNpcs);
            m_messager.Unsubscribe<Services.Message.UserLanguageNotSupported>(OnUserLanguageNotSupported);
            m_messager.Unsubscribe<Services.Message.LanguageSelected>(OnLanguageSelected);
        }

        private void WriteMessageBody(WriteGoogleEvent googleEvent)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("client_id");
                writer.WriteValue(m_trackingProfile.ClientId.ToString());

                WriteUserProperties(writer);

                //Writing the event should be through a callback.
                writer.WritePropertyName("events");
                writer.WriteStartArray();

                googleEvent(writer);

                writer.WriteEnd(); //End of array
                writer.WriteEndObject(); //End of json

            }
            string body = sw.ToString();
            m_messageCallback(body);
        }

        private static void WriteUserProperty(JsonWriter writer, string name, string value)
        {
            if (value != null)
            {
                Trace.Assert(name.Length <= 24, "User property name " + name + " is longer than the 24 character maximum");
                Trace.Assert(value.Length <= 36,
                    "User property value " + value + " is longer than the 36 character maximum");

                writer.WritePropertyName(name);

                writer.WriteStartObject();
                writer.WritePropertyName("value");
                writer.WriteValue(value);
                writer.WriteEnd();
            }
        }

        private void WriteUserProperties(JsonWriter writer)
        {
            writer.WritePropertyName("user_properties");
            writer.WriteStartObject();

            WriteUserProperty(writer, "system_language", m_trackingProfile.SystemLanguage);
            WriteUserProperty(writer, "app_language", m_userSettings.LanguageCode);
            WriteUserProperty(writer, "app_version", m_trackingProfile.AppVersion);
            WriteUserProperty(writer, "os_version", m_trackingProfile.OSVersion);

            writer.WriteEnd(); //End of user_properties object
        }

        private void OnLogin(object sender, Services.Message.Login login)
        {
            //Consent can change throughout the session
            if (m_userSettings.AnalyticsConsent)
            {
                WriteMessageBody(WriteLoginEvent);
            }
        }

        private void WriteLoginEvent(JsonWriter writer)
        {
            writer.WriteStartObject(); //Start of event object

            writer.WritePropertyName("name");
            writer.WriteValue("login");

            writer.WritePropertyName("params");
            writer.WriteStartObject();

            writer.WritePropertyName("method");
            writer.WriteValue("ClientApp");

            writer.WriteEnd(); //End of params object

            writer.WriteEnd(); //End of event object
        }

        private void OnPageView(object sender, Services.Message.PageView pageView)
        {
            if (m_userSettings.AnalyticsConsent)
            {
                WriteMessageBody(Callback);
            }

            void Callback(JsonWriter writer)
            {
                WritePageViewEvent(writer, pageView.Title);
            }
        }

        private static void WritePageViewEvent(JsonWriter writer, string title)
        {
            writer.WriteStartObject(); //Start of event object

            writer.WritePropertyName("name");
            writer.WriteValue("page_view");

            writer.WritePropertyName("params");
            writer.WriteStartObject();

            writer.WritePropertyName("title");
            writer.WriteValue(title);

            writer.WriteEnd(); //End of params object

            writer.WriteEnd(); //End of event object
        }

        private void OnSelectConfiguration(object sender, Message.SelectConfiguration selectConfiguration)
        {
            if (m_userSettings.AnalyticsConsent)
            {
                WriteMessageBody(WriteSelectConfigurationEvent);
            }
        }

        private void WriteSelectConfigurationEvent(JsonWriter writer)
        {
            writer.WriteStartObject(); //Start of event object

            writer.WritePropertyName("name");
            writer.WriteValue("select_configuration");

            writer.WriteEnd(); //End of event object
        }

        private void OnGenerateNpcs(object sender, Message.GenerateNpcs generateNpcs)
        {
            if (m_userSettings.AnalyticsConsent)
            {
                WriteMessageBody(Callback);
            }

            void Callback(JsonWriter writer)
            {
                WriteGenerateNpcsEvent(writer, generateNpcs.Quantity);
            }
        }

        private static void WriteGenerateNpcsEvent(JsonWriter writer, int quantity)
        {
            writer.WriteStartObject(); //Start of event object

            writer.WritePropertyName("name");
            writer.WriteValue("generate_npcs");

            writer.WritePropertyName("params");
            writer.WriteStartObject();

            writer.WritePropertyName("quantity");
            writer.WriteValue(quantity);

            writer.WriteEnd(); //End of params object

            writer.WriteEnd(); //End of event object
        }

        private void OnSaveNpcs(object sender, Message.SaveNpcs saveNpcs)
        {
            if (m_userSettings.AnalyticsConsent)
            {
                WriteMessageBody(Callback);
            }

            void Callback(JsonWriter writer)
            {
                WriteSaveNpcsEvent(writer, saveNpcs.Format);
            }
        }

        private void WriteSaveNpcsEvent(JsonWriter writer, string format)
        {
            writer.WriteStartObject(); //Start of event object

            writer.WritePropertyName("name");
            writer.WriteValue("save_npcs");

            writer.WritePropertyName("params");
            writer.WriteStartObject();

            writer.WritePropertyName("format");
            writer.WriteValue(format);

            writer.WriteEnd(); //End of params object

            writer.WriteEnd(); //End of event object
        }

        private void OnUserLanguageNotSupported(object sender, Services.Message.UserLanguageNotSupported notSupported)
        {
            if (m_userSettings.AnalyticsConsent)
            {
                WriteMessageBody(Callback);
            }

            void Callback(JsonWriter writer)
            {
                WriteUserLanguageNotSupportedEvent(writer, notSupported.LanguageCode);
            }
        }

        private static void WriteUserLanguageNotSupportedEvent(JsonWriter writer, string languageCode)
        {
            writer.WriteStartObject(); //Start of event object

            writer.WritePropertyName("name");
            writer.WriteValue("unsupported_language");

            writer.WritePropertyName("params");
            writer.WriteStartObject();

            writer.WritePropertyName("language_code");
            writer.WriteValue(languageCode);

            writer.WriteEnd(); //End of params object

            writer.WriteEnd(); //End of event object
        }

        private void OnLanguageSelected(object sender, Services.Message.LanguageSelected languageSelected)
        {
            if (m_userSettings.AnalyticsConsent)
            {
                WriteMessageBody(Callback);
            }

            void Callback(JsonWriter writer)
            {
                WriteLanguageSelectedEvent(writer, languageSelected.Language);
            }
        }

        private static void WriteLanguageSelectedEvent(JsonWriter writer, string languageCode)
        {
            writer.WriteStartObject(); //Start of event object

            writer.WritePropertyName("name");
            writer.WriteValue("language_selected");

            writer.WritePropertyName("params");
            writer.WriteStartObject();

            writer.WritePropertyName("language_code");
            writer.WriteValue(languageCode);

            writer.WriteEnd(); //End of params object

            writer.WriteEnd(); //End of event object
        }

        private delegate void WriteGoogleEvent(JsonWriter writer);

        private readonly ITrackingProfile m_trackingProfile;
        private readonly IMessager m_messager;
        private readonly IUserSettings m_userSettings;
        private readonly Action<string> m_messageCallback;
    }
}
