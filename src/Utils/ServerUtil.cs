using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using Microsoft.Win32;

namespace Kiss.Utils
{
    /// <summary>
    /// utils for http server or win service
    /// </summary>
    public static class ServerUtil
    {
        public const string HtmlNewLine = "<br />";

        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="urlToEncode"></param>
        /// <returns></returns>
        public static string UrlEncode(string urlToEncode)
        {
            if (string.IsNullOrEmpty(urlToEncode))
                return urlToEncode;

            return HttpUtility.UrlEncode(urlToEncode).Replace("+", "%20");
        }

        /// <summary>
        /// URL解码
        /// </summary>
        /// <param name="urlToDecode"></param>
        /// <returns></returns>
        public static string UrlDecode(string urlToDecode)
        {
            if (string.IsNullOrEmpty(urlToDecode))
                return urlToDecode;

            return System.Web.HttpUtility.UrlDecode(urlToDecode);
        }

        public static string HtmlDecode(string textToFormat)
        {
            if (string.IsNullOrEmpty(textToFormat))
            {
                return textToFormat;
            }
            return HttpUtility.HtmlDecode(textToFormat);
        }

        public static string HtmlEncode(string textToFormat)
        {
            if (string.IsNullOrEmpty(textToFormat))
            {
                return textToFormat;
            }
            string text = HttpUtility.HtmlEncode(textToFormat)
                .Replace("&lt;b&gt;", "<b>")
                .Replace("&lt;/b&gt;", "</b>")
                .Replace("&lt;B&gt;", "<b>")
                .Replace("&lt;/B&gt;", "</b>")
                .Replace("&lt;i&gt;", "<i>")
                .Replace("&lt;/i&gt;", "</i>")
                .Replace("&lt;I&gt;", "<i>")
                .Replace("&lt;/I&gt;", "</i>")
                .Replace("&lt;img", "<img")
                .Replace("/&gt;", "/>")
                .Replace("&quot;", "\"")
                .Replace("&lt;br", "<br")
                .Replace("&amp;nbsp;", "&nbsp;");

            return text;
        }

        public static string MapPath(string path)
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.Server.MapPath(path);
            else
                return PhysicalPath(path.Replace("/", Path.DirectorySeparatorChar.ToString()).Replace("~", ""));
        }


        public static string PhysicalPath(string path)
        {
            return string.Concat(RootPath().TrimEnd(Path.DirectorySeparatorChar), Path.DirectorySeparatorChar.ToString(), path.TrimStart(Path.DirectorySeparatorChar));
        }

        public static string CalculateLocation(string location)
        {
            if (location == null)
                return null;

            string calculatedLocation;

            // 如果已经是物理路径
            //
            if ((location.IndexOf(":\\") != -1) || (location.IndexOf("\\\\") != -1))
                calculatedLocation = location;
            else
                calculatedLocation = MapPath(location);

            if (!calculatedLocation.EndsWith(Path.DirectorySeparatorChar.ToString()))
                calculatedLocation += Path.DirectorySeparatorChar;


            return calculatedLocation;
        }

        private static string _rootPath = null;

        private static string RootPath()
        {
            if (_rootPath == null)
            {
                _rootPath = AppDomain.CurrentDomain.BaseDirectory;
                string dirSep = Path.DirectorySeparatorChar.ToString();

                _rootPath = _rootPath.Replace("/", dirSep);

            }
            return _rootPath;
        }

        public static string HostPath(Uri uri)
        {
            string str = (uri.Port == 80) ? string.Empty : (":" + uri.Port.ToString());
            return string.Format("{0}://{1}{2}", uri.Scheme, uri.Host, str);
        }

