using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSharpTools
{
	public class FileTool
	{

		/// <summary>
		/// 备份文件的根路径
		/// </summary>
		public static string FilesRootPath= "E:\\_YZ1_Dev\\Game_64\\Bin\\Client\\Game\\Assets\\";

		/// <summary>
		/// 保存了备份文件列表的文件路径
		/// </summary>
		public static string FileListFilePath = "E:\\_YZ1_Dev\\Game_64\\Bin\\Client\\Game\\BackupFiles.log";

		/// <summary>
		/// 根据文件列表备份文件
		/// </summary>
		public static void BackupFiles()
		{

			if (string.IsNullOrEmpty(FileListFilePath)) { return; }

			if (!File.Exists(FileListFilePath)) { return; }

			LogModule.LogWithStrConnect("FileTool.BackupFiles, FilesRootPath=" + FilesRootPath);

			string[] files = File.ReadAllLines(FileListFilePath);

			int counter = 0;

			foreach (string file in files)
			{
				string sourceFile = FilesRootPath + file;

                if (File.Exists(sourceFile) && sourceFile.EndsWith(".prefab")) // File.Exists(sourceFile) &&
                {

                    // 新的目标文件路径
                    string destinationFile = sourceFile.Replace("Assets", "Assets_H");

                    // 目标文件的地址，不包含文件名
                    string destinationDirectory = Path.GetDirectoryName(destinationFile);

                    //LogModule.LogWithStrConnect(destinationDirectory);

                    if (destinationDirectory == null) continue;

                    if (!Directory.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }


					// 还要处理文件夹的 .meta
					//CopyFileDirMeta(sourceFile.Replace(FilesRootPath, "") );


					// string sourceFile = @"C:\source\myfile.txt";
					// string destinationFile = @"C:\destination\myfile.txt";
					// true 表示如果目标文件存在，则覆盖它
					File.Copy(sourceFile, destinationFile, true);

                    //File.Copy(sourceFile + ".meta", destinationFile + ".meta", true);


                    counter++;
                }
                else 
                {
                    LogModule.LogWithStrConnect("FileTool.BackupFiles, sourceFile=" + sourceFile);
                }

			}

			LogModule.LogWithStrConnect("FileTool.BackupFiles, counter=" + counter);

		}


        /// <summary>
        /// 递归处理文件，传入相对路径，例如 unity的 meta 文件
        /// </summary>
        /// <param name="path"></param>
        public static void CopyFileDirMeta(string path)
        {
            // LogModule.LogWithStrConnect("FileTool.CopyFileDirMeta, path=", path);

            path = path.TrimEnd('/');


            string sourceFile = FilesRootPath + path + ".meta";
            string destinationFile = FilesRootPath.Replace("Assets", "Assets_H") + path + ".meta";


            if (File.Exists(sourceFile)) 
            { 
                File.Copy(sourceFile, destinationFile, true);
            }
            else
            {
                LogModule.LogWithStrConnect("FileTool.CopyFileDirMeta, sourceFile=" + sourceFile);
            }

            
            if (path.Contains("/")) 
            {
                string subPath = path.Substring(0, path.LastIndexOf("/"));

                CopyFileDirMeta(subPath);

            }

        }




    }




}
