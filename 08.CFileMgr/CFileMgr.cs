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
        /// <summary>
        /// 파일 선택 대화 상자를 통해 사용자가 선택한 파일들의 이름을 반환합니다.
        /// 
        /// HOW:
        /// 1. 파일 필터를 설정합니다.
        /// 2. OpenFileDialog를 사용하여 파일 선택 대화 상자를 표시합니다.
        /// 3. 사용자가 파일을 선택하면, 선택된 파일들의 이름을 List<string> 형태로 반환합니다.
        /// 4. 파일이 선택되지 않았거나 존재하지 않으면 null을 반환합니다.
        /// </summary>
        /// <returns>선택된 파일들의 이름을 담은 List<string> 객체 또는 null</returns>
        public List<string>? GetFileNames()
        {
            // 파일 필터 설정
            var filter = new StringBuilder(1024)
                .Append("All Files(*.txt; *.md; *.jpg; *.gif; *.bmp; *.png)|*.txt;*.md;*.jpg;*.jpeg;*.gif;*.bmp;*.png")
                .Append("|BMP 파일(*.bmp)|*.bmp|Jpg 파일(*.jpg)|*.jpg|PNG 파일(*.png)|*.png")
                .Append("|GIF 파일(*.gif)|*.gif")
                .Append("|txt 파일(*.txt)|*.txt")
                .ToString();

            // OpenFileDialog 설정
            var openFileDialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = filter,
                RestoreDirectory = true, // 현재 경로가 이전 경로로 복원되는지 여부
                Multiselect = true,      // 여러 파일 선택 가능
                Title = "등록할 파일을 선택하세요."
            };

            // 파일 대화 상자를 표시하고 사용자가 파일을 선택했는지 확인
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // 선택된 파일이 존재하는지 확인
                if (!File.Exists(openFileDialog.FileNames[0]))
                    return null; // 파일이 없으면 null 반환

                // 선택된 파일들의 이름을 리스트로 반환
                return openFileDialog.FileNames.ToList();
            }

            return null; // 파일을 선택하지 않았으면 null 반환
        }

        /// <summary>
        /// 지정된 디렉토리 내의 모든 파일 이름을 반환합니다.
        /// 
        /// HOW:
        /// 1. 지정된 경로의 DirectoryInfo 객체를 생성합니다.
        /// 2. DirectoryInfo 객체를 사용하여 디렉토리 내의 모든 파일 정보를 가져옵니다.
        /// 3. 각 파일의 전체 경로를 List<string>에 추가합니다.
        /// 4. 파일 이름 목록을 반환합니다.
        /// </summary>
        /// <param name="_rootPath">파일 이름을 조회할 디렉토리의 경로</param>
        /// <returns>디렉토리 내의 모든 파일 이름을 담은 List<string> 객체 또는 null</returns>
        public List<string>? GetFileNames(string _rootPath)
        {
            var directoryInfo = new DirectoryInfo(_rootPath);
            var list = directoryInfo.GetFiles().ToList();
            List<string> fileNames = new List<string>(list.Count);
            list.ForEach(file => { fileNames.Add(file.FullName); });
            return fileNames;
        }

        /// <summary>
        /// 지정된 디렉토리와 하위 디렉토리에서 파일이 포함된 디렉토리의 경로를 반환합니다.
        /// 
        /// HOW:
        /// 1. 지정된 경로의 모든 하위 디렉토리를 열거합니다.
        /// 2. 각 디렉토리 내에 파일이 있는지 확인합니다.
        /// 3. 파일이 있는 디렉토리의 경로를 리스트에 추가합니다.
        /// 4. 파일이 포함된 디렉토리 경로 목록을 반환합니다.
        /// </summary>
        /// <param name="_rootDirPath">디렉토리 경로</param>
        /// <returns>파일이 포함된 디렉토리 경로를 담은 List<string> 객체 또는 null</returns>
        public List<string>? GetRecursiveFileNames(string _rootDirPath)
        {
            var directories = Directory.EnumerateDirectories(_rootDirPath, "*.*", SearchOption.AllDirectories).ToList();
            List<string> dirPaths = new List<string>();
            foreach (var dir in directories)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                if (dirInfo.GetFiles().Any())
                {
                    dirPaths.Add(dir); // 파일이 있는 디렉토리만 추가
                }
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
