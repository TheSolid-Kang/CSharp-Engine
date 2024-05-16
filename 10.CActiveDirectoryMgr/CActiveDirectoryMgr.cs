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
            DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://10.225.88.70", "administrator", "yonwoo*211013");
            DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry)
            {
                SearchScope = SearchScope.Subtree,
                Filter = $"(&(objectCategory=person)(objectClass=user))"
            };
            return GetADObjs<Users>(directoryEntry, directorySearcher);
        }
        /// <summary>
        /// AD 객체(User, Group)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private List<T> GetADObjs<T>(DirectoryEntry _directoryEntry, DirectorySearcher _directorySearcher)
        {
            List<T> list = new List<T>();

            try
            {
                //AD User 가져오기
                T obj = default(T);
                obj = System.Activator.CreateInstance<T>();
                foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties())
                    _directorySearcher.PropertiesToLoad.Add(prop.Name);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("## Get Search AD List Exception : " + ex.Message);
            }

            SearchResultCollection resultCollection = _directorySearcher.FindAll();
            list = new List<T>(resultCollection.Count);
            foreach (SearchResult searchResult in resultCollection)
            {
                T obj = System.Activator.CreateInstance<T>();
                foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if(true == searchResult.Properties.Contains(prop.Name))
                        prop.SetValue(obj, searchResult.Properties[prop.Name][0]);
                }
                list.Add(obj);
            }

            return list;
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