        #region Properties
        static public string Language
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.ToString();
            }
        }

        /// <summary>
        /// 应用程序路径
        /// </summary>
        static public string ApplicationPath
        {

            get
            {
                string applicationPath = "/";

                if (HttpContext.Current != null)
                    applicationPath = HttpContext.Current.Request.ApplicationPath;

                return applicationPath.ToLower();
            }

        }

        /// <summary>
        /// IP地址
        /// </summary>
        public static string IPAddress
        {
            get
            {
                if (HttpContext.Current != null)
                    return HttpContext.Current.Request.UserHostAddress;
                return string.Empty;
            }
        }

        #endregion

        public static string CalculateStorageLocation(string storageLocation)
        {
            return CalculateStorageLocation(HttpContext.Current, storageLocation, false);
        }

        public static string CalculateStorageLocation(HttpContext context, string storageLocation)
        {
            return CalculateStorageLocation(context, storageLocation, false);
        }

        public static string CalculateStorageLocation(HttpContext context, string storageLocation, bool endwithDirectorySeparator)
        {
            string calculatedStorageLocation = null;

            // 如果是本地路径
            if ((storageLocation.IndexOf(":\\") != -1) || (storageLocation.IndexOf("\\\\") != -1))
                calculatedStorageLocation = storageLocation;
            else
                calculatedStorageLocation = context.Server.MapPath(storageLocation);

            if (endwithDirectorySeparator)
            {
                // 以斜杠结尾
                if (!calculatedStorageLocation.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    calculatedStorageLocation += Path.DirectorySeparatorChar;
            }
            return calculatedStorageLocation;
        }


        public static string GetServerIPAddress()
        {
            IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            if (addressList.Length > 0)
            {
                return addressList[0].ToString();
            }
            return "127.0.0.1";
        }

        /// <summary>
        /// Finds a Control recursively. Note finds the first match and exits
        /// </summary>
        /// <param name="Root">The top level container to start searching from</param>
        /// <param name="Id">The ID of the control to find</param>
        /// <returns></returns>
        public static Control FindControlRecursive(Control Root, string Id)
        {
            return FindControlRecursive(Root, Id, false);
        }

        /// <summary>
        /// Finds a Control recursively. Note finds the first match and exits
        /// </summary>
        /// <param name="Root">The top level container to start searching from</param>
        /// <param name="Id">The ID of the control to find</param>
        /// <param name="AlwaysUseFindControl">If true uses FindControl to check for hte primary Id which is slower, but finds dynamically generated control ids inside of INamingContainers</param>
        /// <returns></returns>
        public static Control FindControlRecursive(Control Root, string Id, bool AlwaysUseFindControl)
        {
            if (AlwaysUseFindControl)
            {
                Control ctl = Root.FindControl(Id);
                if (ctl != null)
                    return ctl;
            }
            else
            {
                if (Root.ID == Id)
                    return Root;
            }

            foreach (Control Ctl in Root.Controls)
            {
                Control FoundCtl = FindControlRecursive(Ctl, Id, AlwaysUseFindControl);
                if (FoundCtl != null)
                    return FoundCtl;
            }

            return null;
        }

        /// <summary>
        /// Returns a fully qualified HTTP path from a partial path starting out with a ~.
        /// Same syntax that ASP.Net internally supports but this method can be used
        /// outside of the Page framework
        /// </summary>
        /// <param name="RelativePath"></param>
        /// <returns></returns>
        public static string ResolveUrl(string RelativePath)
        {
            if (RelativePath == null)
                return null;

            if (RelativePath.IndexOf(":") != -1)
                return RelativePath;

            // *** Fix up image path for ~ root app dir directory
            if (RelativePath.StartsWith("~"))
            {
                if (HttpContext.Current != null)
                    return StringUtil.CombinUrl(HttpContext.Current.Request.ApplicationPath, RelativePath.Substring(1));
                else
                    // *** Assume current directory is the base directory
                    return RelativePath.Substring(1);
            }

            return RelativePath;
        }

        /// <summary>
        /// Encodes a string to be represented as a string literal. The format
        /// is essentially a JSON string.
        /// 
        /// The string returned includes outer quotes 
        /// Example Output: "Hello \"Rick\"!\r\nRock on"
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string EncodeJsString(string s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"");
            foreach (char c in s)
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        int i = (int)c;
                        if (i < 32 || i > 127)
                        {
                            sb.AppendFormat("\\u{0:X04}", i);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append("\"");

            return sb.ToString();
        }

        /// <summary>
        /// Converts a date to a JavaScript date string in UTC format
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string EncodeJsDate(DateTime date)
        {
            return "new Date(\"" + date.ToString("U") + " UTC" + "\")";
        }

        public static string ExecuteControl(string path)
        {
            Page page = new Page();
            UserControl ctl = (UserControl)page.LoadControl(path);
            page.Controls.Add(ctl);

            return ExecutePage(page);
        }

        public static string ExecutePage(IHttpHandler page)
        {
            using (StringWriter writer = new StringWriter())
            {
                HttpContext.Current.Server.Execute(page, writer, false);

                return writer.ToString();
            }
        }

        public static void AddCache(int cahceMinutes)
        {
            AddCache(HttpContext.Current.Response, cahceMinutes);
        }

        public static void AddCache(HttpResponse response, int cahceMinutes)
        {
            if (cahceMinutes > 0)
            {
                TimeSpan cacheDuration = TimeSpan.FromMinutes(cahceMinutes);
                response.Cache.SetCacheability(HttpCacheability.Public);
                response.Cache.SetExpires(DateTime.Now.AddMinutes(cahceMinutes));
                response.Cache.SetMaxAge(cacheDuration);
                response.Expires = cahceMinutes;
                response.Cache.SetLastModified(DateTime.Now.AddMinutes(-cahceMinutes));
                response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            }
            else
            {
                response.CacheControl = "no-cache";
                response.AddHeader("Pragma", "no-cache");
                response.Expires = -1;
            }
        }

        public static string DnsLookup(string domain)
        {
            try
            {
                if (string.IsNullOrEmpty(domain))
                    return string.Empty;

                IPHostEntry entry = Dns.GetHostEntry(domain);
                IPAddress[] ip_addrs = entry.AddressList;

                if (ip_addrs.Length > 0)
                    return ip_addrs[0].ToString();

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string SetUrl(string key, string value)
        {
            return SetUrl(HttpContext.Current.Request.RawUrl, key, value);
        }

        public static string SetPageUrl(string key, string value)
        {
            string url = HttpContext.Current.Request.RawUrl;

            int index = url.LastIndexOf("/");
            url = url.Remove(index + 1, 1).Insert(index + 1, "1");
            return SetUrl(url, key, value);
        }

        public static string SetUrl(string baseUrl, string key, string value)
        {
            Debug.Assert(!string.IsNullOrEmpty(key));

            if (string.IsNullOrEmpty(value))// remove this querystring
            {
                int index = baseUrl.IndexOf(key);
                int endIndex = baseUrl.IndexOf("&", index);
                if (endIndex == -1)
                    baseUrl = baseUrl.Remove(index);
                else
                    baseUrl = baseUrl.Remove(index, endIndex - index + 1);

                baseUrl = baseUrl.TrimEnd('&');

                return baseUrl;
            }

            if (baseUrl.IndexOf(key) != -1)// aleady contain this querystring key
            {
                baseUrl = SetUrl(baseUrl, key, string.Empty);
                return SetUrl(baseUrl, key, value);
            }
            int count = HttpContext.Current.Request.QueryString.Count;
            return string.Format(count == 0 ? "{0}?{1}={2}" : "{0}&{1}={2}",
                count == 0 ? baseUrl.TrimEnd('?') : baseUrl,
                key,
                value);
        }

        /// <summary>
        /// 设置Windows服务的启动类型
        /// </summary>
        /// <param name="sServiceName">服务名称</param>
        /// <param name="iStartType">要设置的启动类型 1 系统 2 自动 3 手动 4 已禁用 </param>
        /// <returns>true:设置成功; false:设置失败</returns>
        public static bool SetWindowsServiceStartType(String sServiceName, int iStartType)
        {
            try
            {
                ProcessStartInfo objProcessInf = new ProcessStartInfo();
                objProcessInf.FileName = "cmd.exe";
                objProcessInf.CreateNoWindow = false;
                objProcessInf.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                string sStartState = "boot";
                switch (iStartType)
                {
                    case 1:
                        {
                            sStartState = "system";//系统
                            break;
                        }
                    case 2:
                        {
                            sStartState = "auto";//自动
                            break;
                        }
                    case 3:
                        {
                            sStartState = "demand";//手动
                            break;
                        }
                    case 4:
                        {
                            sStartState = "disabled";//已禁用
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                //设置指定服务的启动类型
                objProcessInf.Arguments = "/c sc config " + sServiceName + " start= " + sStartState;
                Process.Start(objProcessInf);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 如果指定的Windows服务不在运行状态,则启动该Windows服务 
        /// </summary>
        /// <param name="sServiceName">Windows服务的名称</param>
        /// <returns>true:启动成功; false:启动失败</returns>
        public static bool StartWindowService(string sServiceName)
        {
            try
            {
                ServiceController controller = new ServiceController(sServiceName);
                //如果状态为停止,那么启动服务
                if (controller.Status == ServiceControllerStatus.Stopped)
                {
                    try
                    {
                        controller.Start();
                    }
                    catch
                    {
                        return false;
                    }
                    controller.Refresh();
                }
                //如果状态为暂停,那么继续服务
                while (controller.Status != ServiceControllerStatus.Running)
                {
                    Thread.Sleep(10);

                    controller.Refresh();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检测是否安装有打印机
        /// </summary>
        /// <returns>true:安装有打印机 false:没安装打印机</returns>
        public static bool HasPrinter()
        {
            try
            {
                return PrinterSettings.InstalledPrinters.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        public static void StartPrintService()
        {
            string sServiceName = "Spooler";

            //判断当前打印服务是否运行
            if (!IsWindowsServiceRunning(sServiceName))
            {
                //如果打印服务已经被禁用，那么设置为自动
                if (GetWindowsServiceStartType(sServiceName) == 4)
                {
                    //设置打印服务为自动
                    bool setStartType = SetWindowsServiceStartType(sServiceName, 2);
                    if (setStartType == true)
                    {
                        Thread.Sleep(500);//延迟半秒时间
                    }
                    else
                    {
                        throw new Exception("设置打印后台程序服务Print Spooler启动类型为自动,操作失败!");
                    }
                }
            }
            //启动打印服务
            if (StartWindowService(sServiceName))
            {
                if (!HasPrinter())
                {
                    throw new Exception("系统尚未安装打印机!");
                }
            }
            else
            {
                throw new Exception("打印后台程序服务Print Spooler启动失败!");
            }
        }

        /// <summary>
        /// 得到指定Windows服务的启动类型
        /// [HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\ServerName] Start"=dword:00000002   其中2为自动,   3为手动. 
        /// </summary>
        /// <param name="sServiceName">Windows服务的名称</param>
        /// <returns>2:自动   3:手动   4:已禁用</returns>
        public static int GetWindowsServiceStartType(string sServiceName)
        {
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey system = rk.OpenSubKey("SYSTEM");
            RegistryKey currentControlSet = system.OpenSubKey("CurrentControlSet");
            RegistryKey services = currentControlSet.OpenSubKey("Services");
            RegistryKey windowsServiceName = services.OpenSubKey(sServiceName);
            int typeValue = Convert.ToInt32(windowsServiceName.GetValue("Start").ToString());
            return typeValue;
        }

        /// <summary>
        /// 判断指定的Windows服务是否运行
        /// </summary>
        /// <param name="sServiceName">Windows服务的名称</param>
        /// <returns>true: 正在运行; false:停止</returns>
        public static bool IsWindowsServiceRunning(string sServiceName)
        {
            ServiceController controller = new ServiceController(sServiceName);
            return controller.Status == ServiceControllerStatus.Running;
        }
    }
}
