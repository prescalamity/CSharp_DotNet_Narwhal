using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		public static string FilesRootPath= "D:\\_CSharpProj\\CSharp_DotNet_Narwhal\\";

		/// <summary>
		/// 保存了备份文件列表的文件路径
		/// </summary>
		public static string FileListFilePath = "D:\\_CSharpProj\\CSharp_DotNet_Narwhal\\BackupFiles\\BackupFiles.txt";

		/// <summary>
		/// 根据文件列表备份文件
		/// </summary>
		public static void BackupFiles()
		{

			if (string.IsNullOrEmpty(FileListFilePath)) { return; }

			if (!File.Exists(FileListFilePath)) { return; }

			Console.WriteLine("FileTool.BackupFiles, FilesRootPath=" + FilesRootPath);

			string[] files = File.ReadAllLines(FileListFilePath);

			int counter = 0;

			foreach (string file in files)
			{
				string sourceFile = FilesRootPath + file;

				if (sourceFile.EndsWith(".prefab")) //File.Exists(sourceFile) &&
				{ 

					string destinationFile = sourceFile;

					// 新的目标文件路径
					destinationFile = destinationFile.Replace("Assets", "Assets_H");

					// 目标文件的地址，不包含文件名
					string destinationDirectory = Path.GetDirectoryName(destinationFile);

					//Console.WriteLine(destinationDirectory);

					if (destinationDirectory == null) continue;

					if (!Directory.Exists(destinationDirectory))
					{
						Directory.CreateDirectory(destinationDirectory);
					}

					// string sourceFile = @"C:\source\myfile.txt";
					// string destinationFile = @"C:\destination\myfile.txt";
					// true 表示如果目标文件存在，则覆盖它
					//File.Copy(sourceFile, destinationFile, true);

					counter++;
				}

			}

			Console.WriteLine("FileTool.BackupFiles, counter=" + counter);

		}



	}
}
