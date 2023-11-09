using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Engine._03.CFTPMgr
{
    public class CSFTPMgr
    {
        private static string _ip = "192.168.211.9";
        private static int _port = 22;
        private static string _id = "root";
        private static string _pwd = "yonwoo*0619!@";

        public static IEnumerable<ISftpFile> GetSftpFiles(string _remoteDirPath)
        {
            var client = CSFTPMgr.GetSftpClient;
            client.Connect();
            client.ChangeDirectory(_remoteDirPath);
            if (false == client.IsConnected)
                return null;

            var iterable = client.ListDirectory(_remoteDirPath);

            client.Disconnect();
            client.Dispose();
            return iterable;
        }

        private static SftpClient GetSftpClient => new SftpClient(_ip, _port, _id, _pwd);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath">리눅스 서버 경로</param>
        /// <param name="localPath">저장 할 윈도우 서버 경로</param>
        public static void DownloadFiles(string remotePath, string localPath)
        {
            var localDir = localPath;
            var remoteDir = remotePath;

            using (SftpClient client = new SftpClient(_ip, _port, _id, _pwd))
            {
                client.Connect();
                client.ChangeDirectory(remoteDir);
                if (false == client.IsConnected)
                {
                    return;
                }
                DownloadDirectory(client, remoteDir, localDir);
                client.Disconnect();
            }
        }


        public static void DownloadDirectory(SftpClient client, string remotePath, string localPath)
        {
            Directory.CreateDirectory(localPath);
            var files = client.ListDirectory(remotePath);
            try
            {
                foreach (SftpFile file in files)
                {
                    if ((file.Name != ".") && (file.Name != ".."))
                    {
                        string sourceFilePath = remotePath + "/" + file.Name;
                        string destFilePath = Path.Combine(localPath, file.Name);
                        if (file.IsDirectory)
                        {
                            DownloadDirectory(client, sourceFilePath, destFilePath);
                        }
                        else
                        {
                            using (Stream fileStream = File.Create(destFilePath))
                            {
                                client.DownloadFile(sourceFilePath, fileStream);
                            }
                        }
                    }
                }
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message);
                System.Diagnostics.Debug.WriteLine(_e.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath">리눅스 서버 경로</param>
        /// <param name="localPath">저장 할 윈도우 서버 경로</param>
        public static void DownloadFiles(string remotePath, string localPath, Dictionary<string, ISftpFile> keyValuePairs)
        {
            var localDir = localPath;
            var remoteDir = remotePath;

            using (SftpClient client = new SftpClient(_ip, _port, _id, _pwd))
            {
                client.Connect();
                client.ChangeDirectory(remoteDir);
                if (false == client.IsConnected)
                {
                    return;
                }
                DownloadDirectory(client, remoteDir, localDir);
                client.Disconnect();
            }
        }
        public static void DownloadDirectory(SftpClient client, string remotePath, string localPath, Dictionary<string, ISftpFile> keyValuePairs)
        {
            Directory.CreateDirectory(localPath);

            try
            {
                foreach (SftpFile file in keyValuePairs.Values)
                {
                    if ((file.Name != ".") && (file.Name != ".."))
                    {
                        string sourceFilePath = remotePath + "/" + file.Name;
                        string destFilePath = Path.Combine(localPath, file.Name);
                        if (file.IsDirectory)
                        {
                            DownloadDirectory(client, sourceFilePath, destFilePath, keyValuePairs);
                        }
                        else
                        {
                            using (Stream fileStream = File.Create(destFilePath))
                            {
                                client.DownloadFile(sourceFilePath, fileStream);
                            }
                        }
                    }
                }
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message);
                System.Diagnostics.Debug.WriteLine(_e.Message);
            }

        }


    }
}
