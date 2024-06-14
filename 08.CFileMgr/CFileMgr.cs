using Engine._05.CStackTracer;
using Engine._98.Headers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Engine._08.CFileMgr
{
    public class CFileMgr : GENERIC_MGR<CFileMgr>
    {
        public List<string>? GetFileNames()
        {
            System.Text.StringBuilder string_builder_filter = new System.Text.StringBuilder(1024);
            string_builder_filter.Append("All Files(*.txt; *.md; *.jpg; *.gif; *.bmp; *.png)|*.txt;*.md;*.jpg;*.jpeg;*.gif;*.bmp;*.png");
            string_builder_filter.Append("|BMP 파일(*.bmp)|*.bmp|Jpg 파일(*.jpg)|*.jpg|PNG 파일(*.png)|*.png");
            string_builder_filter.Append("|GIF 파일(*.gif)|*.gif");
            string_builder_filter.Append("|txt 파일(*.txt)|*.txt");

            System.Windows.Forms.OpenFileDialog open_file_dlg = new System.Windows.Forms.OpenFileDialog();
            open_file_dlg.Filter = string_builder_filter.ToString();
            //open_file_dlg.InitialDirectory = System.Environment.CurrentDirectory;         //초기경로
            open_file_dlg.RestoreDirectory = true;                 //현재 경로가 이전 경로로 복원되는지 여부          
            open_file_dlg.Multiselect = true;                      //여러파일선택
            open_file_dlg.Title = "등록할 파일을 선택하세요.";
            if (System.Windows.Forms.DialogResult.OK == open_file_dlg.ShowDialog())
            {
                if (false == File.Exists(open_file_dlg.FileNames[0]))
                    return null; //파일이 없으면 그냥 나감 

                var list_file_name = open_file_dlg.FileNames;
                return (from element in list_file_name select element).ToList<string>();
            }
            return null;
        }
        public List<string>? GetFileNames(string _rootPath)
        {
            var directoryInfo = new DirectoryInfo(_rootPath);
            var list = directoryInfo.GetFiles().ToList();
            List<string> fileNames = new List<string>(list.Count);
            list.ForEach(file => { fileNames.Add(file.FullName); });
            return fileNames;
        }
        public List<string>? GetRecursiveFileNames(string _rootDirPath)
        {
            var directories = Directory.EnumerateDirectories(_rootDirPath, "*.*", SearchOption.AllDirectories).ToList();
            List<string> dirPaths = new List<string>();
            foreach (var _dir in directories)
            {
                DirectoryInfo _dirInfo = new DirectoryInfo(_dir);
                var files = _dirInfo.GetFiles().ToList();
                if (files.Count != 0)
                    dirPaths.Add(_dir); //파일을 가지고 있는 디렉토리만 추가
            }
            return dirPaths;
        }

        #region txt, md 파일 관련
        public IEnumerable<string> GetFileTextIter(string _path)
        {
            if (false == File.Exists(_path))
            {
                return null;
            }

            using (var fileStream = new FileStream(_path, FileMode.Open))
            {
                using (var binaryReader = new BinaryReader(fileStream))
                {

                }
            }

            return System.IO.File.ReadLines(_path);
        }
        public string? GetFileText(string _path)
        {
            if (false == File.Exists(_path))
                return "";
            return System.IO.File.ReadAllText(_path);
        }
        public async void WriteTextFile(string _path, string _txt)
        {
            await File.WriteAllTextAsync(_path, _txt);
        }
        /// <summary>
        /// 텍스트 파일 인코딩 구하기
        /// https://icodebroker.tistory.com/4831
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Encoding GetTextFileEncoding(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath, Encoding.Default, true))
            {
                reader.Peek();

                Encoding encoding = reader.CurrentEncoding;

                return encoding;
            }
        }
        #endregion

        #region 데이터테이블 TO EXCEL
        /// <summary>
        /// 사용법: WriteDataTableToExcel(데이터테이블, Application.StartupPath + @"\데이터테이블.csv");
        /// </summary>
        /// <param name="_dt"></param>
        /// <param name="_path"></param>
        /// <param name="_separator"></param>
        public void WriteDataTableToCsv(DataTable _dt, string _path, string _separator = ",")
        {
#if !DEBUG
            return;
#endif

            if (_dt == null)
                return;
            string[] columnNames = _dt.Columns
                .Cast<DataColumn>()
                .Select(column => column.ColumnName)
                .ToArray();

            var lines = new List<string>();
            lines.Add(string.Join(_separator, columnNames.Select(_name => $"\"{_name}\"")));

            var valueLines = _dt.AsEnumerable().Select(_dr => {
                var str = "";
                if (_dr.RowState != DataRowState.Deleted)
                    str = string.Join(_separator, _dr.ItemArray.Select(_obj => $"\"{_obj}\""));
                return str;
            });
            lines.AddRange(valueLines);

            try
            {
                File.WriteAllLines(_path, lines, System.Text.Encoding.Default);
            }
            catch (Exception _e)
            {
                CStackTracer.GetInstance().WriteTraceInfo(_e.Message);
            }
        }
        public DataTable ReadCsvToDataTable()
        {
            DataTable dt = new DataTable();

            return null;
        }
        #endregion
    }
}
