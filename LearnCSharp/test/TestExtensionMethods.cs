/*
 * 
 * 
 * 测试学习扩展函数
 * 
 * 
 * 
 * 
 * 
 */

namespace LearnCSharp.test
{
	/// <summary>
	/// 测试扩展函数
	/// </summary>
	public class TestExtensionMethods
	{

		public string name = "Lugiyan";

		private int age;

		//public string GetName()
		//{
		//    return name;
		//}

		public int GetAge() { return age; }


		public void RunThis() {

			MainTest mainTest = new MainTest();


		}



	}


	public static class ExtensionMethods
	{

		public static string GetName(this TestExtensionMethods self)
		{
			return self.name + "_ext";
		}





	}
}
