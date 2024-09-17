using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSharpTools
{
	public class StringTool
	{

		public void RunThis()
		{


			string FilesRootPath = "E:/_YZ1_Dev/Game_64/Bin/Client/Game/Assets";
			string subPath = FilesRootPath.Substring(0, FilesRootPath.LastIndexOf("/"));

			Console.WriteLine("Test.RunThis! End." + subPath);


		}





	}
}
