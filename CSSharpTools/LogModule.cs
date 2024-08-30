/**
 * 
 * 日志模块，
 * 实现输出指定 等级的日志 到 指定文件中
 * 
 * (暂时缺少具体路径文件流的实现)
 * 
 */



using System.Text;
/**
* 
* 日志模块，
* 实现输出指定 等级的日志 到 指定文件中
* 
* (暂时缺少具体路径文件流的实现)
* 
*/
namespace CSSharpTools
{
    public class LogModule
    {

		#region 模块对外接口

		public static string TAG = "PersonDebug, ";

        /// <summary>
        /// 包含字符串拼接，不应该在发布版中 运行，
        /// 初衷是在查找某个 bug 的时候使用，
        /// </summary>
        public static void PersonDebug( string content, params object[] args)
        {
			content = TAG + string.Format(content, args);
			Instance.ControllerPrintLog(content, LogController.PersonDebug);
        }


		/// <summary>
		/// 普通的不带字符串连接的日志，resStr = "str1";
		/// </summary>
		public static void LogWihtoutStrConnect(string content)
        {
            Instance.ControllerPrintLog(content, LogController.LogWihtoutStrConnect);
        }

        /// <summary>
        /// 普通的带有字符串连接的日志，例如：resStr = "str1" + data;
        /// </summary>
        /// <param name="content"></param>
        public static void LogWithStrConnect(string content, params object[] args)
        {
			content = string.Format(content, args);

			Instance.ControllerPrintLog(content, LogController.LogWithStrConnect);

        }

        public static void Warning(string content)
        {
            Instance.ControllerPrintLog(content, LogController.Warning);

        }

        public static void ProgramImportantNode(string content)
        {
            Instance.ControllerPrintLog(content, LogController.ProgramImportantNode);

        }

        public static void Error(string content)
        {
            Instance.ControllerPrintLog(content, LogController.Error);

        }

        #endregion

		/// <summary>
		/// 日志锁
		/// </summary>
		public static object lockLogQueue = new object();


        private string _LogPath = string.Empty;
        /// <summary>
        /// 指定日志输出到哪个文件
        /// </summary>
        public string LogPath
        {
            get { return _LogPath; }
            private set { _LogPath = value; }
        }


        private int _NeedOutputLogs = LogController.OutputToConsole + LogController.Error + LogController.ProgramImportantNode;
        /// <summary>
        /// 指定 LogController中 哪些等级的日志可以输出，
        /// 默认是错误和程序流程节点日志
        /// </summary>
        public int NeedOutputLogs
        {
            get { return _NeedOutputLogs; }
            private set { _NeedOutputLogs = value; }
        }

        private int _LogMaxCount = int.MaxValue;
        /// <summary>
        /// 最大的日志行数
        /// </summary>
        public int LogMaxCount
        {
            get { return _LogMaxCount; }
            private set { _LogMaxCount = value; }
        }


		private Queue<string> logsWillOutputToFile = new Queue<string>();


		/// <summary>
		/// 当前已输出日志的行数
		/// </summary>
        private int logCount = 0;

        /// <summary>
        /// 外部不能 new 本类
        /// </summary>
        private LogModule()
        {
            //RunThis();
        }


        private static LogModule instance = null;

        public static LogModule Instance {

            get
            {
                if (instance == null) 
                { 
                    instance = new LogModule();
                }

                return instance; 
            }

            set { instance = value; }
        }


        public void Init(int needOutputLogs, string logPath="")
        {
            ///Console.WriteLine($"LogModule.Init, logPath={logPath}, needOutputLogs={needOutputLogs}");

            //_NeedOutputLogs += LogController.PersonDebug;

            //if ( (_NeedOutputLogs & LogController.PersonDebug) == 0)
            //{
            //    Console.WriteLine($"LogModule.RunThis, {_NeedOutputLogs} not contain {LogController.PersonDebug}");
            //}
            //else
            //{
            //    Console.WriteLine($"LogModule.RunThis, {_NeedOutputLogs} contain {LogController.PersonDebug}");
            //}
            NeedOutputLogs = needOutputLogs;


			if ((_NeedOutputLogs & LogController.OutputToFile) != 0)
			{
				InitLogFile(logPath);
			}

		}


