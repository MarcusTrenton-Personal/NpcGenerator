using System;
using System.Diagnostics;
using System.Text;

namespace NpcGenerator
{
    public static class UriHelper
    {
        //Adapted from https://stackoverflow.com/questions/502199/how-to-open-a-web-page-from-my-application
        public static bool OpenUri(Uri uri)
        {
            if(uri == null)
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
    }
}
