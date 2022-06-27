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

using Services;
using Services.Message;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Diagnostics;

namespace NpcGenerator
{
    public class GoogleAnalytics
    {
#pragma warning disable CS0649 //Field is never assigned.  In this case, it's written by JSON conversion.
        private class ValidationContent
        {
            public string fieldPath;
            public string description;
            public string validationCode;
        }

        private class ValidationResponse
        {
            public ValidationContent[] validationMessages;
        }
#pragma warning disable CS0649 //Field is never assigned. 

        public GoogleAnalytics(IAppSettings appSettings, 
            ITrackingProfile trackingProfile, 
            IMessager messager, 
            IUserSettings userSettings, 
            bool dryRunValidation)
        {
            m_appSettings = appSettings;
            m_trackingProfile = trackingProfile;
            m_messager = messager;
            m_userSettings = userSettings;
            m_dryRunValidation = dryRunValidation;

            //Each Google Analytics event must have a name 40 characters or less: 
            //https://developers.google.com/analytics/devguides/collection/protocol/ga4/sending-events?client_type=gtag#limitations
            m_messager.Subscribe<Message.Login>(OnLogin);
            m_messager.Subscribe<Message.PageView>(OnPageView);
            m_messager.Subscribe<Message.SelectConfiguration>(OnSelectConfiguration);
            m_messager.Subscribe<Message.GenerateNpcs>(OnGenerateNpcs);
            m_messager.Subscribe<Message.SaveNpcs>(OnSaveNpcs);
            m_messager.Subscribe<Message.UserLanguageNotSupported>(OnUserLanguageNotSupported);
            m_messager.Subscribe<Message.LanguageSelected>(OnLanguageSelected);
        }

        ~GoogleAnalytics()
        {
            m_messager.Unsubscribe<Message.Login>(OnLogin);
            m_messager.Unsubscribe<Message.PageView>(OnPageView);
            m_messager.Unsubscribe<Message.SelectConfiguration>(OnSelectConfiguration);
            m_messager.Unsubscribe<Message.GenerateNpcs>(OnGenerateNpcs);
            m_messager.Unsubscribe<Message.SaveNpcs>(OnSaveNpcs);
            m_messager.Unsubscribe<Message.UserLanguageNotSupported>(OnUserLanguageNotSupported);
            m_messager.Unsubscribe<Message.LanguageSelected>(OnLanguageSelected);
        }

        private async void TrackEvent(WriteGoogleEvent googleEvent)
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

#if DEBUG
            string measurementId = m_appSettings.GoogleAnalytics.MeasurementIdDev;
            string additionalId = m_appSettings.GoogleAnalytics.AdditionalIdDev;
#else
            string measurementId = m_appSettings.GoogleAnalytics.MeasurementIdProd;
            string additionalId = m_appSettings.GoogleAnalytics.AdditionalIdProd;
#endif
            string url = m_dryRunValidation ?   "https://www.google-analytics.com/debug/mp/collect?api_secret={0}&measurement_id={1}" :
                                                "https://www.google-analytics.com/mp/collect?api_secret={0}&measurement_id={1}";

            /*
             * Don't laugh. Doing security on a public source code, client-only app distributed to the public is not easy. 
             * There are no good solutions. This is why anything needing serious security is server authoritative.
             * Basic techniques have been thwarted. No obvious variable names. The plain text ID is not in source control.
             * There is no line to set a break point on to view the precious data. 
             * Text scrapers and casual inspection are insufficient to break the protection.
             * Either editing the source code or advanced sniffers are needed to break this.
             */
            using StringContent content = new StringContent(body, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await s_client.PostAsync(
                new Uri(string.Format(url, Encryption.XorEncryptDecrypt(additionalId, m_appSettings.EncryptionKey), measurementId)),
                content
            ).ConfigureAwait(false);
            if(!response.IsSuccessStatusCode)
            {
                Console.Error.WriteLine("Failed analytics response: " + response.StatusCode);
            }
            else if (m_dryRunValidation)
            {
                string resopnseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                ValidationResponse validationResponse = JsonConvert.DeserializeObject<ValidationResponse>(resopnseText);
                bool haveErrorInfo = validationResponse != null && 
                    validationResponse.validationMessages != null && 
                    validationResponse.validationMessages.Length > 0;
                if (haveErrorInfo)
                {
                    ValidationContent validation = validationResponse.validationMessages[0];
                    //Don't localize dev messages.
                    MessageBox.Show("Analytics failed validation.\n" + validation.fieldPath + "\n" +
                        validation.description + "\n" + validation.validationCode + "\n\nOriginal Message:\n" +
                        body);
                }
            }
        }

        private static void WriteUserProperty(JsonWriter writer, string name, string value)
        {
            Debug.Assert(name.Length <= 24, "User property name " + name + " is longer than the 24 character maximum");
            Debug.Assert(value.Length <= 36, "User property value " + value + " is longer than the 36 character maximum");

            writer.WritePropertyName(name);
            
            writer.WriteStartObject();
            writer.WritePropertyName("value");
            writer.WriteValue(value);
            writer.WriteEnd();
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

        private void OnLogin(object sender, Message.Login login)
        {
            //Consent can change throughout the session
            if(m_userSettings.AnalyticsConsent)
            {
                TrackEvent(WriteLoginEvent);
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

        private void OnPageView(object sender, Message.PageView pageView)
        {
            if (m_userSettings.AnalyticsConsent)
            {
                TrackEvent(Callback);
            }

            void Callback(JsonWriter writer)
            {
                WritePageViewEvent(writer, pageView.Title);
            }
        }

        private void WritePageViewEvent(JsonWriter writer, string title)
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
                TrackEvent(WriteSelectConfigurationEvent);
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
                TrackEvent(Callback);
            }

            void Callback(JsonWriter writer)
            {
                WriteGenerateNpcsEvent(writer, generateNpcs.Quantity);
            }
        }

        private void WriteGenerateNpcsEvent(JsonWriter writer, int quantity)
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
                TrackEvent(WriteSaveNpcsEvent);
            }     
        }

        private void WriteSaveNpcsEvent(JsonWriter writer)
        {
            writer.WriteStartObject(); //Start of event object

            writer.WritePropertyName("name");
            writer.WriteValue("save_npcs");

            writer.WriteEnd(); //End of event object
        }

        private void OnUserLanguageNotSupported(object sender, Message.UserLanguageNotSupported notSupported)
        {
            if (m_userSettings.AnalyticsConsent)
            {
                TrackEvent(Callback);
            }

            void Callback(JsonWriter writer)
            {
                WriteUserLanguageNotSupportedEvent(writer, notSupported.LanguageCode);
            }
        }

        private void WriteUserLanguageNotSupportedEvent(JsonWriter writer, string languageCode)
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

        private void OnLanguageSelected(object sender, Message.LanguageSelected languageSelected)
        {
            if (m_userSettings.AnalyticsConsent)
            {
                TrackEvent(Callback);
            }

            void Callback(JsonWriter writer)
            {
                WriteLanguageSelectedEvent(writer, languageSelected.Language);
            }
        }

        private void WriteLanguageSelectedEvent(JsonWriter writer, string languageCode)
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

        private static readonly HttpClient s_client = new HttpClient();
        private readonly IAppSettings m_appSettings;
        private readonly ITrackingProfile m_trackingProfile;
        private readonly IMessager m_messager;
        private readonly IUserSettings m_userSettings;
        private readonly bool m_dryRunValidation;
    }
}