using Engine._98.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine._02.KMP
{
  public class _KMP : GENERIC_MGR<_KMP>
  {

    private List<int> get_pi(string _strSearchKey)
    {
      int j = 0;
      int search_size = (int)_strSearchKey.Length;
      List<int> list_pi = new List<int>(search_size);

      for (int i = 0; i < search_size; i++)
        list_pi.Add(0);

      for (int i = 1; i < search_size; i++)
      {
        while (j > 0 && _strSearchKey[i] != _strSearchKey[j])
          j = list_pi[j - 1];
        if (_strSearchKey[i] == _strSearchKey[j])
          list_pi[i] = ++j;
      }

      return list_pi;
    }
    public List<int> get_searched_address(string _strText, string _strSearchKey)
    {
      int j = 0;
      int iTextSize = (int)_strText.Length;
      int iSearchKeySize = (int)_strSearchKey.Length;

      List<int> list_pi = get_pi(_strSearchKey);
      List<int> list_addr = new List<int>();

      for (int i = 0; i < iTextSize; ++i)
      {
        while (j > 0 && _strText[i] != _strSearchKey[j])
          j = list_pi[j - 1];
        if (_strText[i] == _strSearchKey[j])
        {
          if (j == iSearchKeySize - 1)
          {
            list_addr.Add(i - iSearchKeySize + 1);
            j = list_pi[j];
          }
          else
            ++j;
        }
      }

      return list_addr;
    }

  }
}
