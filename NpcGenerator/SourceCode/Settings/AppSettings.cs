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
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace NpcGenerator
{
    public class GoogleAnalyticsSettings : IGoogleAnalyticsSettings
    {
        public string MeasurementIdDev { get; set; }
        public string MeasurementIdProd { get; set; }
        public string AdditionalIdDev { get; set; }
        public string AdditionalIdProd { get; set; }
        
        public void Validate()
        {
            if (string.IsNullOrEmpty(MeasurementIdDev))
            {
                throw new InvalidProductKeyException(nameof(MeasurementIdDev), MeasurementIdDev);
            }

            if (string.IsNullOrEmpty(MeasurementIdProd))
            {
                throw new InvalidProductKeyException(nameof(MeasurementIdProd), MeasurementIdProd);
            }

            if (string.IsNullOrEmpty(AdditionalIdDev))
            {
                throw new InvalidProductKeyException(nameof(AdditionalIdDev), AdditionalIdDev);
            }

            if (string.IsNullOrEmpty(AdditionalIdProd))
            {
                throw new InvalidProductKeyException(nameof(AdditionalIdProd), AdditionalIdProd);
            }
        }
    }

    public class AppSettings : IAppSettings
    {
        public IGoogleAnalyticsSettings GoogleAnalytics { get; set; } = new GoogleAnalyticsSettings();

        public int EncryptionKey { get; set; }

        public string DefaultLanguageCode { get; set; }

        public ReadOnlyCollection<string> HiddenLanguageCodes { get; set; }

        public string HomeWebsite { get; set; }

        public string DonationWebsite { get; set; }

        public string SupportEmail { get; set; }

        public string DefaultConfigurationRelativePath { get; set; }

        public static AppSettings Create(string json)
        {
            ParamUtil.VerifyHasContent(nameof(json), json);

            AppSettings settings = JsonConvert.DeserializeObject<AppSettings>(json);
            settings.Validate();
            return settings;
        }

        private void Validate()
        {
            GoogleAnalytics.Validate();
            ValidateEncryptionKey();
            ValidateDefaultLanguageCode();
            ValidateHiddenLanguageCodes();
            ValidateWebsite(HomeWebsite);
            ValidateWebsite(DonationWebsite);
            ValidateEmail(SupportEmail);
            ValidateDefaultConfigurationRelativePath();
        }

        private static void ValidateEncryptionKey()
        {
            //Any value is acceptable.
        }

        private void ValidateDefaultLanguageCode()
        {
            if (string.IsNullOrWhiteSpace(DefaultLanguageCode))
            {
                throw new InvalidDefaultLanguageCodeException();
            }
        }

        private void ValidateHiddenLanguageCodes()
        {
            //The list can be null or empty, but contents cannot
            if (HiddenLanguageCodes != null)
            {
                foreach (string language in HiddenLanguageCodes)
                {
                    if (string.IsNullOrWhiteSpace(language))
                    {
                        throw new InvalidHiddenLanguageCodeException();
                    }
                }
            }
        }

        private static void ValidateWebsite(string website)
        {
            if (string.IsNullOrWhiteSpace(website))
            {
                throw new MalformedWebsiteException(website);
            }

            try
            {
                Uri uri = new Uri(website);
                bool isValid = uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
                if (!isValid)
                {
                    throw new MalformedWebsiteException(website);
                }
            }
            catch (UriFormatException)
            {
                throw new MalformedWebsiteException(website);
            }
        }

        private static void ValidateEmail(string email)
        {
            bool isValid = IsValidEmail(email);
            if (!isValid)
            {
                throw new MalformedEmailException(email);
            }
        }

        //Taken from https://stackoverflow.com/questions/1365407/c-sharp-code-to-validate-email-address
        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            string trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void ValidateDefaultConfigurationRelativePath()
        {
            if (DefaultConfigurationRelativePath is null)
            {
                throw new InvalidDefaultConfigurationRelativePath(DefaultConfigurationRelativePath);
            }

            string absolutePath = PathHelper.FullPathOf(DefaultConfigurationRelativePath);
            bool doesExists = File.Exists(absolutePath);
            if (!doesExists)
            {
                throw new InvalidDefaultConfigurationRelativePath(DefaultConfigurationRelativePath);
            }
        }
    }

    public class InvalidDefaultLanguageCodeException : Exception
    {
        public InvalidDefaultLanguageCodeException()
        {
        }
    }

    public class InvalidHiddenLanguageCodeException : Exception
    {
        public InvalidHiddenLanguageCodeException()
        {
        }
    }

    public class MalformedWebsiteException : FormatException
    {
        public MalformedWebsiteException(string uri)
        {
            Uri = uri;
        }

        public string Uri { get; private set; }
    }

    public class MalformedEmailException : FormatException
    {
        public MalformedEmailException(string email)
        {
            Email = email;
        }

        public string Email { get; private set; }
    }

    public class InvalidProductKeyException : Exception
    {
        public InvalidProductKeyException(string name, string value)
        {
            ProductKeyName = name;
            ProductKeyValue = value;
        }

        public string ProductKeyName { get; private set; }
        public string ProductKeyValue { get; private set; }
    }

    public class InvalidDefaultConfigurationRelativePath : Exception
    {
        public InvalidDefaultConfigurationRelativePath(string path)
        {
            Path = path;
        }

        public string Path { get; private set; }
    }
}