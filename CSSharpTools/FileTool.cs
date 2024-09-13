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
				string filePath = FilesRootPath + file;

				if ( filePath.EndsWith(".prefab")) //File.Exists(filePath) &&
				{ 
					counter++;

					string pathOnly = Path.GetDirectoryName(filePath);

					Console.WriteLine(pathOnly);

					if (pathOnly == null) continue;

					pathOnly = pathOnly.Replace("Assets", "Assets_H");

					if (!Directory.Exists(pathOnly)) { 

						Directory.CreateDirectory(pathOnly); 

					}
				}
			}

			Console.WriteLine("FileTool.BackupFiles, counter=" + counter);

		}





	}
}
