using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Engine._03.CFTPMgr
{
  public class CFtpInfo
  {
    public CFtpInfo()
    {
      this._ipAddress = "127.0.0.1"; //IP 주소
      this._port = "21"; //FTP 접속 _port
      this._user = "접속계정"; //FTP 접속 계정
      this._password = "비밀번호"; //FTP 계정 비밀번호
      this.server_path = @"www/data/";
    }
    public CFtpInfo(string _ipAddress, string _port, string _user, string _password)
    {
      this._ipAddress = _ipAddress;
      this._port = _port;
      this._user = _user;
      this._password = _password;
      server_path = @"www/data/";
    }

    public string _ipAddress { get; set; }
    public string _user { get; set; }
    public string _password { get; set; }
    public string _port { get; set; }

    public string server_path { get; set; }
  }
  public class CFTPMgr : IDisposable
  {
    #region DEFAULT (생성자/ 홀더/ Instance)
    private CFTPMgr() { }
    private static readonly Lazy<CFTPMgr> _instance = new Lazy<CFTPMgr>(() => new CFTPMgr());
    public static CFTPMgr GetInstance() => _instance.Value;
    #endregion


    #region 제어용 멤버변수
    public bool isOn = true;
    #endregion

    #region FTP 멤버변수
    public delegate void ExceptionEventHandler(string LocationID, Exception ex);
    public event ExceptionEventHandler ExceptionEvent;
    public Exception LastException = null;

    public bool IsConnected { get; set; }
    private CFtpInfo _ftpInfo = new CFtpInfo();
    #endregion


    #region 멤버함수 선언부
    //1. 서버 연결
    public bool ConnectToServer() => _ConnectToServer();
    public bool ConnectToServer(string ip, string _port, string _userId, string _password) => _ConnectToServer(ip, _port, _userId, _password);

    //2. 서버에 파일 업로드 
    public bool UpLoad(string _serverDirPath, string _localFilePath) => _UpLoad(_serverDirPath, _localFilePath);
    public void MakeDirectory(string _serverDirPath) => _MakeDirectory(_serverDirPath);

    //3. 서버에서 파일 다운로드
    public bool DownLoad(string _localFilePath, string _serverDirPath) => _DownLoad(_localFilePath, _serverDirPath);
    public void CheckDirectory(string _localFilePath) => _CheckDirectory(_localFilePath);

    //4. 서버의 해당 폴더에 담긴 파일 리스트 가져옴
    public List<string> GetFileList(string _serverDirPath) => _GetFileList(_serverDirPath);

    //5. 파일 다이어로그 출력_여러 파일 선택 가능
    public List<string> OpenDlg() => open_dlg();

    //7. 서버의 특정 폴더에 저장된 파일 삭제  
    public bool DeleteSavedFileInServer(string _serverFilePath) => _DeleteSavedFileInServer(_serverFilePath);
    //7_2. 서버의 특정 폴더에 저장된 파일 전체 삭제  
    public bool DeleteAllSavedFileInServer(string _serverDirPath) => _DeleteAllSavedFileInServer(_serverDirPath);
    #endregion


    #region 멤버함수 정의부

    //1. 서버 연결
    private bool _ConnectToServer() => _ConnectToServer(_ftpInfo._ipAddress, _ftpInfo._port, _ftpInfo._user, _ftpInfo._password);
    private bool _ConnectToServer(string ip, string _port, string _userId, string _password)
    {
      this.IsConnected = false;
      this._ftpInfo._ipAddress = ip;
      this._ftpInfo._port = _port;
      this._ftpInfo._user = _userId;
      this._ftpInfo._password = _password;
      
      string url = string.Format(@"FTP://{0}:{1}/", this._ftpInfo._ipAddress, this._ftpInfo._port);

      try
      {
        FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(url);
        ftpRequest.Credentials = new NetworkCredential(_userId, _password);
        ftpRequest.KeepAlive = false;
        ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
        ftpRequest.UsePassive = false;

        using (ftpRequest.GetResponse())
        {

        }

        this.IsConnected = true;
      }

      catch (Exception ex)
      {
        MessageBox.Show($"FTP 접속 실패 \n{ex.Message}");
        this.LastException = ex;
        System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
        string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);

        if (this.ExceptionEvent != null)
          this.ExceptionEvent(id, ex);

        return false;
      }
      return true;
    }

    //2. 서버에 파일 업로드 
    private bool _UpLoad(string _serverDirPath, string _localFilePath)
    {
      try
      {
        _MakeDirectory(_serverDirPath); //폴더가 없을 수도 있으니 확인
        FileInfo fileInf = new FileInfo(_localFilePath);
        _serverDirPath = _serverDirPath.Replace('\\', '/'); //역슬래시 -> 슬래시
        _serverDirPath = _serverDirPath.Replace("//", "/"); //역슬래시 -> 슬래시
        _localFilePath = _localFilePath.Replace('\\', '/'); //역슬래시 -> 슬래시

        string url = "";
        if (!_serverDirPath[_serverDirPath.Length - 1].Equals('/'))
          url = string.Format(@"FTP://{0}:{1}/{2}", this._ftpInfo._ipAddress, this._ftpInfo._port, _serverDirPath);
        else
          url = string.Format(@"FTP://{0}:{1}/{2}{3}", this._ftpInfo._ipAddress, this._ftpInfo._port, _serverDirPath, fileInf.Name);

        FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(url);
        ftpRequest.Credentials = new NetworkCredential(_ftpInfo._user, _ftpInfo._password); //쓰기 권한이 있는 FTP 사용자 로그인 지정
        ftpRequest.KeepAlive = false;
        ftpRequest.UseBinary = false;
        ftpRequest.UsePassive = false;
        ftpRequest.Method = WebRequestMethods.Ftp.UploadFile; //FTP 업로드 한다는 것을 표기
        ftpRequest.ContentLength = fileInf.Length;

        int buffLength = 2048;
        byte[] buff = new byte[buffLength]; //입력 파일을 BYTE 배열로 읽음.

        using (FileStream fs = fileInf.OpenRead())
        {
          using (Stream strm = ftpRequest.GetRequestStream())
          {
            int contentLen = fs.Read(buff, 0, buffLength);
            while (contentLen != 0)
            {
              strm.Write(buff, 0, contentLen);
              contentLen = fs.Read(buff, 0, buffLength);
            }
          }
          fs.Flush();
          fs.Close();
        }
        if (buff != null)
        {
          Array.Clear(buff, 0, buff.Length);
          buff = null;
        }
        Console.WriteLine("FTP Upload 완료");
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Upload 실패 \n{ex.Message}");
        this.LastException = ex;
        System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
        string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);
        if (this.ExceptionEvent != null)
          this.ExceptionEvent(id, ex);
        return false;
      }
      return true;
    }

    //  1) 서버에 파일 업로드 할 때 사용
    //서버에 폴더가 없다면 폴더를 만든다.
    private void _MakeDirectory(string _serverDirPath)
    {
      if (!_serverDirPath[_serverDirPath.Length - 1].Equals('/'))
        _serverDirPath = _serverDirPath.Remove(_serverDirPath.LastIndexOf("/") + 1);

      string[] arr_directory = _serverDirPath.Split('/');
      string currentDir = string.Empty;
      try
      {
        foreach (string tmpFoler in arr_directory)
        {
          try
          {
            if (tmpFoler == string.Empty)
              continue;
            currentDir += "/" + tmpFoler;
            string url = string.Format(@"FTP://{0}:{1}{2}", this._ftpInfo._ipAddress, this._ftpInfo._port, currentDir);
            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(url);
            ftpRequest.Credentials = new NetworkCredential(_ftpInfo._user, _ftpInfo._password);
            ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            ftpRequest.KeepAlive = false;
            ftpRequest.UsePassive = false;

            FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();

            response.Close();
          }
          catch (Exception _e)
          {
            //MessageBox.Show(_e.Message);
          }
        }
      }
      catch (Exception ex)
      {
        this.LastException = ex;
        System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
        string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);
        if (this.ExceptionEvent != null)
          this.ExceptionEvent(id, ex);
      }
    }


    //3. 서버에서 파일 다운로드
    private bool _DownLoad(string _localFilePath, string _serverDirPath)
    {
      try
      {
        _CheckDirectory(_localFilePath);
        _localFilePath += @"\" + _serverDirPath.Substring(_serverDirPath.LastIndexOf('/') + 1);


        string url = string.Format(@"FTP://{0}:{1}/{2}", this._ftpInfo._ipAddress, this._ftpInfo._port, _serverDirPath);
        FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(url); // "FTP://" 이기에 FtpWebRequest로 캐스팅 한 것
        ftpRequest.Credentials = new NetworkCredential(_ftpInfo._user, _ftpInfo._password);
        ftpRequest.KeepAlive = false;
        ftpRequest.UseBinary = true;
        ftpRequest.UsePassive = false;

        using (FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse())
        {
          using (FileStream outputStream = new FileStream(_localFilePath, FileMode.Create, FileAccess.Write))
          {
            using (Stream ftpStream = response.GetResponseStream())
            {
              int bufferSize = 2048;
              int readCount;
              byte[] buffer = new byte[bufferSize];

              readCount = ftpStream.Read(buffer, 0, bufferSize);
              while (readCount > 0)
              {
                outputStream.Write(buffer, 0, readCount);
                readCount = ftpStream.Read(buffer, 0, bufferSize);
              }
              ftpStream.Close();
              outputStream.Close();
              if (buffer != null)
              {
                Array.Clear(buffer, 0, buffer.Length);
                buffer = null;
              }
            }
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Download Fail! \n{ex.Message}");
        this.LastException = ex;
        if (_serverDirPath.Contains(@"\ZOOM\") == true)
          return false;
        System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
        string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);
        if (this.ExceptionEvent != null)
          this.ExceptionEvent(id, ex);
        return false;
      }
    }
    //  1) 서버에서 파일 다운로드 할 때 사용
    //로컬에 폴더가 없다면 폴더를 만든다.
    private void _CheckDirectory(string _localFilePath)
    {
      FileInfo fInfo = new FileInfo(_localFilePath);
      if (!fInfo.Exists)
      {
        DirectoryInfo dInfo = new DirectoryInfo(fInfo.DirectoryName);
        if (!dInfo.Exists)
          dInfo.Create();
      }
    }

    //4. 해당 폴더에 담긴 파일 리스트 가져옴
    private List<string> _GetFileList(string _serverDirPath)
    {
      List<string> resultList = new List<string>();
      StringBuilder result = new StringBuilder();
      try
      {
        string url = string.Format(@"FTP://{0}:{1}/{2}", this._ftpInfo._ipAddress, this._ftpInfo._port, _serverDirPath);
        FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(url);
        ftpRequest.Credentials = new NetworkCredential(_ftpInfo._user, _ftpInfo._password);
        ftpRequest.KeepAlive = false;
        ftpRequest.UseBinary = false;
        ftpRequest.UsePassive = false;
        ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

        using (WebResponse response = ftpRequest.GetResponse())
        {
          StreamReader reader = new StreamReader(response.GetResponseStream());
          string line = reader.ReadLine();
          while (line != null)
          {
            result.Append(line);
            result.Append("\n");
            line = reader.ReadLine();
          }
          result.Remove(result.ToString().LastIndexOf('\n'), 1);
          if (reader != null)
            reader.Close();
          foreach (string file in result.ToString().Split('\n'))
            resultList.Add(file);
        }
        return resultList;
      }
      catch (Exception ex)
      {
        //MessageBox.Show(ex.Message);
        this.LastException = ex;
        System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
        string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);
        if (this.ExceptionEvent != null)
          this.ExceptionEvent(id, ex);
        return resultList;
      }
    }

    //5. 파일 다이어로그 출력_여러 파일 선택 가능
    private List<string>? open_dlg()
    {
      StringBuilder string_builder_filter = new StringBuilder(1024);
      string_builder_filter.Append("Image Files(*.jpg; *.gif; *.bmp; *.png)|*.jpg;*.jpeg;*.gif;*.bmp;*.png");
      string_builder_filter.Append("|BMP 파일(*.bmp)|*.bmp|Jpg 파일(*.jpg)|*.jpg|PNG 파일(*.png)|*.png");
      string_builder_filter.Append("|GIF 파일(*.gif)|*.gif");
      string_builder_filter.Append("|txt 파일(*.txt)|*.txt");

      System.Windows.Forms.OpenFileDialog open_file_dlg = new System.Windows.Forms.OpenFileDialog();
      //open_file_dlg.InitialDirectory = System.Environment.CurrentDirectory;         //초기경로
      open_file_dlg.RestoreDirectory = true;                 //현재 경로가 이전 경로로 복원되는지 여부          
      open_file_dlg.Filter = string_builder_filter.ToString();
      open_file_dlg.Title = "등록할 파일을 선택하세요.";
      open_file_dlg.Multiselect = true;                      //여러파일선택

      if (System.Windows.Forms.DialogResult.OK == open_file_dlg.ShowDialog())
      {
        string file_name = open_file_dlg.FileName; //Dialog의 결과이기에 이곳에서만 원본 이미지 파일의 경로를 참조할 수 있다.
        if (false == File.Exists(file_name))
          return null;

        var list_file_name = open_file_dlg.FileNames;
        return (from element in list_file_name select element).ToList<string>();
      }
      return null;
    }

    //7. 서버의 특정 폴더에 저장된 파일 삭제 
    private bool _DeleteSavedFileInServer(string _serverFilePath)
    {
      bool is_delete = false;
      try
      {
        string url = string.Format(@"FTP://{0}:{1}/{2}", this._ftpInfo._ipAddress, this._ftpInfo._port, _serverFilePath);
        FtpWebRequest? ftpRequest = (FtpWebRequest)WebRequest.Create(url);
        ftpRequest.Credentials = new NetworkCredential(_ftpInfo._user, _ftpInfo._password);
        ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;

        using (WebResponse response = ftpRequest.GetResponse())
        {

        }
        is_delete = true;
        ftpRequest = null;
      }
      catch (Exception _e)
      {
        MessageBox.Show(_e.Message);
      }
      return is_delete;
    }
    //7_2. 서버의 특정 폴더에 저장된 파일 전체 삭제 
    private bool _DeleteAllSavedFileInServer(string _serverDirPath)
    {
      Action<string> action = (string __serverDirPath) => _DeleteSavedFileInServer(__serverDirPath);
      var list_serverFilePath = (from element in GetFileList(_serverDirPath) select element.Insert(0, _serverDirPath)).ToList();
      var list_thread = new List<Thread>();
      list_serverFilePath.ForEach(element => list_thread.Add(new Thread(() => action(element))));
      list_thread.ForEach(element => element.Start());
      return true;
    }

    public void Dispose()
    {

    }



    #endregion


  }

}
