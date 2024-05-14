using Engine._98.Headers;
using System.DirectoryServices;
using System.Reflection.PortableExecutable;
using DirectoryEntry = System.DirectoryServices.DirectoryEntry;

namespace Engine._10.CActiveDirectoryMgr
{
    public class CActiveDirectoryMgr : GENERIC_MGR<CActiveDirectoryMgr>
    {
        /*
            CN      commonName
            L       localityName
            ST      stateOrProvinceName
            O       organizationName
            OU      organizationalUnitName
            C       countryName
            STREET  streetAddress
            DC      domainComponent
            UID     userid
         */
        public string GetCurrentDomainPath()
        {
            //CN=김동혁(DONGHYEOK KIM),OU=HR계정,OU=Users,OU=_Yonwoo,DC=adtest,DC=com
            //adtest.com/_Yonwoo/Users/HR계정


            //DirectoryEntry de = new DirectoryEntry("LDAP://adtest.com");
            //DirectoryEntry de = new DirectoryEntry("LDAP://adtest.com:3389");
            //DirectoryEntry de = new DirectoryEntry("LDAP://adtest.com", "administrator", "yonwoo*211013");

            //DirectoryEntry de = new DirectoryEntry("LDAP://CN=Person,CN=Schema,CN=Configuration,DC=adtest,DC=com/Users");
            //DirectoryEntry de = new DirectoryEntry("LDAP://adtest.com/_Yonwoo/Users/HR계정/");

            DirectoryEntry de = new DirectoryEntry("LDAP://10.225.88.70", "administrator", "yonwoo*211013");
            //DirectoryEntry de = new DirectoryEntry("LDAP://10.225.88.70:3389");
            //DirectoryEntry de = new DirectoryEntry("LDAP://10.225.88.70:3389", "administrator", "yonwoo*211013");
            //DirectoryEntry de = new DirectoryEntry("LDAP://[10.225.88.70]:[3389]");
            //DirectoryEntry de = new DirectoryEntry("LDAP://[10.225.88.70]:[3389]", "administrator", "yonwoo*211013");
            //DirectoryEntry de = new DirectoryEntry("LDAP://[10.225.88.70]:[3389]", "administrator", "yonwoo*211013");
            //DirectoryEntry de = new DirectoryEntry("LDAP://10.225.88.70:3389.com", "administrator", "yonwoo*211013");
            //DirectoryEntry de = new DirectoryEntry("LDAP://[10.225.88.70]:[3389].com", "administrator", "yonwoo*211013");
            var temp = de.Children;
            DirectorySearcher search = new DirectorySearcher(de);


            return "LDAP://" + de.Properties["defaultNamingContext"][0].ToString();
        }
        void AccessADEntryTest()
        {
            // 사용자의 DirectoryEntry 가져오기
            DirectoryEntry userEntry = new DirectoryEntry("LDAP://CN=John Doe,CN=Users,DC=domain,DC=com");

            // Active Directory에서 사용자 찾기
            DirectorySearcher searcher = new DirectorySearcher(userEntry);
            searcher.Filter = "(samaccountname=johndoe)";
            SearchResult result = searcher.FindOne();

            if (result != null)
            {
                DirectoryEntry foundUser = result.GetDirectoryEntry();
                // 사용자 속성에 액세스
                string username = foundUser.Properties["samaccountname"].Value.ToString();
            }
        }
        void AccessADEntry(string _url, string _ldapId, string _ldapPwd, string _domain, string _userId, string _userPwd)
        {
            // 사용자의 DirectoryEntry 가져오기
            DirectoryEntry userEntry = new DirectoryEntry("LDAP://CN=John Doe,CN=Users,DC=domain,DC=com");

            // Active Directory에서 사용자 찾기
            DirectorySearcher searcher = new DirectorySearcher(userEntry);
            searcher.Filter = "(samaccountname=johndoe)";
            SearchResult result = searcher.FindOne();

            if (result != null)
            {
                DirectoryEntry foundUser = result.GetDirectoryEntry();
                // 사용자 속성에 액세스
                string username = foundUser.Properties["samaccountname"].Value.ToString();
            }
        }
    }
}
