﻿/*Copyright(C) 2022 Marcus Trenton, marcus.trenton@gmail.com

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
using Services.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NpcGenerator
{
    public class GoogleAnalyticsMessageGenerator
    {
        //https://developers.google.com/analytics/devguides/collection/protocol/ga4/sending-events?client_type=gtag#limitations
        const int USER_PROPERY_NAME_MAX_LENGTH = 24;
        const int USER_PROPERY_VALUE_MAX_LENGTH = 36;
        const int EVENT_NAME_MAX_LENGTH = 40;
        const int PARAMETER_NAME_MAX_LENGTH = 40;
        const int PARAMETER_VALUE_MAX_LENGTH = 100;

        public GoogleAnalyticsMessageGenerator(in ITrackingProfile trackingProfile,
            in IMessager messager,
            in IUserSettings userSettings,
            Action<string> messageCallback)
        {
            m_trackingProfile = trackingProfile ?? throw new ArgumentNullException(nameof(trackingProfile));
            m_messager = messager ?? throw new ArgumentNullException(nameof(messager));
            m_userSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
            m_messageCallback = messageCallback ?? throw new ArgumentNullException(nameof(messageCallback));

            
            m_messager.Subscribe<Services.Message.Crash>(OnCrash);
            m_messager.Subscribe<Message.InvalidNpcs>(OnInvalidNpcs);
            m_messager.Subscribe<Message.GenerateNpcs>(OnGenerateNpcs);
            m_messager.Subscribe<Services.Message.LanguageSelected>(OnLanguageSelected);
            m_messager.Subscribe<Services.Message.Login>(OnLogin);
            m_messager.Subscribe<Services.Message.PageView>(OnPageView);
            m_messager.Subscribe<Message.SaveNpcs>(OnSaveNpcs);
            m_messager.Subscribe<Message.SelectConfiguration>(OnSelectConfiguration);
            m_messager.Subscribe<Services.Message.UserLanguageNotSupported>(OnUserLanguageNotSupported);
        }

        ~GoogleAnalyticsMessageGenerator()
        {
            if (m_messager != null)
            {
                m_messager.Unsubscribe<Services.Message.Crash>(OnCrash);
                m_messager.Unsubscribe<Message.InvalidNpcs>(OnInvalidNpcs);
                m_messager.Unsubscribe<Message.GenerateNpcs>(OnGenerateNpcs);
                m_messager.Unsubscribe<Services.Message.LanguageSelected>(OnLanguageSelected);
                m_messager.Unsubscribe<Services.Message.Login>(OnLogin);
                m_messager.Unsubscribe<Services.Message.PageView>(OnPageView);
                m_messager.Unsubscribe<Message.SaveNpcs>(OnSaveNpcs);
                m_messager.Unsubscribe<Message.SelectConfiguration>(OnSelectConfiguration);
                m_messager.Unsubscribe<Services.Message.UserLanguageNotSupported>(OnUserLanguageNotSupported);
            }
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
                Trace.Assert(name.Length <= USER_PROPERY_NAME_MAX_LENGTH, 
                    "User property name " + name + " is longer than the " + USER_PROPERY_NAME_MAX_LENGTH + " character maximum");
                Trace.Assert(value.Length <= USER_PROPERY_VALUE_MAX_LENGTH,
                    "User property value " + value + " is longer than the " + USER_PROPERY_VALUE_MAX_LENGTH + " character maximum");

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
            WriteUserProperty(writer, "os_version", m_trackingProfile.OsVersion);

            writer.WriteEnd(); //End of user_properties object
        }

        private static void WriteEventName(JsonWriter writer, string eventName)
        {
            Trace.Assert(eventName.Length <= USER_PROPERY_NAME_MAX_LENGTH,
                "User property name " + eventName + " is longer than the " + EVENT_NAME_MAX_LENGTH + " character maximum");

            writer.WritePropertyName("name");
            writer.WriteValue(eventName);
        }

        private static void WriteParameter(JsonWriter writer, string parameter, string value)
        {
            if (!string.IsNullOrEmpty(parameter) && !string.IsNullOrEmpty(value))
            {
                WriteParameterName(writer, parameter);

                Trace.Assert(value.Length <= PARAMETER_VALUE_MAX_LENGTH,
                    "User property name " + value + " is longer than the " + PARAMETER_VALUE_MAX_LENGTH + " character maximum");

                writer.WriteValue(value);
            }
        }

        private static void WriteParameter(JsonWriter writer, string parameter, bool value)
        {
            WriteParameterName(writer, parameter);
            writer.WriteValue(value);
        }

        private static void WriteParameter(JsonWriter writer, string parameter, int value)
        {
            WriteParameterName(writer, parameter);
            writer.WriteValue(value);
        }

        private static void WriteParameterName(JsonWriter writer, string parameter)
        {
            Trace.Assert(parameter.Length <= PARAMETER_NAME_MAX_LENGTH,
                "User property name " + parameter + " is longer than the " + PARAMETER_NAME_MAX_LENGTH + " character maximum");

            writer.WritePropertyName(parameter);
        }

        private void OnCrash(object sender, Services.Message.Crash crash)
        {
            //Consent can change throughout the session
            if (m_userSettings.AnalyticsConsent)
            {
                WriteMessageBody(Callback);
            }

            void Callback(JsonWriter writer)
            {
                WriteCrashEvent(writer, crash);
            }
        }

        private static void WriteCrashEvent(JsonWriter writer, Services.Message.Crash crash)
        {
            writer.WriteStartObject(); //Start of event object

            WriteEventName(writer, "crash");

            writer.WritePropertyName("params");
            writer.WriteStartObject();

            //This hacky crash reporter has a very low character limit. This might cover the first function of the call stack.
            string baseMessageString = crash.Exception.GetType() + ": " + crash.Exception.Message;
            string messageString = Truncate(baseMessageString, PARAMETER_VALUE_MAX_LENGTH);
            WriteParameter(writer, "message", messageString);
            string callStrackString = Truncate(crash.Exception.StackTrace, PARAMETER_VALUE_MAX_LENGTH);
            WriteParameter(writer, "call_stack", callStrackString);

            writer.WriteEnd(); //End of params object

            writer.WriteEnd(); //End of event object
        }

        private static string Truncate(string original, int maxLength)
        {
            string truncated = original != null && original.Length > maxLength ? original[..maxLength] : original;
            return truncated;
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

            WriteEventName(writer, "login");

            writer.WritePropertyName("params");
            writer.WriteStartObject();

            WriteParameter(writer, "method", "ClientApp");

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

            WriteEventName(writer, "page_view");

            writer.WritePropertyName("params");
            writer.WriteStartObject();

            WriteParameter(writer, "title", title);

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

            WriteEventName(writer, "select_configuration");

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
                WriteGenerateNpcsEvent(writer, generateNpcs.Quantity, generateNpcs.Features);
            }
        }

        private static void WriteGenerateNpcsEvent(JsonWriter writer, int quantity, TraitSchema.Features features)
        {
            writer.WriteStartObject(); //Start of event object

            WriteEventName(writer, "generate_npcs");

            writer.WritePropertyName("params");
            writer.WriteStartObject();

            WriteParameter(writer, "quantity", quantity);

            WriteSchemaFeatures(writer, features);

            writer.WriteEnd(); //End of params object

            writer.WriteEnd(); //End of event object
        }

        private static void WriteSchemaFeatures(JsonWriter writer, TraitSchema.Features features)
        {
            foreach (int featureValue in Enum.GetValues(typeof(TraitSchema.Features)))
            {
                bool hasFeature = (featureValue & (int)features) > 0;
                string featureName = Enum.GetName(typeof(TraitSchema.Features), featureValue);
                WriteParameter(writer, featureName, hasFeature);
            }
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

        private static void WriteSaveNpcsEvent(JsonWriter writer, string format)
        {
            writer.WriteStartObject(); //Start of event object

            WriteEventName(writer, "save_npcs");

            writer.WritePropertyName("params");
            writer.WriteStartObject();

            WriteParameter(writer, "format", format);

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

            WriteEventName(writer, "unsupported_language");

            writer.WritePropertyName("params");
            writer.WriteStartObject();

            WriteParameter(writer, "language_code", languageCode);

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

            WriteEventName(writer, "language_selected");

            writer.WritePropertyName("params");
            writer.WriteStartObject();

            WriteParameter(writer, "language_code", languageCode);

            writer.WriteEnd(); //End of params object

            writer.WriteEnd(); //End of event object
        }

        private void OnInvalidNpcs(object sender, Message.InvalidNpcs invalidNpcs)
        {
            if (m_userSettings.AnalyticsConsent)
            {
                WriteMessageBody(Callback);
            }

            void Callback(JsonWriter writer)
            {
                HashSet<NpcSchemaViolation.Reason> violationTypes = GetTypesOfNpcSchemaViolation(invalidNpcs.Violations);
                WriteOnInvalidEvent(writer, violationTypes);
            }
        }

        private static HashSet<NpcSchemaViolation.Reason> GetTypesOfNpcSchemaViolation(NpcSchemaViolationCollection violations)
        {
            HashSet<NpcSchemaViolation.Reason> result = new HashSet<NpcSchemaViolation.Reason>();
            foreach (NpcSchemaViolation violation in violations.categoryViolations)
            {
                result.Add(violation.Violation);
            }
            foreach (List<NpcSchemaViolation> violationList in violations.violationsByNpc.Values)
            {
                foreach (NpcSchemaViolation violation in violationList)
                {
                    result.Add(violation.Violation);
                }
            }
            return result;
        }

        private static void WriteOnInvalidEvent(JsonWriter writer, HashSet<NpcSchemaViolation.Reason> violationTypes)
        {
            writer.WriteStartObject(); //Start of event object

            WriteEventName(writer, "invalid_npcs");

            writer.WritePropertyName("params");
            writer.WriteStartObject();

            foreach (NpcSchemaViolation.Reason reason in violationTypes)
            {
                WriteParameter(writer, reason.ToString(), true);
            }

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
