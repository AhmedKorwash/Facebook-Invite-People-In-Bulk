using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace LoginFacebook
{
    public class AuthFaceBook
    {
        private bool islogin;
        public bool IsLogin
        {
            get { return islogin; }
            set { islogin = value; }
        }
        private string username;
        public string UserName
        {
            get { return username; }
            set { username = value; }
        }
        private string pass;
        public string Password
        {
            get { return pass; }
            set { pass = value; }
        }
        private CookieCollection cookies;
        public CookieCollection Cookies
        {
            get { return cookies; }
            set { cookies = value; }
        }
        // Useragent of blackberry you can use any other useragent as you like.
        private string useragent = "Mozilla/5.0 (BlackBerry; U; BlackBerry 9900; en) AppleWebKit/534.11+ (KHTML, like Gecko) Version/7.1.0.346 Mobile Safari/534.11+";//"Mozilla/5.0 (Windows NT x.y; Win64; x64; rv:10.0) Gecko/20100101 Firefox/10.0";
        public AuthFaceBook(string username, string password)
        {
            AuthFaceBookNow(username, password);
        }

        public AuthFaceBook()
        {
            // TODO: Complete member initialization
        }
        public void AuthFaceBookNow(string username, string password)
        {
            UserName = username;
            Password = password;
            cookies = new CookieCollection();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.facebook.com/");
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
                request.UserAgent = useragent;
                request.KeepAlive = false;
                request.Timeout = 45000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                cookies.Add(response.Cookies);
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string html = sr.ReadToEnd();
                html = html.Substring(html.IndexOf("login_form"));
                html = html.Remove(html.IndexOf("/form"));
                Regex reg = new Regex(@"name=""[^""]+"" value=""[^""]+""");
                MatchCollection mc = reg.Matches(html);
                List<string> values = new List<string>();
                for (int k = 0; k < mc.Count; k++)
                {
                    if (k != 0)
                        postData += "&";
                    if (k == 1)
                    {
                        postData += "email=" + this.username + "&pass=" + this.pass + "&";
                    }
                    string m = mc[k].Value.Replace("\"", "");
                    m = m.Replace("name=", "");
                    m = m.Replace(" value=", "=");
                    postData += m;
                }


                string getUrl = "https://www.facebook.com/login.php?login_attempt=1";
                HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(getUrl);
                getRequest.CookieContainer = new CookieContainer();
                // Send Cookies from the first step
                getRequest.CookieContainer.Add(cookies);
                getRequest.Method = WebRequestMethods.Http.Post;
                getRequest.UserAgent = useragent;
                getRequest.AllowWriteStreamBuffering = true;
                getRequest.ProtocolVersion = HttpVersion.Version11;
                getRequest.AllowAutoRedirect = false;
                getRequest.ContentType = "application/x-www-form-urlencoded";
                getRequest.Referer = "https://www.facebook.com";
                getRequest.KeepAlive = false;
                getRequest.Timeout = 45000;
                //postData is the Parameter needed to complete Login Request
                byte[] byteArray = Encoding.ASCII.GetBytes(postData);
                getRequest.ContentLength = byteArray.Length;
                Stream newStream = getRequest.GetRequestStream(); //open connection
                newStream.Write(byteArray, 0, byteArray.Length); // Send the data.
                newStream.Close();

                HttpWebResponse getResponse = (HttpWebResponse)getRequest.GetResponse();
                cookies.Add(getResponse.Cookies);
                //In General facebook back 6:8 cookies when you login this is best solution to determin done or not.
                if (getResponse.Cookies.Count > 6)
                    islogin = true;
                else
                    islogin = false;

            }
            catch { }

        }
        public string postData { get; set; }
        public void ReloadAuth()
        {
            AuthFaceBookNow(username, pass);
        }
        public string UserAgent
        {
            get { return useragent; }
            set { useragent = value; }
        }




    }
}
