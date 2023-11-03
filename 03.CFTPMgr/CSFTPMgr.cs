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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath">리눅스 서버 경로</param>
        /// <param name="localPath">저장 할 윈도우 서버 경로</param>
        public static void DownloadFiles(string remotePath, string localPath)
        {
            var localDir = localPath;
            var remoteDir = remotePath;

            var files = new List<String>();
            using (SftpClient client = new SftpClient("192.168.211.9", 22, "root", "yonwoo*0619!@"))
            {
                /*
                client.KeepAliveInterval = TimeSpan.FromSeconds(60);
                client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
                client.OperationTimeout = TimeSpan.FromMinutes(180);
                */
                client.Connect();
                if (false == client.IsConnected)

                    client.ChangeDirectory(remoteDir);
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
        public static void UploadDirectory(SftpClient client, string localPath, string remotePath)
        {
            Console.WriteLine("Uploading directory {0} to {1}", localPath, remotePath);

            IEnumerable<FileSystemInfo> infos =
                new DirectoryInfo(localPath).EnumerateFileSystemInfos();
            try
            {
                foreach (FileSystemInfo info in infos)
                {
                    if (info.Attributes.HasFlag(FileAttributes.Directory))
                    {
                        string subPath = remotePath + "/" + info.Name;
                        if (!client.Exists(subPath))
                        {
                            client.CreateDirectory(subPath);
                        }
                        UploadDirectory(client, info.FullName, remotePath + "/" + info.Name);
                    }
                    else
                    {
                        using (Stream fileStream = new FileStream(info.FullName, FileMode.Open))
                        {
                            Console.WriteLine(
                                "Uploading {0} ({1:N0} bytes)",
                                info.FullName, ((FileInfo)info).Length);

                            client.UploadFile(fileStream, remotePath + "/" + info.Name);
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
