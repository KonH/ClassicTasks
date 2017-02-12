using System;

namespace CSharp
{
    public class BubbleSort : ISort
    {
        public int[] Sort(int[] array)
        {
			for( int i = 0; i < array.Length; i++ ) {
				var changed = false;
				Console.WriteLine($"i = {i}");
				for( int j = array.Length - 1; j > i; j-- ) {
					Console.WriteLine($"j = {j}");
					if( array[j-1] > array[j] ) {
						var temp = array[j];
						array[j] = array[j-1];
						array[j-1] = temp;
						changed = true;
						Program.WriteArray(array);
					}
				}
				if( !changed ) {
					break;
				}
			}
			return array;
        }
    }
}