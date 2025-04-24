using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json.Nodes;
using SimpleJSON;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using LitJson;
using System.Data;

namespace LearnCSharp
{

    public struct SMsgPayPrize_OpenPay_SC
    {
        public UInt32 dwProductId;                      // 商品ID
        public string szName;                           // 商品名字
        public string szPayItemID;                      // 内购ID（IOS支付专用）
    }

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

    public enum EntityFlags : uint
    {
        flagVisible = 0x1,			/// 可见（标识是否在屏幕范围内）1
	    flagSelectable = 0x2,			/// 逻辑可选择 2
	    flagSelected = 0x4,			/// 被选中
	    flagSelectedForMapEdit = 0x8,			/// 被选中（用于地图编辑器）
	    flagHighlight = 0x10,           /// 是否高亮

        // 类型
        flag2D = 0x20,			/// 2D类型
	    flag3D = 0x40,          /// 3D类型

        // 渲染标志
        flagDrawFilename = 0x80,
        flagDrawName = 0x100,		/// 绘制实体名字 256
	    flagDrawResId = 0x200,		/// 绘制实体资源Id
	    flagDrawHP = 0x400,		/// 绘制实体HP
	    flagDrawTitle = 0x1000,		/// 绘制称号
	    flagDrawStall = 0x2000,     /// 绘制摊位名
        //flagDrawForce			= 0x4000,		/// 强制渲染，不受F12影响
        flagTitleTopmost = 0x4000,		/// 称号画在最上层
	    flagCanIgnoreRender = 0x8000,		/// 标识渲染时受细节层次的影响(远视野可忽略)
	    flagDrawHPAni = 0x100000,		/// boss、精英的hp动画
	    flagDrawTeam = 0x200000,		/// 我的队伍成员标志
        flagDrawTribeLeader = 0x400000,		    /// 帮主标志
	    flagDrawNationLeader = 0x800000,		/// 国王标志
	    flagDrawIgnore = 0x10000,       /// 标识忽略渲染，目前用于F12检测

        // 位置信息
        flagInMap = 0x20000,		/// 标识对象是否在地图上
	    flagFade = 0x40000,		/// 是否支持淡入淡出处理
	    flagNoLogic = 0x80000,		/// 是否是非逻辑对象
	    flagNoShadow = 0x1000000,	/// 模型是否取消影子渲染
	    flagDisplayList = 0x2000000,	/// 标识存在于Display列表
	    flagFadeOutList = 0x4000000,    /// 标识存在于FadeOut列表

        // 状态
        flagResLoaded = 0x8000000,	/// 标识资源是否加载
	    flagHide = 0x10000000,	/// 隐藏标志(隐藏状态下，不进行update和draw)
	    flagForbidAngleChanged = 0x20000000,	/// 禁止实体的角度改变(可以在创建后，设置个初始角度，然后添加该标志，使后面不再进行角度修改)
        flagMoving = 0x30000000,   /// 移动
	    flagSelectedByRect = 0x40000000,	/// 只通过包围矩形选中
	    flagRotateEnable = 0x80000000,	/// 是否支持缓冲旋转
	    flagForbidInheritSR = 0x800,		/// 光效是否禁止继承拥有者的体型参数

        //flagSelectedViewName    = 0x100000000,  ///选中时显示名字
        //flagSelectedViewBlood   = 0x200000000,  ///选中时显示血条
    };


    public struct TMP_TextProcessingStack<T>
    {
        public T[] itemStack;
        public int index;

        T m_DefaultItem;
        int m_Capacity;
        int m_RolloverSize;
        int m_Count;

        const int k_DefaultCapacity = 4;
    }


    // 结构体示例
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point()
        {
        }
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // 类示例
    public class Rectangle
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Point startPoint=new Point(0,1);

        public MyCodeChild myCodeChild;

        public Rectangle()
        {
        }

        public Rectangle(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int GetArea() { 
            return Width * Height; 
        }

    }

    public class MainTest
    {
        private List<RTBGMInfo> m_strBgmFileList = new List<RTBGMInfo>();

        private List<string> m_PlaybackFileID = new List<string>();
        private uint m_FlagsF;


        public Dictionary<string, string> assetBundleCaches= new Dictionary<string, string>();

        private List<string> m_RemovedKeys=new List<string>();


        //private Int64 m_iCurrentHP = 1138506000;//-2147483648;
        //private Int64 m_UnitBlood = 1138506000;//-2147483648;

        //private Int64 m_iCurrentHP = -2147483648;
        //private Int64 m_UnitBlood = -2147483648;

        private Int64 m_iCurrentHP = -10;
        private Int64 m_UnitBlood = -10;


