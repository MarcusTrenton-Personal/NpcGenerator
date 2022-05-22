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

using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace NpcGenerator
{
    public class GoogleAnalytics
    {
        public GoogleAnalytics(IAppSettings appSettings, ITrackingProfile trackingProfile, IMessageCenter messageCenter)
        {
            m_appSettings = appSettings;
            m_trackingProfile = trackingProfile;
            m_messageCenter = messageCenter;

            m_messageCenter.Subscribe<Message.Login>(OnLogin);           

            //TODO: subscribe to page_view and select_content
        }

        ~GoogleAnalytics()
        {
            m_messageCenter.Unsubcribe<Message.Login>(OnLogin);
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

        private delegate void WriteGoogleEvent(JsonWriter writer);

        private static readonly HttpClient s_client = new HttpClient();
        private IAppSettings m_appSettings;
        private ITrackingProfile m_trackingProfile;
        private IMessageCenter m_messageCenter;
    }
}