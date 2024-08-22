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


            //LogModule.Instance.Init(LogController.OutputToConsole + LogController.Log);


            long start = Timer.DateTimeToLongTimeStamp();

            long flag1 = DateTime.UtcNow.Ticks;

            int couter = 0;

            for (int i = 0; i < MaxCount; i++) {
                //couter++;
                //LogModule.Log($"Test.RunThis, i={i}");
            }


            long end = Timer.DateTimeToLongTimeStamp();

            long flag2 = DateTime.UtcNow.Ticks;

            Console.WriteLine($"Start here, time totle: {end-start} ms, \n flag1={flag1}, \n flag2={flag2}");

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
