using Engine._98.Headers;
using Org.BouncyCastle.Bcpg;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.Reflection;
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
        public List<ADUser> GetADUsers(string? _ldapUrl, string? _username, string? _password)
        {
            DirectoryEntry directoryEntry = new DirectoryEntry(_ldapUrl, _username, _password);
            DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry)
            {
                SearchScope = SearchScope.Subtree,
                Filter = $"(&(objectCategory=person)(objectClass=user))"
            };
            return GetADObjs<ADUser>(directoryEntry, directorySearcher);
        }
        public List<ADUser> GetADUsers()
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
                        if(prop.Name.Equals("lastLogon") 
                            || prop.Name.Equals("lastLogonTimestamp") 
                            || prop.Name.Equals("pwdLastSet"))
                            prop.SetValue(obj, DateTime.FromFileTime((System.Int64)searchResult.Properties[prop.Name][0]));
                        else
                            prop.SetValue(obj, searchResult.Properties[prop.Name][0]);
                    }
                }
                list.Add(obj);
            }

            return list;
        }


        public DataTable GetDataTable<T>(string? _ldapUrl, string? _username, string? _password)
        {
            DirectoryEntry directoryEntry = new DirectoryEntry(_ldapUrl, _username, _password);
            DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry)
            {
                SearchScope = SearchScope.Subtree,
                Filter = $"(&(objectCategory=person)(objectClass=user))"
            };
            return GetDataTable<ADUser>(directoryEntry, directorySearcher);
        }
        public DataTable GetDataTable<T>(DirectoryEntry _directoryEntry, DirectorySearcher _directorySearcher)
        {
            var list = GetADObjs<T>(_directoryEntry, _directorySearcher);
            return ToDataTable(list);
        }
        public DataTable ToDataTable<T>(List<T>? items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                tb.Columns.Add(prop.Name, prop.PropertyType);
            }

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }
    }
}
