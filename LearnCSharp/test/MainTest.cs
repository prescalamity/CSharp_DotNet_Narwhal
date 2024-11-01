using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json.Nodes;
using SimpleJSON;

namespace LearnCSharp
{
    /// <summary>
    /// 日志等级，一共8个，共 255个 组合
    /// 1111 1111 = 255
    /// 1000 0000 = 128  = 2^7
    /// 0000 0001 = 1    = 2^0
    /// </summary>
    public enum LogController
    {

        First = 1,

        Second = 2,

        Third = 4,

        PersonDebug = 8,

        NetOrDataTransmit = 16,

        Warning = 32,

        ProgramImportantNode = 64,

        Error = 128,

    }

    /// <summary>
    /// 实时语音背景语音 信息
    /// </summary>
    public struct RTBGMInfo
    {
        public string szDisplayName;
        public string szFilePath;
        public int nDuration;
    }

    public class MainTest
    {
        private List<RTBGMInfo> m_strBgmFileList = new List<RTBGMInfo>();

        private List<string> m_PlaybackFileID = new List<string>();

        public void RunThis() {
            Console.WriteLine("MainTest.RunThis!");

            int dasd = 100;

            float dda = dasd;


            string ddasdJson = @"
[
	{	""display_name"":""Tales of the Electric Romeo - Immediate Music.mp3"",
		""file_path"":""PresetBGM0.mp3"",
		""duration_second"":""130""
	},
	{	""display_name"":""金达莱花.mp3"",
		""file_path"":""PresetBGM1.mp3"",
		""duration_second"":""149""
	},
	{	""display_name"":""孙浩雨 - 像男人一样去战斗(DJ小鱼儿 Remix) - YT_Official.mp3"",
		""file_path"":""PresetBGM2.mp3"",
		""duration_second"":""170""
	},
	{	""display_name"":""孙浩雨 - 生命之枪.mp3"",
		""file_path"":""PresetBGM3.mp3"",
		""duration_second"":""362""
	}
]
";

            JSONNode jsonNode = JSON.Parse(ddasdJson);

            for (int i = 0; i < jsonNode.Count; i++)
            {
                RTBGMInfo rTBGMInfo = new RTBGMInfo();

                rTBGMInfo.szDisplayName = jsonNode[i]["display_name"].ToString();
                rTBGMInfo.szFilePath = jsonNode[i]["file_path"].ToString();
                rTBGMInfo.nDuration = int.Parse(jsonNode[i]["duration_second"].ToString());

                m_strBgmFileList.Add(rTBGMInfo);

            }

            Console.WriteLine($"dda={dda}, CVoiceChatClient.OnResponse, m_strBgmFileList.Count={m_strBgmFileList.Count}");
            Console.WriteLine($"CVoiceChatClient.OnResponse, m_strBgmFileList.1.szDisplayName={m_strBgmFileList[1].szDisplayName}");


            string data = "dsadssdasdcxzvzxlvsdjirnurgnbvuefdwuejdfnjsdfneiujqiejiewmfksdnfsjkdnfdfdsfsdfwefwfvsvxcbxcbdsfrgsdgfs";

            Console.WriteLine($"BillboardList.OnGetData, v0, data.Length={data.Length}, data 50--->{data.Substring(0, data.Length > 50 ? 50 : data.Length)}");


            string filename = "wxfile//user/xxx.mp3";
            string fileName = filename.Substring(filename.LastIndexOf("/")+1);
            Console.WriteLine($"BillboardList.OnGetData, v0, fileName={fileName}, ");


            m_PlaybackFileID.Add( "aaa" );
            m_PlaybackFileID.Add( "bbb" );
            m_PlaybackFileID.Add( "ccc" );


            Console.WriteLine($"MainTest.RunThis, 1, m_PlaybackFileID.Length={m_PlaybackFileID.Count}");

            m_PlaybackFileID.Remove("aaa");

            Console.WriteLine($"MainTest.RunThis, 2, m_PlaybackFileID.Length={m_PlaybackFileID.Count}");


            MyCodeChild.funcLgy();


            bool dasdBool = true;


            Console.WriteLine($"BillboardList.OnGetData, ------- dasdBool={dasdBool} -------------------------\n");


            LogController testEnum = (LogController)dasd;

            LogController testEnum1 = LogController.First;

            if (testEnum == testEnum1)
            {
                Console.WriteLine($"BillboardList.OnGetData, ------- testEnum={testEnum} -----true--------------------\n");
            }
            else
            {
                Console.WriteLine($"BillboardList.OnGetData, ------- testEnum={testEnum} -----false--------------------\n");
            }

            string das= nameof( LogController.PersonDebug);

            Console.WriteLine($"------- das={das} ----------\n");


            var dsad = new MyCode[3];
            Console.WriteLine($"MainTest.RunThis, --------dsad type={dsad.GetType()==typeof(MyCode[])}");

            if (dsad.GetType() == typeof(MyCode[]))
            {

            }
            if (dsad.GetType().BaseType == typeof(Array)) { 
                Console.WriteLine($"MainTest.RunThis, --------dsad type=true lgy");
                int counter = 0;
                foreach(var i in dsad)
                {
                    Console.WriteLine(counter++); 
                } 
            }
            else
            {

                Console.WriteLine($"MainTest.RunThis, --------dsad type=false lgy");
            }





            //UnitTest1 unitTest1 = new UnitTest1();

            //unitTest1.TestMethod1();
            //unitTest1.TestMethod2();
            //unitTest1.TestMethod3();
            //unitTest1.TestMethod4();


            // 测试命名空间继承关系
            //TestExtensionMethods testExtensionMethods = new TestExtensionMethods();
            //string das = testExtensionMethods.GetName();
            //testExtensionMethods.RunThis();
            //Console.WriteLine(das);

        }
    }
    public class MyCode {


        public static void funcLgy()
        {
            Console.WriteLine($"Hello, World! in 2024-7-3, in MyCode.funcLgy, {DateTime.Now.ToString("yyyy/MM/dd_HH:mm:ss_fff")}");

            Console.WriteLine($"Hello, World! in 2024-7-3, in MyCode.funcLgy, \n{new System.Diagnostics.StackTrace().ToString()}");

            object modelItem = null;

            Console.WriteLine($"Hello, World! in 2024-7-3, in MyCode.funcLgy, modelItem =? null:{modelItem == null}");

        }


        public bool isBoolRes()
        {

            int re = 2 + 4;


            float fNumber = 100f;
            int iNumber = 100;


            if (fNumber <= iNumber)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }



    public class MyCodeChild : MyCode
    {


        public string myCodeChildFunc()
        {

            //funcLgy();

            return "MyCodeChild.myCodeChildFunc";
        }


    }



}
