using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class BinarySearch
    {
        public double TwoArray(int[] array1, int[] array2)
        {
            int[] smallArray;
            int[] largeArray;
            if (array1.Length > array2.Length)
            {
                smallArray = array2;
                largeArray = array1;
            }
            else
            {
                smallArray = array1;
                largeArray = array2;
            }

            return TwoArray(smallArray, largeArray, smallArray.Length + largeArray.Length + 1, smallArray.Length / 2);
        }

        public double TwoArray(int[] smallArray, int[] largeArray, int arraysSize, int currentIndex)
        {
            int largeIndex = arraysSize / 2 - currentIndex;
            var smallArrayMin = smallArray[currentIndex];
            var smallArrayMax = smallArray[currentIndex + 1];
            var largeArrayMin = largeArray[largeIndex];
            var largeArrayMax = largeArray[largeIndex];

            var computedMedian = (Math.Max(smallArrayMin, largeArrayMin) + Math.Max(smallArrayMax, largeArrayMax)) / 2;

            return computedMedian;
        }
    }
}
