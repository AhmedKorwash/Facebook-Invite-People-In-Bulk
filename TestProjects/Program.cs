using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook_Invite_People;

namespace TestProjects
{
    class Program
    {
        static void Main(string[] args)
        {
            InvitePeople p = new InvitePeople();
            p.StartInvite("akorwash", "Iloveyouaya", "100005832311252");
        }
    }
}
