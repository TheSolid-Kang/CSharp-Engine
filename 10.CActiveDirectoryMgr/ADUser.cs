using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine._10.CActiveDirectoryMgr
{
    public class ADUser
    {
        public string cn { get; set; } = "";
        public string description { get; set; } = "";
        public string department { get; set; } = "";
        public string displayName { get; set; } = "";
        public string distinguishedName { get; set; } = "";
        public string givenName { get; set; } = ""; //이름
        public string homePhone { get; set; } = ""; //내선번호
        public DateTime lastLogon { get; set; }
        //public DateTime lastLogonTimestamp { get; set; }
        //public string name { get; set; } = ""; //이름 전체
        public string otherHomePhone { get; set; } = ""; //기타 내선번호
        public string otherMobile { get; set; } = ""; //기타 휴대폰번호
        //public Byte[] objectGUID { get; set; } //사용자 객체 키값
        //Encoding.Unicode.GetString(objectGUID);
        //public string strobjectGUID { get { return BitConverter.ToString(objectGUID); } } //사용자 객체 키값
        //public Byte[] objectSid { get; set; } //
        //public string strobjectSid { get; } //
        //public int primaryGroupID { get; set; }
        public DateTime pwdLastSet { get; set; }
        public string sn { get; set; } = ""; //성
        public string title { get; set; } = ""; //직급
        public string mail { get; set; } = "";
        public string manager { get; set; } = ""; //관리자
        public string mobile { get; set; } = ""; //휴대폰
        [Key]
        public string sAMAccountName { get; set; } = "";
        //public string userGroup { get; set; } = "";
        public string userPrincipalName { get; set; } = ""; //yonwoo.cos / yonwookorea.com 구분
        public long uSNChanged { get; set; }
        public long uSNCreated { get; set; }
        public DateTime whenChanged { get; set; }
        public DateTime whenCreated { get; set; }
    }
    public class ADUserComparer : IEqualityComparer<ADUser>
    {
        public bool Equals(ADUser x, ADUser y)
        {
            return x.cn == y.cn
                    && x.description == y.description
                    && x.department == y.department
                    && x.displayName == y.displayName
                    && x.distinguishedName == y.distinguishedName
                    && x.givenName == y.givenName
                    && x.homePhone == y.homePhone
                    && x.otherHomePhone == y.otherHomePhone
                    && x.otherMobile == y.otherMobile
                    && x.sn == y.sn
                    && x.title == y.title
                    && x.mail == y.mail
                    && x.manager == y.manager
                    && x.mobile == y.mobile
                    && x.sAMAccountName == y.sAMAccountName
                    && x.userPrincipalName == y.userPrincipalName
                    && x.uSNChanged == y.uSNChanged
                    && x.uSNCreated == y.uSNCreated;
        }

        public int GetHashCode(ADUser obj)
        {
            return obj.uSNCreated.GetHashCode() ^ obj.uSNCreated.GetHashCode();
        }
    }
}
