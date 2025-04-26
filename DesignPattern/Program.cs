using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern
{
    internal class Program
    {
        static void Main(string[] args)
        {

			Console.WriteLine($"Program.Main!");

			Console.WriteLine($"Program.Main, name=" +Singleton.getInstance().name);

			FactoryPattern factoryPattern = new FactoryPattern();

			Console.WriteLine($"Program.Main, name=" + factoryPattern.main());

		}

    }

}
