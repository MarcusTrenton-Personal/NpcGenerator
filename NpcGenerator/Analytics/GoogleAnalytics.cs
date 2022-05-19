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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

public class GoogleAnalytics
{
    public GoogleAnalytics(AppSettings appSettings)
    {
        m_appSettings = appSettings;
        //int key = -485227;
        //string scrambled = Encryption.XorEncryptDecrypt(secret, key);
        //string original = Encryption.XorEncryptDecrypt(scrambled, key);
        //Console.WriteLine(original);

        //string scrambled2 = Encryption.XorEncryptDecrypt("zFQhZQx6QwWDg0rBv4yf-Q", key);
        //string original2 = Encryption.XorEncryptDecrypt(scrambled2, key);
        //Console.WriteLine(original2);

        //TODO: Subscribe to events.

        //TODO call event via subscription instead of directly.
        TrackEvent();
    }

    ~GoogleAnalytics()
    {
        //TODO: unsubscribe from events
    }

    private async void TrackEvent()
    {
        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.WriteStartObject();
                writer.WritePropertyName("client_id");
                writer.WriteValue("Postman Test");
                writer.WritePropertyName("events");
                writer.WriteStartArray();
                    writer.WriteStartObject();
                        writer.WritePropertyName("name");
                        writer.WriteValue("login");
                        writer.WritePropertyName("params");
                        writer.WriteStartObject();
                            writer.WritePropertyName("method");
                            writer.WriteValue("ClientApp");
                        writer.WriteEnd();
                    writer.WriteEnd();
                writer.WriteEnd();
            writer.WriteEndObject();
            
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
         * Don't laugh. Doing security on a client-only app distributed to the public is not easy. There are no good solutions.
         * Basic techniques have been thwarted. No obvious variable names. The plain text is not in source control.
         * There is no line to set a break point on an view the precious data. 
         * Text scrapers and casual inspection are insufficient to break the protection.
         * Either editing the source code or advanced sniffers are needed to break this.
         */
        using(StringContent content = new StringContent(body))
        {
            HttpResponseMessage response = await client.PostAsync(
                new Uri(
                    string.Format(
                        "https://www.google-analytics.com/mp/collect?api_secret={0}&measurement_id={1}",
                        Encryption.XorEncryptDecrypt(additionalId, m_appSettings.EncryptionKey),
                        measurementId)
                    ), 
                content);
            Console.WriteLine("Analytics response: " + response.StatusCode);
        }
    }

    static readonly HttpClient client = new HttpClient();
    AppSettings m_appSettings;
}