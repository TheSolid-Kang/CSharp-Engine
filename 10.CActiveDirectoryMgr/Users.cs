using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine._10.CActiveDirectoryMgr
{
    public class Users
    {
        public string cn { get; set; } = "";
        public string department { get; set; } = "";
        public string displayName { get; set; } = "";
        public string distinguishedName { get; set; } = "";
        public string givenName { get; set; } = ""; //이름
        public DateTime lastLogon { get; set; }
        public DateTime lastLogonTimestamp { get; set; }
        public string name { get; set; } = ""; //이름 전체
        public int primaryGroupID { get; set; }
        public DateTime pwdLastSet { get; set; }
        public string sn { get; set; } = ""; //성
        public string title { get; set; } = ""; //직급
        public string mail { get; set; } = "";
        public string sAMAccountName { get; set; } = "";
        public string userGroup { get; set; } = "";
        public string userPrincipalName { get; set; } = ""; //yonwoo.cos / yonwookorea.com 구분
        public DateTime whenChanged { get; set; }
        public DateTime whenCreated { get; set; }
    }
}
