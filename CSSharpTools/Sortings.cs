using System;
using System.Collections.Generic;
using System.Text;

namespace CSSharpTools
{
    class Sortings
    {

        public void RunThis()
        {

            int[] array = new[] { 6, -9, 2, -1, 9, 10, 15, 10, 5, 8 };
            foreach (int item in array)Console.Write(item + " ");
            Console.WriteLine();

            QuickSort(array, 0, array.Length - 1);
            foreach (int item in array) 
                Console.Write(item + " ");

            Console.ReadLine();

        }

        /// <summary>
        /// 快速排序
        /// </summary>
        /// <param name="array"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        public void QuickSort(int[] array, int low, int high)
        {
            if (array.Length < 2 || low >= high) return ;

            int index= SortUnit(array,  low,  high);

            QuickSort(array, low, index - 1);

            QuickSort(array, index + 1,high);
        }
        private int SortUnit(int[] array, int low, int high)
       {
            int key = array[low];

            while (low < high)
            {
                while (low < high && array[high] >= key) high--;
                array[low] = array[high];
                while (low < high && array[low] < key) low++;
                array[high] = array[low];
            }

            array[low] = key;

            return high;
        }
    }


}
