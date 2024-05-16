using Engine._98.Headers;
using Org.BouncyCastle.Bcpg;
using System.Collections;
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

            //DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://10.225.88.70", "administrator", "yonwoo*211013");
            DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://10.225.88.70", "administrator", "yonwoo*211013");
            DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry)
            {
                SearchScope = SearchScope.Subtree
                ,
                Filter = $"(&(objectCategory=person)(objectClass=user))"
            };
            directorySearcher.PropertiesToLoad.Add("cn");//
            directorySearcher.PropertiesToLoad.Add("sAMAccountName");//ID를 가져오겠다는 뜻이다.  
            directorySearcher.PropertiesToLoad.Add("mail"); //mail을 가져오겠다는 뜻이다.
            directorySearcher.PropertiesToLoad.Add("usergroup"); //usergroup을 가져오겠다는 뜻이다.
            directorySearcher.PropertiesToLoad.Add("displayname"); //표기이름을 가져오겠다는 뜻이다.
            SearchResultCollection resultCollection = directorySearcher.FindAll();
            List<Dictionary<string, string>> ADUsers = new List<Dictionary<string, string>>(resultCollection.Count);

            foreach (SearchResult searchResult in resultCollection)
            {
                Dictionary<string, string> temp = new Dictionary<string, string>();
                foreach (DictionaryEntry de in searchResult.Properties)
                    temp.Add(de.Key as string, ((ResultPropertyValueCollection)de.Value)[0].ToString());
                ADUsers.Add(temp);
            }


            return "LDAP://" + directoryEntry.Properties["defaultNamingContext"][0].ToString();
        }
        public List<Users> GetADUsers()
        {
            List<Users> lstADUsers = new List<Users>();
            try
            {
                DirectoryEntry searchRoot = new DirectoryEntry("LDAP://10.225.88.70", "administrator", "yonwoo*211013");  //서버도메인 정보, 서버의 계정ID, 계정 PW               
                DirectorySearcher directorySearcher = new DirectorySearcher(searchRoot);
                directorySearcher.Filter = $"(&(objectCategory=person)(objectClass=user))";
                string text = "sAMAccountName"; //ID를 가져오겠다는 뜻이다.                     
                //directorySearcher.PropertiesToLoad.Add("cn");               
                directorySearcher.PropertiesToLoad.Add(text);
                directorySearcher.PropertiesToLoad.Add("mail"); //mail을 가져오겠다는 뜻이다.               
                directorySearcher.PropertiesToLoad.Add("usergroup"); //usergroup을 가져오겠다는 뜻이다.               
                directorySearcher.PropertiesToLoad.Add("displayname"); //표기이름을 가져오겠다는 뜻이다.               
                SearchResultCollection resultCol = directorySearcher.FindAll();
                SearchResult result; 
                if (resultCol != null)
                {
                    for (int counter = 0; counter < resultCol.Count; counter++)
                    {
                        string UserNameEmailString = string.Empty;
                        result = resultCol[counter];
                        if (result.Properties.Contains("samaccountname"))
                        {
                            Users objSurveyUsers = new Users();
                            if (result.Properties["samaccountname"] != null)
                                objSurveyUsers.sAMAccountName = (String)result.Properties["samaccountname"][0];
                            lstADUsers.Add(objSurveyUsers);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("## Get Search AD User List Exception : " + ex.Message);
            }

            return lstADUsers;
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
