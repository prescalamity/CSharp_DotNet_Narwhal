﻿using System;
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
		#region ---------------------------获取 文件夹下的 文件的 编码格式----------------------------------------


		public static void GetFileEncodingByPath(string folderPath = "")
		{
			folderPath = @"D:\_YZ1_Publish_Game64\Bin\Client\Game_Harmony\Assets\GResources\Lua\LuaScript"; // 请替换为你的文件夹路径
			folderPath = @"D:\_EncodingFiles"; // 请替换为你的文件夹路径

			int counter = 0;
			foreach (var file in Directory.GetFiles(folderPath, "*.lua", SearchOption.AllDirectories))
			{

				//Encoding  encoding = GetFileEncoding( file);

				//if (file.EndsWith(".lua"))
				//{
				//	counter++;

				//	string[] bytes = File.ReadAllLines(file);
				//	File.WriteAllLines(file, bytes, new UTF8Encoding(true));
				//}

				//Console.WriteLine($"File: {file}, Encoding.EncodingName: {encoding.EncodingName}, " +
				//	$"Encoding.HeaderName: {encoding.HeaderName}, ");
				// File: D:\BindPhone\BossGambleMgr.lua, Encoding: System.Text.UTF8Encoding+UTF8EncodingSealed

				bool isBom = IsUtf8WithBom( file);
				if (!isBom)
				{
					Console.WriteLine($"File: {file}, not is Utf8-Bom: {isBom}, ");
				}

			}
			Console.WriteLine($"counter: {counter}");
		}

		static Encoding GetFileEncoding(string filename)
		{
			using (var reader = new StreamReader(filename, true))
			{
				// 读取一行以使编码能够被识别
				reader.ReadLine();
				return reader.CurrentEncoding;
			}
		}


		static bool IsUtf8WithBom(string filePath)
		{
			// UTF-8 BOM 的字节序列
			byte[] bom = new byte[] { 0xEF, 0xBB, 0xBF };

			// 检查文件的前 3 个字节是否等于 BOM 字节
			using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				byte[] buffer = new byte[3];
				int bytesRead = fs.Read(buffer, 0, 3);
				if (bytesRead < 3)
				{
					return false; // 文件小于 3 字节，不可能是 UTF-8 BOM
				}

				return buffer[0] == bom[0] && buffer[1] == bom[1] && buffer[2] == bom[2];
			}
		}

		#endregion ---------------------------获取 文件夹下的 文件的 编码格式----------------------------------------



		#region ---------------------------删除文件夹下指定后缀的文件----------------------------------------

		/// <summary>
		/// 指定的文件夹
		/// </summary>
		public static string FolderForDeleteFile = "D:\\_TuanjieHarmonyProjs\\_Game_Harmony_SDK25\\entry";

        /// <summary>
        /// 后缀的匹配模式
        /// </summary>
        public static string theSuffixSearchPattern = "*.meta";

        /// <summary>
        /// 递归处理文件，传入相对路径，例如 unity的 meta 文件
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteTheFileBySuffix(string path="", string thePatternSuffix = "")
        {
            if (string.IsNullOrEmpty(path)) path = FolderForDeleteFile;
            if (string.IsNullOrEmpty(thePatternSuffix)) thePatternSuffix = theSuffixSearchPattern;

            if ( ! Directory.Exists(path)) return;

            string[] files = Directory.GetFiles(path, thePatternSuffix, SearchOption.AllDirectories);

            Console.WriteLine($"FileTool.DeleteTheFileBySuffix, path: {path}, suffix={thePatternSuffix}, files.Length: {files.Length} \n");

            try
            {
                // 获取指定路径下所有文件和子目录中的文件
                foreach (string file in files)
                {
                    // 删除找到的每个.png文件
                    File.Delete(file);
                    //Console.WriteLine($"Deleted: {file}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }

        #endregion ---------------------------删除文件夹下指定后缀的文件----------------------------------------




        #region ---------------------------备份路径下的某些符合要求的文件----------------------------------------

        /// <summary>
        /// 备份文件的根路径
        /// </summary>
        public static string FilesRootPath= "E:\\_YZ1_Dev\\Game_64\\Bin\\Client\\Game\\Assets\\";

		/// <summary>
		/// 保存了备份文件列表的文件路径
		/// </summary>
		public static string FileListFilePath = "E:\\LogDiffFileLgy - 副本.log";

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

                    string sourceFileDir = Path.GetDirectoryName(sourceFile);
                    // 还要处理文件夹的 .meta
                    CopyFileDirMeta(sourceFileDir.Replace(FilesRootPath, "") );


					// string sourceFile = @"C:\source\myfile.txt";
					// string destinationFile = @"C:\destination\myfile.txt";
					// true 表示如果目标文件存在，则覆盖它
					File.Copy(sourceFile, destinationFile, true);

                    File.Copy(sourceFile + ".meta", destinationFile + ".meta", true);


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

        #endregion ---------------------------备份路径下的某些符合要求的文件----------------------------------------



        #region --------------------------- 比较两个文件并将结果保存 ---------------------------------------------

        /// <summary>
        /// 要比较的文件 File1
        /// </summary>
        public static string CompareFileListFilePath1 = "E:\\_MyFont\\新华字典汉字16142.txt";

        /// <summary>
        /// 要比较的文件 File2
        /// </summary>
        public static string CompareFileListFilePath2 = "E:\\_MyFont\\常用汉字8105.txt";

        /// <summary>
        /// 比较后的结果文件 File2
        /// </summary>
        public static string CompareFileListFilePathRes = "E:\\_MyFont\\汉字16142-8105.txt";


        /// <summary>
        /// 对比两个文件中不同的文件，存在或不存在
        /// </summary>
        public static void CompareFileListFiles()
        {
            if (string.IsNullOrEmpty(CompareFileListFilePath1)) { return; }
            if (string.IsNullOrEmpty(CompareFileListFilePath2)) { return; }


            //if (!File.Exists(CompareFileListFilePath1)) { return; }
            //if (!File.Exists(CompareFileListFilePath2)) { return; }


            //LogModule.LogWithStrConnect("FileTool.CompareFileListFiles, CompareFileListFilePath1=" + CompareFileListFilePath1);


            string[] files1 = File.ReadAllLines(CompareFileListFilePath1);
            string[] files2 = File.ReadAllLines(CompareFileListFilePath2);

            int counter = 0;

            for (int i = 0; i< files1.Length; i++)
            {
                bool flag = true;

                for (int j = 0; j < files2.Length; j++)
                {
                    // file1 中的元素存在于 file2
                    if ( !string.IsNullOrEmpty(files1[i]) && !string.IsNullOrEmpty(files2[j]) && files1[i].Contains(files2[j]) ) 
                    {
                        flag = false;
                        // continue;
                        break;
                    }

                }

                if (flag) {
                    // 包含就保存
                    counter++;
                    File.AppendAllText(CompareFileListFilePathRes, string.Format("{0}{1}", files1[i], Environment.NewLine), new UTF8Encoding(false));
                }

            }

            //File.AppendAllText(CompareFileListFilePathRes, string.Format("{0}counter--->{1}{2}", Environment.NewLine, counter, 
            //    Environment.NewLine), new UTF8Encoding(false));


            Console.WriteLine("FileTool.CompareFileListFiles");
        }


        #endregion --------------------------- 比较两个文件并将结果保存 ---------------------------------------------



        #region ---------------------按规则修改指定文件中的内容----------------------------------------------


        public static string mfcFilePath = "C:\\Users\\Administrator\\Desktop\\dasdas.log";

        public static void modifyFileContent()
        {

            string[] files1 = File.ReadAllLines(mfcFilePath);
            int counter = 0;
            for(int i =0; i<files1.Length; i++)
            {
                if (files1[i].Contains("好友0")) {
                    files1[i] = files1[i].Replace("好友0", "好友" + counter++);
                }
            }

            File.WriteAllLines(mfcFilePath, files1, new UTF8Encoding(false));

        }

        #endregion ---------------------按规则修改指定文件中的内容---------------------------------------------



    }




}
