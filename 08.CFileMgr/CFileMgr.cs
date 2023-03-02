using Engine._98.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Engine._08.CFileMgr
{
    public class CFileMgr : GENERIC_MGR<CFileMgr>
    {
        public List<string>? GetFileNames(string _path)
        {
            System.Text.StringBuilder string_builder_filter = new System.Text.StringBuilder(1024);
            string_builder_filter.Append("Image Files(*.jpg; *.gif; *.bmp; *.png)|*.jpg;*.jpeg;*.gif;*.bmp;*.png");
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

        #region txt, md 파일 관련
        public IEnumerable<string> GetFileTextIter(string _path)
        {
            if (false == File.Exists(_path))
                return null;
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
        #endregion
    }
}
