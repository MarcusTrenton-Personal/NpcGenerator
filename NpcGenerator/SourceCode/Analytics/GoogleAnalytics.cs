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

using NpcGenerator.Message;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace NpcGenerator
{
    public class GoogleAnalytics
    {
        public GoogleAnalytics(IAppSettings appSettings, ITrackingProfile trackingProfile, IMessager messager)
        {
            m_appSettings = appSettings;
            m_trackingProfile = trackingProfile;
            m_messager = messager;

            m_messager.Subscribe<Login>(OnLogin);
            m_messager.Subscribe<PageView>(OnPageView);
            m_messager.Subscribe<SelectConfiguration>(OnSelectConfiguration);
            m_messager.Subscribe<GenerateNpcs>(OnGenerateNpcs);
            m_messager.Subscribe<SaveNpcs>(OnSaveNpcs);
        }

        ~GoogleAnalytics()
        {
            m_messager.Unsubcribe<Login>(OnLogin);
            m_messager.Unsubcribe<PageView>(OnPageView);
            m_messager.Unsubcribe<SelectConfiguration>(OnSelectConfiguration);
            m_messager.Unsubcribe<GenerateNpcs>(OnGenerateNpcs);
            m_messager.Unsubcribe<SaveNpcs>(OnSaveNpcs);
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

            /*
             * Don't laugh. Doing security on a public source code, client-only app distributed to the public is not easy. 
             * There are no good solutions. This is why anything needing serious security is server authoritative.
             * Basic techniques have been thwarted. No obvious variable names. The plain text ID is not in source control.
             * There is no line to set a break point on to view the precious data. 
             * Text scrapers and casual inspection are insufficient to break the protection.
             * Either editing the source code or advanced sniffers are needed to break this.
             */
            using (StringContent content = new StringContent(body))
            {
                //TODO: Catch exceptions. This can fail silently.
                HttpResponseMessage response = await s_client.PostAsync(
                    new Uri(
                        string.Format(
                            "https://www.google-analytics.com/mp/collect?api_secret={0}&measurement_id={1}",
                            Encryption.XorEncryptDecrypt(additionalId, m_appSettings.EncryptionKey),
                            measurementId)
                        ),
                    content);
                if(!response.IsSuccessStatusCode)
                {
                    //TODO: Write to warning log.
                    Console.Error.WriteLine("Failed analytics response: " + response.StatusCode);
                }
            }
        }

        private void WriteUserProperties(JsonWriter writer)
        {
            writer.WritePropertyName("user_properties");
            writer.WriteStartObject();
            
            writer.WritePropertyName("language");
            writer.WriteValue(m_trackingProfile.Language);

            writer.WritePropertyName("app_version");
            writer.WriteValue(m_trackingProfile.AppVersion);

            writer.WriteEnd(); //End of user_properties object
        }

        private void OnLogin(object sender, Message.Login login)
        {
            TrackEvent(WriteLoginEvent);
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

        private void OnPageView(object sender, PageView pageView)
        {
            WriteGoogleEvent googlePageViewEvent = delegate (JsonWriter writer) { WritePageViewEvent(writer, pageView.Title); };
            TrackEvent(googlePageViewEvent);
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

        private void OnSelectConfiguration(object sender, SelectConfiguration selectConfiguration)
        {
            TrackEvent(WriteSelectConfigurationEvent);
        }

        private void WriteSelectConfigurationEvent(JsonWriter writer)
        {
            writer.WriteStartObject(); //Start of event object

            writer.WritePropertyName("name");
            writer.WriteValue("select_configuration");

            writer.WriteEnd(); //End of event object
        }

        private void OnGenerateNpcs(object sender, GenerateNpcs generateNpcs)
        {
            WriteGoogleEvent googleGenerateNpcsEvent = delegate (JsonWriter writer) { WriteGenerateNpcsEvent(writer, generateNpcs.Quantity); };
            TrackEvent(googleGenerateNpcsEvent);
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

        private void OnSaveNpcs(object sender, SaveNpcs saveNpcs)
        {
            TrackEvent(WriteSaveNpcsEvent);
        }

        private void WriteSaveNpcsEvent(JsonWriter writer)
        {
            writer.WriteStartObject(); //Start of event object

            writer.WritePropertyName("name");
            writer.WriteValue("save_npcs");

            writer.WriteEnd(); //End of event object
        }

        private delegate void WriteGoogleEvent(JsonWriter writer);

        private static readonly HttpClient s_client = new HttpClient();
        private readonly IAppSettings m_appSettings;
        private readonly ITrackingProfile m_trackingProfile;
        private readonly IMessager m_messager;
    }
}