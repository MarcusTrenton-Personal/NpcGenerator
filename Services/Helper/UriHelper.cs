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

using System;
using System.Diagnostics;
using System.Text;

namespace Services
{
    public static class UriHelper
    {
        //Adapted from https://stackoverflow.com/questions/502199/how-to-open-a-web-page-from-my-application
        public static bool OpenUri(Uri uri)
        {
            if(uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            bool isValid = uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
            if (isValid)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = uri.ToString(),
                    UseShellExecute = true
                });
            }
            return isValid;
        }

        public const int MAX_EMAIL_MESSAGE_LENGTH = 36_500; //Obtained by trail and error.
        public static void WriteEmail(string email, string subject, string body)
        {
            ParamUtil.VerifyMatchesPattern(nameof(email), email, RegexUtil.EMAIL, email + " is not an email address");
            ParamUtil.VerifyHasContent(nameof(subject), subject);
            ParamUtil.VerifyHasContent(nameof(body), body);
            
            if (email.Length + subject.Length + body.Length > MAX_EMAIL_MESSAGE_LENGTH)
            {
                throw new ArgumentException("Combination of email, subject, and body exceed maximum length of " + MAX_EMAIL_MESSAGE_LENGTH);
            }

            StringBuilder actionBuilder = new StringBuilder();
            actionBuilder.Append("mailto:" + email);
            actionBuilder.Append("?subject=" + subject);
            actionBuilder.Append("&body=" + body);

            Uri uri = new Uri(actionBuilder.ToString());
            Process.Start(new ProcessStartInfo
            {
                FileName = uri.ToString(),
                UseShellExecute = true
            });
        }
    }
}
