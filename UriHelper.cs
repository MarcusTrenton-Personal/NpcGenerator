using System;
using System.Diagnostics;
using System.Text;

namespace NpcGenerator
{
    public static class UriHelper
    {
        //Taken from https://stackoverflow.com/questions/502199/how-to-open-a-web-page-from-my-application
        public static bool IsValidUri(string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                return false;
            Uri tmp;
            if (!Uri.TryCreate(uri, UriKind.Absolute, out tmp))
                return false;
            return tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps;
        }

        public static bool OpenUri(string uri)
        {
            bool isValid = IsValidUri(uri);
            if (isValid)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = uri,
                    UseShellExecute = true
                });
            }
            return isValid;
        }
    }
}
