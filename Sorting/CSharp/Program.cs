using System;
using System.Text;

namespace CSharp
{
	public interface ISort {
		int[] Sort(int[] array);
	}

    public class Program
    {
        public static void Main(string[] args)
        {
			var array = new int[5] {3, 1, 8, -5, 0};
            Console.WriteLine("Initial array:");
			WriteArray(array);
			var sorter = new BubbleSort();
			Console.WriteLine("Process:");
			var sortedArray = sorter.Sort(array);
			Console.WriteLine("Sorted array:");
			WriteArray(sortedArray);
        }

		public static void WriteArray(int[] array) 
		{
			var sb = new StringBuilder();
			foreach( var item in array ) 
			{
				sb.Append(item);
				sb.Append(", ");
			}
			sb.Remove(sb.Length - 2, 2);
			Console.WriteLine(sb.ToString());
		}
    }
}
