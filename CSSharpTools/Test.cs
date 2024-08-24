using System;
using System.Collections.Generic;
using System.Text;

namespace CSSharpTools
{
    class Test
    {
        int MaxCount = 30000;

        public void RunThis()
        {
            Console.WriteLine("Test.RunThis!");
            //Console.WriteLine(Factorial(3));

            //Stack<int> sta = new Stack<int>();
            //sta.Push(0);
            //sta.Push(1);
            //sta.Push(2);
            //sta.Push(3);
            //sta.Push(4);
            //foreach (int i in sta) Console.WriteLine(i);
            //reverseStack(sta);
            //foreach (int i in sta) Console.WriteLine(i);


            //new Sortings().RunThis(); 





            //long start = Timer.DateTimeToLongTimeStamp();

            int couter = 0;
			string testStr = "";

            StringBuilder sb = new StringBuilder();

			long flag1 = DateTime.UtcNow.Ticks;
            for (int i = 0; i < MaxCount; i++) {
				//couter++;
				//if (couter == -1) { couter++; }
				//testStr = "Test.RunThis, i=-1";
				sb.Clear();
				sb.Append("Test.RunThis, i=");
				sb.Append(i.ToString());
				testStr = sb.ToString();
				//testStr = string.Format("Test.RunThis, i={0}", i);
			}

            long flag2 = DateTime.UtcNow.Ticks;

            for (int i = 0; i < MaxCount; i++)
            {
                Sub1.ThisStaticFunc("");
            }
            long flag2_1 = DateTime.UtcNow.Ticks;
            

            LogModule.Instance.Init(LogController.Close);


            long flag2_5 = DateTime.UtcNow.Ticks;
            for (int i = 0; i < MaxCount; i++)
            {
                LogModule.Info($"Test.RunThis, i=-1");
            }
            long flag2_6 = DateTime.UtcNow.Ticks;


            LogModule.Instance.Init(LogController.OutputToConsole + LogController.Info);


            long flag3 = DateTime.UtcNow.Ticks;
            for (int i = 0; i < MaxCount; i++)
            {
                //LogModule.Info($"Test.RunThis, i={i}");
                LogModule.Info($"Test.RunThis, i=-1");
            }
            long flag4 = DateTime.UtcNow.Ticks;


            //long end = Timer.DateTimeToLongTimeStamp();

            //Console.WriteLine($"Start here, time totle: -- ms, \n flag1={flag1}, \n flag2={flag2}, ns");

            Console.WriteLine($"Test.RunThis, testStr={testStr}, MaxCount: {MaxCount}, flag1={flag1}, + = {flag2-flag1}00 ns, CallFunction = {flag2_1 - flag2}00 ns, CallFunctionInit = {flag2_5- flag2_1}00 ns, " +
                $"CallFunctionBool = {flag2_6-flag2_5}00 ns, CallFunctionInit = {flag3 - flag2_6}00 ns, CallFunctionPrint = {flag4-flag3}00 ns.");

            //Sub1 sub = new Sub1();
            //sub.Say();
        }

        /// <summary>
        /// n 的阶乘
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private int Factorial(int n)
        {
            if (n < 2) return n;
            return n * Factorial(n - 1);
        }


        /// <summary>
        /// 不借助辅助数组倒转栈
        /// </summary>
        /// <param name="sta"></param>
        private void reverseStack(Stack<int> sta)
        {
            if (sta.Count == 0) return;
            int val = getLast(sta);   //  先用 递归 取出最后一个
            reverseStack(sta);   // (利用函数) 递归倒转
            sta.Push(val);
        }
        /// <summary>
        /// 递归取出最后一个对象
        /// </summary>
        /// <param name="sta"></param>
        /// <returns></returns>
        private int getLast(Stack<int> sta)
        {
            int valTop = sta.Pop();
            if (sta.Count == 0) return valTop;
            else
            {
                int last = getLast(sta);     // 递归取最低元素
                sta.Push(valTop);            // 将取出的不是栈底元素放回栈中
                return last;
            }
        }


   

    }


    public class Base
    {

        public static void ThisStaticFunc(string value)
        {

        }

        public Base()
        {
            Say();
        }

        public virtual void Say() {
            Console.WriteLine("Base: Hello, World !");
        }

        public virtual void Eat() { }

    }

    public class Sub1 : Base
    {
        public Sub1()
        {
            Say();
        }

        public override  void Say() {
            Console.WriteLine("Sub1: Hello, World !");
        }

        public override void Eat() { }
    }

    public class Sub2 
    {
        public void Say() { }

        public void Eat() { }
    }

    public class Sub3 
    {
        public void Say() { }

        public void Eat() { }
    }




}
