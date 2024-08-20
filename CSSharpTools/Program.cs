using System.Collections;

namespace CSSharpTools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CSharp_DotNet_Narwhal.CSSharpTool.Program.Main, Hello, World!");

            //new Program().RunThis();
            //new Sortings().RunThis(); 
            //new Test().RunThis();

            //LogModule.Instance.RunThis();
            LogModule.PersonDebug("I am testing PersonDebug log");
            LogModule.Warning("I am testing Warning log");
            LogModule.Error("I am testing error log");
            LogModule.LogPath = "";
        }

        
        public void RunThis()
        {
            ArrayList al = new ArrayList();
            al.Add("dasas");
            al.Add(100);
            string dasd = (string)al[0];
            Console.WriteLine("Hello World!" + dasd);



            Dictionary<string, string> dajduo = new Dictionary<string, string>();
            dajduo.Add("111", "dasdada");
            dajduo["111"] = "djijidasd";
            Console.WriteLine(dajduo["111"]);



            ReadOnlyDictionary<string, string > readOnlyDictionary = new ReadOnlyDictionary<string, string>();
            //readOnlyDictionary = new ReadOnlyDictionary<string, List<int>>();
            readOnlyDictionary.Add("222", "abcd");
            readOnlyDictionary.Add("333", "abcde");
            Console.WriteLine(readOnlyDictionary.Count+"  AAAAAA  "+readOnlyDictionary["222"]);
            //readOnlyDictionary["222"] = "abcdd";

        }
    }
}
