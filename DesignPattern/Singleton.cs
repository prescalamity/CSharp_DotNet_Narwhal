using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern
{

    /// <summary>
    /// 懒汉式
    /// </summary>
    public class Singleton
    {

        private static object locker = new object();
        private static volatile Singleton instance;


		public string name = "DesignPattern.Singleton";



        // 构造方法 private 化
        private Singleton()
        {
            if (Singleton.instance != null)
            {
                throw new ArgumentException("不允许创建多个单例!");  //防止使用反射创建多个实例
            }
        }

        // 得到 Singleton 的实例(唯一途径）
        public static Singleton getInstance()
        {
            if (instance == null)       // 检查实例是否存在，不存在则进入下一步
            {                 
                lock (locker)           // 防止多个线程同时进入创建实例
                {                    
                    if (instance == null)
                    {
                        instance = new Singleton();
                    }
                }
            }
            return instance;
        }
        
    }


}