        private void ControllerPrintLog(string content, int logLevel)
        {
            if ( _NeedOutputLogs == 0) return;

            if ((_NeedOutputLogs & logLevel) == 0) return;

            if (logCount >= (_LogMaxCount-1))
            {
                if(logCount == (_LogMaxCount - 1))
                {
                    // 输出已超过最大的日志行数
                    OutputLog($"Warning: logs over {_LogMaxCount}, and no longer print after logs.");
                }

                return;
            }

            OutputLog(content);

        }

        private void OutputLog(string content)
        {

            // 需要输出到控制台，且包含指定日志
            if ((_NeedOutputLogs & LogController.OutputToConsole) != 0)
            {
                Console.WriteLine(content);
                //Debug.Info(content);
            }

            // 需要输出到文件，且包含指定日志
            if ((_NeedOutputLogs & LogController.OutputToFile) != 0)
            {
				// 输出到外部文件中
				lock (lockLogQueue)
				{
					logsWillOutputToFile.Enqueue(content);
				}

			}

            if (((_NeedOutputLogs & LogController.OutputToConsole) != 0) || ((_NeedOutputLogs & LogController.OutputToFile) != 0))
            {
                logCount++;
            }

        }


		/// <summary>
		/// 初始化日志文件
		/// </summary>
		private void InitLogFile(string logPath)
		{
			if (!string.IsNullOrEmpty(logPath))
			{
				LogPath = logPath;
			}
			else
			{
				LogPath = "D:\\myLogFile.log";
			}

			// 如果是在 windows Android iOS 平台，需要输出到文件，且包含指定日志
			if ((_NeedOutputLogs & LogController.OutputToFile) != 0)
			{
				// 创建新的线程
				Thread myThread = new Thread(new ThreadStart(_OutputLogToFileInWindowsAndroidIos));
				myThread.Start();

			}
		}

		/// <summary>
		/// 必须是新的线程调用，在 windows Android iOS 平台
		/// </summary>
		private void _OutputLogToFileInWindowsAndroidIos()
		{
			// 备份文件

			string _log = "";
			while (true)
			{
				if (logsWillOutputToFile.Count > 0 && !string.IsNullOrEmpty(LogPath))
				{
					lock(lockLogQueue) 
					{
						_log = logsWillOutputToFile.Dequeue();
					}
					// 拼接字符串

					File.AppendAllText(LogPath, string.Format("{0}{1}", _log, Environment.NewLine), new UTF8Encoding(false));

				}

				//Thread.Sleep(30); // 休眠30毫秒
			}
		}


    }

	/// <summary>
	/// 单个日志元素
	/// </summary>
	public class LogContent
	{
		int LogLevel;
		string tag="";
		string logFormet = "";
		object[] args = null;
		long time;
		long frameCount;
	}

    ///// <summary>
    ///// 日志等级，一共8个，共 255个 组合
    ///// 1111 1111 = 255
    ///// 1000 0000 = 128  = 2^7
    ///// 0000 0001 = 1    = 2^0
    ///// </summary>
    //public enum LogController
    //{

    //    First = 1,

    //    Second = 2,

    //    Third = 4,

    //    PersonDebug = 8,

    //    NetOrDataTransmit = 16,

    //    Warning = 32,

    //    ProgramImportantNode = 64,

    //    Error = 128,

    //}


    /// <summary>
    /// 日志等级，一共8个，共 255个 组合
    /// 1111 1111 = 255
    /// 1000 0000 = 128  = 2^7
    /// 0000 0001 = 1    = 2^0
    /// </summary>
    public class LogController
    {
        /// <summary>
        /// 关闭日志打印
        /// </summary>
        public const int Close = 0;

        /// <summary>
        /// 是否输出到控制台
        /// </summary>
        public const int OutputToConsole = 1;

        /// <summary>
        /// 是否输出到文件
        /// </summary>
        public const int OutputToFile = 2;

        /// <summary>
        /// 初衷是在查找某个 bug 的时候使用
        /// </summary>
        public const int PersonDebug = 4;

        /// <summary>
        /// 普通的不带字符串连接的日志
        /// </summary>
        public const int LogWihtoutStrConnect = 8;

        /// <summary>
        /// 普通的带有字符串连接的日志
        /// </summary>
        public const int LogWithStrConnect = 16;

        public const int Warning = 32;

        public const int ProgramImportantNode = 64;

        public const int Error = 128;


    }

}


