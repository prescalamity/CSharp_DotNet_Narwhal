using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnCSharp
{
    public class MainTest
    {

        public void RunThis() {

            Console.WriteLine("MainTest.RunThis!");



            MyCodeChild.funcLgy();



        }





    }


    public class MyCode {


        public static void funcLgy()
        {
            Console.WriteLine($"Hello, World! in 2024-7-3, in MyCode.funcLgy, {new System.Diagnostics.StackTrace().ToString()}");

            Console.WriteLine($"Hello, World! in 2024-7-3, in MyCode.funcLgy, {DateTime.Now.ToString("yyyy/MM/dd_HH:mm:ss_fff")}");

            object modelItem = null;

            Console.WriteLine($"Hello, World! in 2024-7-3, in MyCode.funcLgy, modelItem == null:{modelItem == null}");

            Console.WriteLine($"Hello, World! in 2024-7-3, in MyCode.funcLgy, isBoolRes:{modelItem == null}");

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
