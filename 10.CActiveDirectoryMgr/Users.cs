using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine._10.CActiveDirectoryMgr
{
    public class Users
    {
        public string cn { get; set; }
        public string mail { get; set; }
        public string sAMAccountName {  get; set; }
        public string userGroup { get; set; }
        public string displayName { get; set; }
    }
}
