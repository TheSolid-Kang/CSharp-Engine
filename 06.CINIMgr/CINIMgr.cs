using Engine._98.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine._06.CINIMgr
{
  public class CINIMgr : GENERIC_MGR<CINIMgr>
  {
    public CINIMgr() => CreateINIFile("config.ini"); 

    private string _inipath = string.Empty;
    private string _file_name = "config.ini";
    private const int DEFAULT_CAP = 1024;

    [DllImport("kernel32")]
    private static extern long WritePrivateProfileString(string _section, string _key, string _value, string _filePath);
    public void write_ini(string _section, string _key, string _value) => WritePrivateProfileString(_section, _key, _value, _inipath); 

    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string _section, string _key, string _def, StringBuilder _str_buil, int _size, string _filePath);
    public string reade_ini(string _section, string _key, string _def)
    {
      StringBuilder str_buil = new StringBuilder(1024);
      GetPrivateProfileString(_section, _key, _def, str_buil, DEFAULT_CAP, _inipath);
      return str_buil.ToString().Trim();
    }

    public void SetFileName(string _name) { _file_name = _name; _inipath = System.IO.Directory.GetCurrentDirectory()  + "\\"+ _file_name; }
    public void CreateINIFile(string _name) { 
      SetFileName(_name);
      if (!File.Exists(_inipath))//파일이 없다면 만들어라.
        File.Create(_inipath);
    }
  }
}
