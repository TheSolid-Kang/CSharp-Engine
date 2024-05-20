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
        public string description { get; set; } = "";
        public string department { get; set; } = "";
        public string displayName { get; set; } = "";
        public string distinguishedName { get; set; } = "";
        public string givenName { get; set; } = ""; //이름
        public string homePhone { get; set; } = ""; //내선번호
        public DateTime lastLogon { get; set; }
        public DateTime lastLogonTimestamp { get; set; }
        public string name { get; set; } = ""; //이름 전체
        public string otherHomePhone { get; set; } = ""; //기타 내선번호
        public string otherMobile { get; set; } = ""; //기타 휴대폰번호
        public int primaryGroupID { get; set; }
        public DateTime pwdLastSet { get; set; }
        public string sn { get; set; } = ""; //성
        public string title { get; set; } = ""; //직급
        public string mail { get; set; } = "";
        public string manager { get; set; } = ""; //관리자
        public string sAMAccountName { get; set; } = "";
        public string userGroup { get; set; } = "";
        public string userPrincipalName { get; set; } = ""; //yonwoo.cos / yonwookorea.com 구분
        public long uSNChanged { get; set; }
        public long uSNCreated { get; set; }
        public DateTime whenChanged { get; set; }
        public DateTime whenCreated { get; set; }
    }
}
