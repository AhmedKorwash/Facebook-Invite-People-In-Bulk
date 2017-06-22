using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoginFacebook;
using System.Net;
using Facebook;
using System.Windows.Forms;
using System.IO;

namespace Facebook_Invite_People
{
    public class InvitePeople
    {
        //How its Work?
        // Simply using this url
        //https://m.facebook.com/a/send_page_invite/?invitee_id=xxxxx&page_id=990971567677455&reference=send_page_invite_from_profile&gfid=xxxx
        //invitee_id immutable for each user
        //gfid immutable at the while you request to invite in referance to the user and page

       public void StartInvite(string user, string pass, string uid)
        {
            AuthFaceBook auth = new AuthFaceBook(user, pass);
            if (auth.IsLogin)
            {
                InviteStructre InvS = new InviteStructre();
                InvS.inviteid = uid;
                InvS.ListData = new Dictionary<string, string>();

                // Prepare GFID List
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://m.facebook.com/pages/invite_to_like_mtouch_mbasic/?target_id="+uid);
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(auth.Cookies);
                request.UserAgent = auth.UserAgent;
                request.KeepAlive = false;
                request.Timeout = 45000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                auth.Cookies.Add(response.Cookies);
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string html = sr.ReadToEnd();
                var logs_page = html.Split(new string[] { "page_id=" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in logs_page)
                {
                    var gfidlisttemp = item.Split(new string[] { "&amp;reference=send_page_invite_from_profile&amp;gfid=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (gfidlisttemp[0].Length < 45)
                    {
                        var gfids = gfidlisttemp[1].Split(new string[] { "\"><span class=" }, StringSplitOptions.RemoveEmptyEntries);
                        InvS.ListData.Add(gfidlisttemp[0], gfids[0]);
                    }
                }



                // Here we have a collection of Page id and their GFID to the user id
                // we can start Httprequest easly to invite them
                foreach (var item in InvS.ListData)
                {
                    request = (HttpWebRequest)WebRequest.Create(string.Format("https://m.facebook.com/a/send_page_invite/?invitee_id={0}&page_id={1}&reference=send_page_invite_from_profile&gfid={2}",uid,item.Key,item.Value));
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(auth.Cookies);
                    request.UserAgent = auth.UserAgent;
                    request.KeepAlive = false;
                    request.Timeout = 45000;
                    response = (HttpWebResponse)request.GetResponse();
                    auth.Cookies.Add(response.Cookies);
                    sr = new StreamReader(response.GetResponseStream());
                    html = sr.ReadToEnd();
                }
                
            }
            else
            {
                MessageBox.Show("Username or Password Falied");
            }
        }
        void StartInvite(string user, string pass, List<string> uids)
        {
            AuthFaceBook auth = new AuthFaceBook(user, pass);
            if (auth.IsLogin)
            {
                foreach (var item in uids)
                {
                    StartInvite(user, pass, item);
                }
            }
            else
            {
                MessageBox.Show("Username or Password Falied");
            }
        }


    }
    public class InviteStructre
    {
        public string inviteid { get; set; }
        public Dictionary<string,string> ListData { get; set; }

    }
}
