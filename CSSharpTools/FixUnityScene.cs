using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace CSSharpTools
{

	/// <summary>
	/// 修改unity场景中某个指定组件的某个属性值
	/// </summary>
	public class FixUnityScene
	{

	}





	/// <summary>
	/// OC AI代码解析Unity Scene文件
	/// </summary>
	public static class UnitySceneParser
	{
		public static void FixUnityScene()
		{

			string scene = File.ReadAllText("D:\\_JianXian_HuanPi\\Bin\\Client\\GameUI\\Assets\\TestScene.unity");

			//foreach (var (fileID, body) in UnitySceneParser.MatchGameObjects(scene))
			//{
			//	Console.WriteLine($"找到 GameObjects，fileID={fileID}, body=\n{body}");
			//	// 可在 body 里进一步用正则提取 m_Script、字段值等
			//}

			foreach (var (fileID, body) in UnitySceneParser.MatchGameObjectNodes(scene))
			{
				Console.WriteLine($"找到 GameObjects，fileID={fileID}, body=\n{body}");
				// 可在 body 里进一步用正则提取 m_Script、字段值等
			}

		}





		// 匹配 GameObject 块（Hierarchy 节点本体）
		//   --- !u!1 &<fileID>
		//   GameObject:
		//   ...块体...
		private static readonly Regex GameObjectBlockRegex = new Regex(
			@"^--- !u!1 &(?<fileID>\d+).*?\r?\n" +     // 块头，classID=1，捕获 fileID
			@"GameObject:\r?\n" +                      // 类型行
			@"(?<body>(?:.*\r?\n?)*?)" +               // 块体（非贪婪）
			@"(?=^--- |\z)",                           // 前瞻：下一个块或文件末尾
			RegexOptions.Multiline | RegexOptions.Compiled);

		// 匹配 Transform / RectTransform 块（还原父子关系用）
		//   --- !u!4 &<fileID>   或   --- !u!224 &<fileID>
		private static readonly Regex TransformBlockRegex = new Regex(
			@"^--- !u!(?<classID>4|224) &(?<fileID>\d+).*?\r?\n" +
			@"(?<type>Transform|RectTransform):\r?\n" +
			@"(?<body>(?:.*\r?\n?)*?)" +
			@"(?=^--- |\z)",
			RegexOptions.Multiline | RegexOptions.Compiled);

		public static IEnumerable<(string fileID, string body)> MatchGameObjects(string sceneText)
		{
			foreach (Match m in GameObjectBlockRegex.Matches(sceneText))
				yield return (m.Groups["fileID"].Value, m.Groups["body"].Value);
		}

		public static IEnumerable<(string classID, string fileID, string body)> MatchTransforms(string sceneText)
		{
			foreach (Match m in TransformBlockRegex.Matches(sceneText))
				yield return (m.Groups["classID"].Value, m.Groups["fileID"].Value, m.Groups["body"].Value);
		}




		// 匹配整个 GameObject 节点：从 --- !u!1 开始，
		// 抓到下一个 GameObject 块头(--- !u!1)或文件结尾为止。
		// body 中会包含该节点的 Transform/RectTransform、Canvas、MonoBehaviour 等所有组件块。
		private static readonly Regex GameObjectNodeRegex = new Regex(
			@"^--- !u!1 &(?<fileID>\d+).*?\r?\n" +          // GameObject 块头，捕获 fileID
			@"GameObject:\r?\n" +                            // 类型行，二次确认
			@"(?<body>(?:(?!^--- !u!1 ).*\r?\n?)*)",         // 块体：吃掉后续所有行，但遇到下一个 GameObject 块头停止
			RegexOptions.Multiline | RegexOptions.Compiled);

		public static IEnumerable<(string fileID, string body)> MatchGameObjectNodes(string sceneText)
		{
			foreach (Match m in GameObjectNodeRegex.Matches(sceneText))
				yield return (m.Groups["fileID"].Value, m.Groups["body"].Value);
		}





	}





	/// <summary>
	/// Unity Scene UImage 自动调整大小属性修复工具
	/// 用户输入 Scene 文件路径后，自动修改所有 UImage 组件的 m_autoAjustSize 为 1
	/// </summary>
	public class UnitySceneUImageFixer
	{
		// UImage 脚本的 FileID
		private const int UIMAGE_SCRIPT_ID = 1261933078;

		/// <summary>
		/// 已处理的文件名字列表
		/// </summary>
		static StringBuilder _DealFilePath = new StringBuilder();

		public static void FixUnityScene()
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			//PrintWelcome();

			while (true)
			{
				Console.Write("\n📁 请输入 Unity Scene 文件夹路径: ");
				string folderPath = Console.ReadLine();

				if (string.IsNullOrWhiteSpace(folderPath))
				{
					Console.WriteLine("❌ 路径不能为空，请重新输入");
					continue;
				}

				// 移除引号（Windows 可能包含引号）
				folderPath = folderPath.Trim('"', '\'');

				if (!Directory.Exists(folderPath))
				{
					Console.WriteLine($"❌ 文件夹不存在: {folderPath}");
					continue;
				}

				_DealFilePath.Clear();

				string[] files = DeleteTheFileBySuffix(folderPath, "*.unity");

				if (files == null)
				{
					Console.WriteLine($"❌ {folderPath}，没有找到目标文件，请重新输入");
					continue;
				}

				Console.WriteLine($"找到的目标文件数量: {files.Length}");

				try
				{
					foreach (var file in files)
					{
						Console.WriteLine($"处理目标文件路径: {file}");
						if (!file.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
						{
							Console.WriteLine("❌ 请选择 .unity 文件");
							continue;
						}

						FixAutoAdjustSize(file);
					}


					if (_DealFilePath.Length > 0)
					{
						// 保存修改后的文件
						string backupPath = "D:\\_JianXian_HuanPi\\Bin\\Client\\GameUI\\ModifiedRedPointFile.log";
						File.WriteAllText(backupPath, _DealFilePath.ToString(), Encoding.UTF8);
					}

					Console.Write("\n是否继续处理其他文件夹？(y/n): ");
					string continueInput = Console.ReadLine();
					if (continueInput?.ToLower() != "y")
					{
						break;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"❌ 处理失败: {ex.Message}");
				}
			}

			Console.WriteLine("\n👋 程序已退出");
		}

		/// <summary>
		/// 指定的文件夹
		/// </summary>
		public static string FolderForDeleteFile = "D:\\_JianXian_HuanPi\\Bin\\Client\\GameUI\\Assets\\scenes";

		/// <summary>
		/// 后缀的匹配模式
		/// </summary>
		public static string theSuffixSearchPattern = "*.unity";

		public static string[] DeleteTheFileBySuffix(string path = "", string thePatternSuffix = "")
		{
			if (string.IsNullOrEmpty(path)) path = FolderForDeleteFile;
			if (string.IsNullOrEmpty(thePatternSuffix)) thePatternSuffix = theSuffixSearchPattern;

			if (!Directory.Exists(path)) return null;

			string[] files = Directory.GetFiles(path, thePatternSuffix, SearchOption.AllDirectories);

			Console.WriteLine($"FileTool.DeleteTheFileBySuffix, path: {path}, suffix={thePatternSuffix}, files.Length: {files.Length} \n");

			return files;
		}

		//private static void PrintWelcome()
		//{
		//	Console.WriteLine("════════════════════════════════════════");
		//	Console.WriteLine("   Unity Scene UImage 属性修复工具");
		//	Console.WriteLine("════════════════════════════════════════");
		//	Console.WriteLine("✨ 功能: 将 UImage 组件的 m_autoAjustSize");
		//	Console.WriteLine("        从 0 改为 1");
		//	Console.WriteLine("════════════════════════════════════════\n");
		//}

		private static void FixAutoAdjustSize(string filePath)
		{
			Console.WriteLine($"\n📖 正在读取文件...");

			// 读取文件
			string content = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
			string originalContent = content;

			// 查找所有 MonoBehaviour 块，其 Script 为 UImage
			int changedCount = 0;
			var modifiedContent = ReplaceUImageAutoAdjustSize(content, ref changedCount);

			if (modifiedContent == originalContent)
			{
				//Console.WriteLine($"⚠️  文件中未找到需要修改的 UImage 组件, changedCount={changedCount}, {filePath}" );
				return;
			}
			else
			{
				Console.WriteLine($"⚠️  文件中找到并修改了的 UImage 组件, changedCount={changedCount}, {filePath}");
				_DealFilePath.Append(filePath);
				_DealFilePath.Append(Environment.NewLine);
			}

			// 保存修改后的文件
			//string backupPath = filePath + ".backup";
			//File.Copy(filePath, backupPath, true);
			//Console.WriteLine($"✅ 已创建备份文件: {backupPath}");

			File.WriteAllText(filePath, modifiedContent, System.Text.Encoding.UTF8);

			//PrintResults(changedCount, filePath);
		}

		private static string ReplaceUImageAutoAdjustSize(string content, ref int changedCount)
		{
			// 正则表达式：匹配 MonoBehaviour 块
			// 查找包含 "m_Script: {fileID: 1261933078" 的块，然后替换其中的 m_autoAjustSize

			string result = content;    // 原文件内容

			var pattern = @"(?ms)^(?<block>--- !u!114 &(?<fileId>\d+)\r?\nMonoBehaviour:\r?\n.*?)(?=^--- !u!|\z)";


			// 先找到所有 UImage 组件的位置
			MatchCollection scriptMatches = Regex.Matches(content, pattern, RegexOptions.Multiline | RegexOptions.Singleline);


			foreach (Match scriptMatch in scriptMatches)
			{
				changedCount++;

				string originalBlock = scriptMatch.Value;

				string newBlock = string.Empty;

				if (originalBlock.Contains("fileID: 1261933078") && originalBlock.Contains("m_resName: flg_47") && originalBlock.Contains("m_autoAjustSize: 0"))
				{

					//Console.WriteLine($"ReplaceUImageAutoAdjustSize, 这个块 引用了flg_35文件, 且没有设置自适应， blockMatch={scriptMatch.Value}");

					newBlock = originalBlock.Replace("m_autoAjustSize: 0", "m_autoAjustSize: 1");

					result = result.Replace(originalBlock, newBlock);
				}
				else
				{
					//Console.WriteLine($"ReplaceUImageAutoAdjustSize, 这个块没有UImage组件和引用flg_35文件");
				}

			}

			return result;
		}


	}


}






