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

using Newtonsoft.Json;
using Services;
using Services.Message;
using System;
using System.Net.Http;
using System.Text;
using System.Windows;

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
            m_dryRunValidation = dryRunValidation;

            m_messageGenerator = new GoogleAnalyticsMessageGenerator(trackingProfile, messager, userSettings, TrackEvent);
        }

        private async void TrackEvent(string messageBody)
        {
#if DEBUG
            string measurementId = m_appSettings.GoogleAnalytics.MeasurementIdDev;
            string additionalId = m_appSettings.GoogleAnalytics.AdditionalIdDev;
#else
            string measurementId = m_appSettings.GoogleAnalytics.MeasurementIdProd;
            string additionalId = m_appSettings.GoogleAnalytics.AdditionalIdProd;
#endif
            string url = m_dryRunValidation ?   
                "https://www.google-analytics.com/debug/mp/collect?api_secret={0}&measurement_id={1}" :
                "https://www.google-analytics.com/mp/collect?api_secret={0}&measurement_id={1}";

            /*
             * Don't laugh. Doing security on a public source code, client-only app distributed to the public is not easy. 
             * There are no good solutions. This is why anything needing serious security is server authoritative.
             * Basic techniques have been thwarted. No obvious variable names. The plain text ID is not in source control.
             * There is no line to set a break point on to view the precious data. 
             * Text scrapers and casual inspection are insufficient to break the protection.
             * Either editing the source code or advanced sniffers are needed to break this.
             */
            using StringContent content = new StringContent(messageBody, Encoding.UTF8, "application/json");
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
                        messageBody);
                }
            }
        }

        private static readonly HttpClient s_client = new HttpClient();
        private readonly IAppSettings m_appSettings;
        private readonly bool m_dryRunValidation;

#pragma warning disable IDE0052 // Remove unread private members. 
        //This warning is stupid. The subscriber object only need to be created to be useful. It does not need to be read.
        private readonly GoogleAnalyticsMessageGenerator m_messageGenerator;
#pragma warning restore IDE0052 // Remove unread private members
    }
}