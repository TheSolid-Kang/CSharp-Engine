using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine._10.CActiveDirectoryMgr
{
    public class ADGroup
    {
        #region 그룹 공통
        public string cn { get; set; } = "";
        public string distinguishedName { get; set; } = "";
        public long groupType { get; set; }
        public string member { get; set; } = ""; //구성원
        public string name { get; set; } = "";
        public string objectCategory { get; set; } = "";
        public string objectClass { get; set; } = "";
        public string sAMAccountName { get; set; } = "";
        public long sAMAccountType { get; set; } = 0;
        public long uSNChanged { get; set; }
        public long uSNCreated { get; set; }
        public DateTime whenChanged { get; set; }
        public DateTime whenCreated { get; set; }
        #endregion
        #region 조직그룹 
        public string description { get; set; } = "";
        public string mail { get; set; } = "";
        #endregion

    }
    public class ADGroupComparer : IEqualityComparer<ADGroup>
    {
        public bool Equals(ADGroup x, ADGroup y)
        {
            return x.cn == y.cn
                    && x.description == y.description
                    && x.distinguishedName == y.distinguishedName
                    && x.mail == y.mail
                    && x.sAMAccountName == y.sAMAccountName
                    && x.uSNChanged == y.uSNChanged
                    && x.uSNCreated == y.uSNCreated;
        }

        public int GetHashCode(ADGroup obj)
        {
            return obj.uSNCreated.GetHashCode() ^ obj.uSNCreated.GetHashCode();
        }
    }
}
