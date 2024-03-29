using Engine._98.Headers;
using System.DirectoryServices;

namespace Engine._10.CActiveDirectoryMgr
{
    public class CActiveDirectoryMgr : GENERIC_MGR<CActiveDirectoryMgr>
    {
        public string GetCurrentDomainPath()
        {
            //DirectoryEntry de = new DirectoryEntry("LDAP://adtest.com");
            DirectoryEntry de = new DirectoryEntry("LDAP://adtest.com", "administrator", "yonwoo*211013");

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
