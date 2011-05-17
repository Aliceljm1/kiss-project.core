using System;
using System.IO;
using System.Net;
using System.Security;
using System.Text;

namespace Kiss.Utils.Net
{
    public class HttpRequest
    {
        private const int defaultTimeout_ = 60000;
        private static string referer_ = "Kiss";

        //public static readonly string UserAgent = String.Format("Openlab {0} ({1}; .NET CLR {2};)", SiteStatistics.VersionVersionInfo, Environment.OSVersion.ToString(), Environment.Version.ToString());

        /// <summary>
        /// Creates a new HttpRequest with the default Referral value
        /// </summary>
        public static HttpWebRequest CreateRequest(string url)
        {
            return CreateRequest(url, referer_);
        }

        /// <summary>
        /// Creates a new HttpRequest and sets the referral value.
        /// </summary>
        public static HttpWebRequest CreateRequest(string url, string referral)
        {
            ICredentials credentials = null;

            //note this code will not work if there is an @ or : in the username or password
            if (url.IndexOf('@') > 0)
            {
                string[] urlparts = url.Split('@');
                if (urlparts.Length >= 2)
                {
                    string[] userparts = urlparts[0].Split(':');

                    if (userparts.Length == 3)
                    {
                        string protocol = userparts[0];
                        string username = userparts[1].TrimStart('/');
                        string password = userparts[2];

                        credentials = new NetworkCredential(username, password);
                        url = url.Replace(string.Format("{0}:{1}@", username, password), "");
                    }

                }
            }
            else
            {
                credentials = CredentialCache.DefaultCredentials;
            }

            WebRequest req;

            // This may throw a SecurityException if under medium trust... should set it to null so it will return instead of error out.
            try { req = WebRequest.Create(url); }
            catch (SecurityException) { req = null; }

            HttpWebRequest wreq = req as HttpWebRequest;
            if (null != wreq)
            {
                //wreq.UserAgent = UserAgent;
                wreq.Referer = referral;
                wreq.Timeout = defaultTimeout_;

                if (credentials != null)
                    wreq.Credentials = credentials;
            }
            return wreq;
        }

        public static HttpWebResponse MakeHttpPost(string url, string referral, params HttpPostItem[] items)
        {
            return MakeHttpPost(CreateRequest(url, referral), items);
        }

        public static HttpWebResponse MakeHttpPost(string url, params HttpPostItem[] items)
        {
            return MakeHttpPost(url, null, items);
        }

        public static HttpWebResponse MakeHttpPost(HttpWebRequest wreq, params HttpPostItem[] items)
        {
            if (wreq == null)
                throw new Exception("HttpWebRequest is not initialized");

            if (items == null || items.Length == 0)
                throw new Exception("No HttpPostItems");

            StringBuilder parameters = new StringBuilder();

            foreach (HttpPostItem item in items)
            {
                parameters.Append("&" + item.ToString());
            }

            byte[] payload = Encoding.UTF8.GetBytes(parameters.ToString().Substring(1));

            wreq.Method = "POST";
            wreq.ContentLength = payload.Length;
            wreq.ContentType = "application/x-www-form-urlencoded";
            wreq.KeepAlive = false;

            using (Stream st = wreq.GetRequestStream())
            {
                st.Write(payload, 0, payload.Length);
                st.Close();

                return wreq.GetResponse() as HttpWebResponse;
            }
        }

        /// <summary>
        /// Gets the HttpResponse using the default referral
        /// </summary>
        public static HttpWebResponse GetResponse(string url)
        {
            WebExceptionStatus status;
            return GetResponse(url, referer_, out status);
        }

        /// <summary>
        /// Gets the HttpResponse using the supplied referral
        /// </summary>
        public static HttpWebResponse GetResponse(string url, string referral, out WebExceptionStatus status)
        {
            HttpWebRequest request = CreateRequest(url, referral);
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                status = WebExceptionStatus.Success;
            }
            catch (WebException e)
            {
                status = e.Status;
            }
            catch
            {
                status = WebExceptionStatus.UnknownError;
            }
            return response;
        }

        public static string GetPageText(string url)
        {
            return GetPageText(url, referer_);
        }

        /// <summary>
        /// Gets the full text at the url parameter
        /// </summary>
        public static string GetPageText(string url, Encoding encode, out WebExceptionStatus status)
        {
            return GetPageText(url, referer_, encode, out status);
        }

        /// <summary>
        /// Gets the full text at the url parameter
        /// </summary>
        public static string GetPageText(string url, string referral, Encoding encode, out WebExceptionStatus status)
        {
            HttpWebResponse response = GetResponse(url, referral, out status);
            if (response == null)
                return null;
            return GetPageText(response, encode);
        }

        public static string GetPageText(string url, string referral)
        {
            WebExceptionStatus status;

            HttpWebResponse response = GetResponse(url, referral, out status);
            return GetPageText(response);
        }

        /// <summary>
        /// Gets the full text at the url parameter
        /// </summary>
        public static string GetPageText(HttpWebResponse response)
        {
            Encoding encode;
            string enc = response.ContentEncoding.Trim();

            if (string.IsNullOrEmpty(enc) && StringUtil.HasText(response.CharacterSet))
                enc = response.CharacterSet;

            if (string.IsNullOrEmpty(enc) && StringUtil.HasText(response.ContentType))
            {
                string contentType = response.ContentType;
                string c = "charset=";
                enc = contentType.Substring(contentType.IndexOf(c) + c.Length);
            }

            if (string.IsNullOrEmpty(enc))
                encode = Encoding.UTF8;
            else
                encode = Encoding.GetEncoding(enc);

            return GetPageText(response, encode);
        }

        public static string GetPageText(HttpWebResponse response, Encoding encode)
        {
            using (Stream s = response.GetResponseStream())
            {
                string html;
                using (StreamReader sr = new StreamReader(s, encode))
                {
                    html = sr.ReadToEnd();
                }
                return html;
            }
        }

    }
}
