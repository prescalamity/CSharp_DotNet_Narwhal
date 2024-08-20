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

        /// <summary>
        /// 初衷是在查找某个 bug 的时候使用
        /// </summary>
        public static void PersonDebug(string content)
        {

            Instance.PrintLog(content, LogLevel.PersonDebug);
        }


        public static void NetworkOrDataTransmit(string content)
        {
            Instance.PrintLog(content, LogLevel.NetworkOrDataTransmit);

        }

        public static void Warning(string content)
        {
            Instance.PrintLog(content, LogLevel.Warning);

        }

        public static void ProgramImportantNode(string content)
        {
            Instance.PrintLog(content, LogLevel.ProgramImportantNode);

        }

        public static void Error(string content)
        {
            Instance.PrintLog(content, LogLevel.Error);

        }

        private static string _LogPath = string.Empty;
        /// <summary>
        /// 指定日志输出到哪个文件
        /// </summary>
        public static string LogPath
        {
            set { _LogPath = value; }
        }


        private static int _NeedOutputLogGrade = LogLevel.Error + LogLevel.ProgramImportantNode;
        /// <summary>
        /// 指定 LogLevel中 哪些等级的日志可以输出，
        /// 默认是错误和程序流程节点日志
        /// </summary>
        public static int NeedOutputLogGrade
        {
            set { _NeedOutputLogGrade = value; }
        }

        #endregion


        /// <summary>
        /// 外部不能 new 本类
        /// </summary>
        private LogModule()
        {
            RunThis();
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


        private void RunThis()
        {
            Console.WriteLine($"LogModule.RunThis, m_Path={_LogPath}");

            //_NeedOutputLogGrade += LogLevel.PersonDebug;

            //if ( (_NeedOutputLogGrade & LogLevel.PersonDebug) == 0)
            //{
            //    Console.WriteLine($"LogModule.RunThis, {_NeedOutputLogGrade} not contain {LogLevel.PersonDebug}");
            //}
            //else
            //{
            //    Console.WriteLine($"LogModule.RunThis, {_NeedOutputLogGrade} contain {LogLevel.PersonDebug}");
            //}

        }


        private void PrintLog(string content, int logGrade)
        {
            if ((_NeedOutputLogGrade & logGrade) != 0)
            {
                Console.WriteLine(content);

                // 输出到外部文件中

            }

        }




    }



    ///// <summary>
    ///// 日志等级，一共8个，共 255个 组合
    ///// 1111 1111 = 255
    ///// 1000 0000 = 128  = 2^7
    ///// 0000 0001 = 1    = 2^0
    ///// </summary>
    //public enum LogLevel
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
    public class LogLevel
    {

        public const int First = 1; 

        public const int Second = 2;

        public const int Third = 4;

        /// <summary>
        /// 初衷是在查找某个 bug 的时候使用
        /// </summary>
        public const int PersonDebug = 8;

        public const int NetworkOrDataTransmit = 16;

        public const int Warning = 32;

        public const int ProgramImportantNode = 64;

        public const int Error = 128;

    }

}


