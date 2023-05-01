using Engine._05.CStackTracer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine._01.DAO
{
  public class MSSQL_DAO : IDisposable
  {
    public MSSQL_DAO() 
    {
      StringBuilder strBuil = new StringBuilder(1024);
      strBuil.Append($"Data Source={DB_HOST},{DB_PORT};");//Data Source는 MSSQL 서버의 주소
      strBuil.Append($"Initial Catalog={DB_NAME};");//Initial Catalog는 사용할 데이터베이스의 이름을 나타냅니다. 
      strBuil.Append($"User ID={DB_USER};");
      strBuil.Append($"Password={DB_PASS};");
      strBuil.Append($"Integrated Security=True;");//Integrated Security=True는 Windows 인증을 사용하여 로그인하는 것을 의미합니다. 
      strBuil.Append($"Connect Timeout=30;");//Connect Timeout은 연결 시도를 중단하고 예외를 발생시키기 전에 대기할 시간을 나타냅니다.
      strBuil.Append($"Encrypt=False;");
      strBuil.Append($"TrustServerCertificate=False;");
      strBuil.Append($"ApplicationIntent=ReadWrite;");
      strBuil.Append($"MultiSubnetFailover=False");
      url = strBuil.ToString();//url 변수에는 연결할 MSSQL 데이터베이스의 정보가 포함되어 있습니다.
    }


    ~MSSQL_DAO()
    {
      Dispose();
    }
    private const string DB_HOST = "localhost";
    private const string DB_PORT = "1433";
    private const string DB_USER = "root";
    private const string DB_PASS = "root";
    private const string DB_NAME = "TwoMites";
    private const string CHAR_SET = "UTF8";

    private string host;
    private string port;
    private string user;
    private string pass;
    private string name;

    private string url;
    public object result;
    private DataTable _dataTable = null;
    public DataTable m_dataTable
    {
      get
      {
        var dataTable = _dataTable;
        return dataTable;
      }
      set
      {
        if (_dataTable != null)
          _dataTable.Dispose();
        _dataTable = value;
      }
    }
    public void Dispose()
    {

    }

    public DataTable GetDataTable(string _query) => _GetDataTable( _query );

    private DataTable _GetDataTable(string _query)
    {
      try
      {
        using (var connection = new SqlConnection(url))
        using (var command = new SqlCommand(_query, connection))
        {
          connection.Open();
          using (SqlDataReader reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              Console.WriteLine("{0} {1}", reader.GetInt32(0), reader.GetString(1));
            }
          }
        }
      }
      catch (Exception _e)
      {
        CStackTracer.GetInstance().WriteTraceInfo("DB GetDataTable 에러: " + _e.Message);
        System.Diagnostics.Debug.WriteLine($"예외 == {_e.Message}");
      }
      return null;
    }
    
  }
}
