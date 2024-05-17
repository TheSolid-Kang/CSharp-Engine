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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Users> GetADUsers(string? _ldapUrl, string? _username, string? _password)
        {
            DirectoryEntry directoryEntry = new DirectoryEntry(_ldapUrl, _username, _password);
            DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry)
            {
                SearchScope = SearchScope.Subtree,
                Filter = $"(&(objectCategory=person)(objectClass=user))"
            };
            return GetADObjs<Users>(directoryEntry, directorySearcher);
        }
        public List<Users> GetADUsers()
        {
            return GetADUsers("LDAP://10.225.88.70", "administrator", "yonwoo*211013");
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
                T? obj = default(T);
                obj = System.Activator.CreateInstance<T>();
                foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties())
                    _directorySearcher?.PropertiesToLoad.Add(prop.Name);
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
                    if (true == searchResult.Properties.Contains(prop.Name))
                    {
                        if(prop.Name.Equals("lastLogon") || prop.Name.Equals("lastLogonTimestamp") || prop.Name.Equals("pwdLastSet"))
                            prop.SetValue(obj, DateTime.FromFileTime((System.Int64)searchResult.Properties[prop.Name][0]));
                        else
                            prop.SetValue(obj, searchResult.Properties[prop.Name][0]);
                    }
                }
                list.Add(obj);
            }

            return list;
        }

    }
}