        /// <summary>
        /// 资源依赖集
        /// </summary>
        private Dictionary<string, HashSet<string>> m_DependencyMap = new Dictionary<string, HashSet<string>>();
        private Dictionary<string, HashSet<string>> m_ExtDependencyMap = new Dictionary<string, HashSet<string>>();
        private HashSet<string> basicDependAssetBundleSet = null;
        private HashSet<string> extDependAssetBundleSet = null;


        private string m_Q1PID = "";
        /// <summary>
        /// 游戏pid
        /// </summary>
        public string M_Q1PID
        {
            get => m_Q1PID;
            set => m_Q1PID = value;
        }

        private string uuid;
        public string Uuid { 
            get { return uuid; }
            private set { uuid = value; } 
        }


        public string M_Oaid { get; internal set; } = "";
        public string M_Radid { get; internal set; } = "";
        public string M_Rsid { get; internal set; } = "";


        public string strExtension=null;                    // 扩展数据( U8获取订单号时，当前渠道SDK的扩展数据。比如渠道有下单操作之后，这个字段里面，就存放了渠道SDK下单返回的数据)
        public string strSign="";                           // 服务器签名

        public void RunThis() {
            Console.WriteLine($"MainTest.RunThis!");

            SMsgPayPrize_OpenPay_SC sMsgPayPrize_OpenPay_SC = new SMsgPayPrize_OpenPay_SC();
            sMsgPayPrize_OpenPay_SC.szName = "asd";
            sMsgPayPrize_OpenPay_SC.szPayItemID = "123";
            sMsgPayPrize_OpenPay_SC.dwProductId = 456;



            Console.WriteLine($"MainTest.RunThis, sMsgPayPrize_OpenPay_SC={sMsgPayPrize_OpenPay_SC}, " + sMsgPayPrize_OpenPay_SC.szName);

            Console.WriteLine($"MainTest.RunThis, strExtension={strExtension}, strSign={strSign}" );



            assetBundleCaches.Add("111","qqq");
            assetBundleCaches.Add("222","www");
            assetBundleCaches.Add("333","eee");
            assetBundleCaches.Add("444","rrr");
            assetBundleCaches.Add("555","ttt");


            KeyValuePair<string,string> item1 = assetBundleCaches.First();

            Console.WriteLine($"MainTest.RunThis, assetBundleCaches.Count={assetBundleCaches.Count}, item1={item1.Key}, item1={item1.Value}");

            assetBundleCaches.Remove(item1.Key);
            KeyValuePair<string, string> item2 = assetBundleCaches.First();

            Console.WriteLine($"MainTest.RunThis, assetBundleCaches.Count={assetBundleCaches.Count}, item1={item2.Key}, item1={item2.Value}");

            //M_Q1PID = "123456";

            //Uuid = "7890";


            //Console.WriteLine($"MainTest.RunThis, M_Q1PID=" + M_Q1PID);
            //Console.WriteLine($"MainTest.RunThis, M_Uuid=" + Uuid);



            //int ipp = 0;
            //Console.WriteLine($"MainTest.RunThis! ipp++ = {ipp++}");
            //Console.WriteLine($"MainTest.RunThis! ++ipp = {++ipp}");

            //JsonData m_JsonData = new JsonData();

            //m_JsonData["GameID"] = 1;
            //m_JsonData["ServerID"] = 2;
            //m_JsonData["UserID"] = 3;
            //m_JsonData["PayNum"] = 4;
            //Console.WriteLine($"MainTest.RunThis! m_JsonData = {m_JsonData.ToJson()}");

            ////m_JsonData.Clear();
            //m_JsonData["OrderItem"] = 5;
            //m_JsonData["OrderNo"] = 6;
            //m_JsonData["OrderSign"] = 7;
            //m_JsonData["currencyType"] = 8;
            //Console.WriteLine($"MainTest.RunThis! m_JsonData = {m_JsonData.ToJson()}");


            //m_JsonData.Clear();
            //m_JsonData["platform"] = 9;
            //m_JsonData["eventKind"] = 10;
            //m_JsonData["datas"] = JsonMapper.ToJson(new int[]{11,12,13});
            //Console.WriteLine($"MainTest.RunThis! m_JsonData = {m_JsonData.ToJson()}");

            //foreach (var item in basicDependAssetBundleSet)
            //{
            //    Console.WriteLine($"MainTest.RunThis! item = {item}");
            //}

            //for (int i = 0; i < 30000; i++)
            //{
            //    m_DependencyMap.Add("m_DependencyMap"+i, new HashSet<string>());
            //    m_ExtDependencyMap.Add("m_ExtDependencyMap" + i, new HashSet<string>());
            //}

            //long resCounter = 0;

            //long flag1 = DateTime.UtcNow.Ticks;
            //for (int i = 0; i < 30000; i++)
            //{
            //    m_DependencyMap.TryGetValue("m_DependencyMap1000", out basicDependAssetBundleSet);

            //    m_ExtDependencyMap.TryGetValue("m_ExtDependencyMap1000", out extDependAssetBundleSet);

            //    if (basicDependAssetBundleSet != null)
            //    {
            //        resCounter ++;
            //    }
            //}
            //long flag2 = DateTime.UtcNow.Ticks;

            //Console.WriteLine($"MainTest.RunThis! resCounter={resCounter}, res1 - res2 = {flag2 - flag1}");
            ////MainTest.RunThis!  res1 - res2=9991,
            ////MainTest.RunThis! resCounter=30000, res1 - res2 = 10569
            //// 一次 m_DependencyMap.TryGetValue 操作需要 33 ns

            //Int64 res1 = (m_iCurrentHP - 1) % m_UnitBlood;  // 1138506000 - 1

            //Int64 res2 = (m_iCurrentHP - 1) % m_UnitBlood + 1;

            //float m_fPercent = ((m_iCurrentHP - 1) % m_UnitBlood + 1) * 1.0f / m_UnitBlood;

            //Console.WriteLine($"MainTest.RunThis!  res1={res1}, res2={res2}, m_fPercent={m_fPercent}, ");

            //string text = @"喜从天降！<color=""#8888888"">恭喜玩家<color='#289D32FF'>香辣五花肉</color>获得大奖<a href='client::Tips(49055,1,0,1,9131,12131,16131,0,0,0,2,0,0,0,0,0,0)' class=""c4"">阵眼启灵自选宝箱</a>*1，人品爆棚！";

            //string reg = @"\<color=.+?\>";


            //MatchCollection matches = Regex.Matches(text, reg);
            //foreach (Match match in matches)
            //{
            //    Console.WriteLine("MainTest.RunThis, match=" + match.Value);

            //    string tempNew = match.Value.Replace("'", string.Empty);
            //    tempNew = tempNew.Replace("\"", string.Empty);

            //    text = text.Replace(match.Value, tempNew);
            //}

            //Console.WriteLine("MainTest.RunThis, text=" + text);


            //TMP_TextProcessingStack<string> asdStr = new  TMP_TextProcessingStack<string> ();

            //int dasdas;
            //float dasd;



            //if (asdStr.itemStack == null)
            //{
            //    dasdas = 1;
            //    Console.WriteLine($"MainTest.RunThis, asdStr.itemStack is null.");
            //}
            //else
            //{
            //    dasd = 1;
            //    Console.WriteLine($"MainTest.RunThis, asdStr.itemStack not null");
            //}




            //TMP_TextProcessingStack<string> asdStr1 = null;


            //Point s = new Point();
            //Point ss = new Point(100, 101);
            //Point sss = new Point() { X = 102, Y = 103 };

            //Rectangle q = new Rectangle() { Width = 100, Height = 101};

            //Rectangle rectangle =null;

            //if (rectangle == null || rectangle.myCodeChild == null)
            //{
            //    Console.WriteLine("MainTest.RunThis!, rectangle==null || rectangle.myCodeChild == null");
            //}
            //else
            //{
            //    Console.WriteLine("MainTest.RunThis!, dasdas=" + rectangle.myCodeChild.myCodeChildFunc());
            //}

            //uint m_Flags = (int)EntityFlags.flagDrawName | (int)EntityFlags.flagVisible | (int)EntityFlags.flagSelectable;// | (int)EntityFlags.flagDrawHP默认不可显示血量/
            //m_FlagsF = (int)EntityFlags.flagDrawName | (int)EntityFlags.flagVisible | (int)EntityFlags.flagSelectable;// | (int)EntityFlags.flagDrawHP默认不可显示血量/

            //Console.WriteLine("MainTest.RunThis, m_Flags visible=" + m_Flags+ ", m_FlagsF="+ m_FlagsF);

            //Console.WriteLine("MainTest.RunThis, EntityFlags.flagDrawName=" + (uint)EntityFlags.flagDrawName);

            //int ilgy = 1;
            //int jlgy = 8;

            //jlgy |= ilgy;

            //int klgy = jlgy;

            //Console.WriteLine("MainTest.RunThis, klgy visible=" + klgy);

            //string dad = "武道巅峰--3v3新赛季开.";

            //byte[] szTopic = Encoding.UTF8.GetBytes(dad);

            //string tempNumStr = "";
            //for (int j = 0; j < szTopic.Length; j++)
            //{
            //    tempNumStr += ((int)szTopic[j]).ToString() + " ";
            //}
            //Console.WriteLine($"MainTest.RunThis, Length={szTopic.Length}, tempNumStr={tempNumStr}, dad={dad}");

            //int dasd = 100;

            //float dda = dasd;


            //            string ddasdJson = @"
            //[
            //	{	""display_name"":""Tales of the Electric Romeo - Immediate Music.mp3"",
            //		""file_path"":""PresetBGM0.mp3"",
            //		""duration_second"":""130""
            //	},
            //	{	""display_name"":""金达莱花.mp3"",
            //		""file_path"":""PresetBGM1.mp3"",
            //		""duration_second"":""149""
            //	},
            //	{	""display_name"":""孙浩雨 - 像男人一样去战斗(DJ小鱼儿 Remix) - YT_Official.mp3"",
            //		""file_path"":""PresetBGM2.mp3"",
            //		""duration_second"":""170""
            //	},
            //	{	""display_name"":""孙浩雨 - 生命之枪.mp3"",
            //		""file_path"":""PresetBGM3.mp3"",
            //		""duration_second"":""362""
            //	}
            //]
            //";

            //            JSONNode jsonNode = JSON.Parse(ddasdJson);

            //            for (int i = 0; i < jsonNode.Count; i++)
            //            {
            //                RTBGMInfo rTBGMInfo = new RTBGMInfo();

            //                rTBGMInfo.szDisplayName = jsonNode[i]["display_name"].ToString();
            //                rTBGMInfo.szFilePath = jsonNode[i]["file_path"].ToString();
            //                rTBGMInfo.nDuration = int.Parse(jsonNode[i]["duration_second"].ToString());

            //                m_strBgmFileList.Add(rTBGMInfo);

            //            }

            //            Console.WriteLine($"dda={dda}, CVoiceChatClient.OnResponse, m_strBgmFileList.Count={m_strBgmFileList.Count}");
            //            Console.WriteLine($"CVoiceChatClient.OnResponse, m_strBgmFileList.1.szDisplayName={m_strBgmFileList[1].szDisplayName}");


            //string data = "dsadssdasdcxzvzxlvsdjirnurgnbvuefdwuejdfnjsdfneiujqiejiewmfksdnfsjkdnfdfdsfsdfwefwfvsvxcbxcbdsfrgsdgfs";

            //Console.WriteLine($"BillboardList.OnGetData, v0, data.Length={data.Length}, data 50--->{data.Substring(0, data.Length > 50 ? 50 : data.Length)}");


            //string filename = "wxfile//user/xxx.mp3";
            //string fileName = filename.Substring(filename.LastIndexOf("/")+1);
            //Console.WriteLine($"BillboardList.OnGetData, v0, fileName={fileName}, ");


            //m_PlaybackFileID.Add( "aaa" );
            //m_PlaybackFileID.Add( "bbb" );
            //m_PlaybackFileID.Add( "ccc" );


            //Console.WriteLine($"MainTest.RunThis, 1, m_PlaybackFileID.Length={m_PlaybackFileID.Count}");

            //m_PlaybackFileID.Remove("aaa");

            //Console.WriteLine($"MainTest.RunThis, 2, m_PlaybackFileID.Length={m_PlaybackFileID.Count}");


            //MyCodeChild.funcLgy();


            //bool dasdBool = true;


            //Console.WriteLine($"BillboardList.OnGetData, ------- dasdBool={dasdBool} -------------------------\n");


            //LogController testEnum = (LogController)dasd;

            //LogController testEnum1 = LogController.First;

            //if (testEnum == testEnum1)
            //{
            //    Console.WriteLine($"BillboardList.OnGetData, ------- testEnum={testEnum} -----true--------------------\n");
            //}
            //else
            //{
            //    Console.WriteLine($"BillboardList.OnGetData, ------- testEnum={testEnum} -----false--------------------\n");
            //}

            //string das= nameof( LogController.PersonDebug);

            //Console.WriteLine($"------- das={das} ----------\n");


            //var dsad = new MyCode[3];
            //Console.WriteLine($"MainTest.RunThis, --------dsad type={dsad.GetType()==typeof(MyCode[])}");

            //if (dsad.GetType() == typeof(MyCode[]))
            //{

            //}
            //if (dsad.GetType().BaseType == typeof(Array)) { 
            //    Console.WriteLine($"MainTest.RunThis, --------dsad type=true lgy");
            //    int counter = 0;
            //    foreach(var i in dsad)
            //    {
            //        Console.WriteLine(counter++); 
            //    } 
            //}
            //else
            //{

            //    Console.WriteLine($"MainTest.RunThis, --------dsad type=false lgy");
            //}





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
