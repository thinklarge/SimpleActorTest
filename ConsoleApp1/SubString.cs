using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class SubString
    {
        
        public int LengthOfLongestSubstring(string s)
        {
            var hash = new HashSet<char>();
            var queue = new Queue<char>();
            int max = 0;

            foreach (var ch in s)
            {
                if (!hash.Contains(ch))
                {
                    hash.Add(ch);
                    queue.Enqueue(ch);
                    max = Math.Max(max, queue.Count);
                }
                else
                {
                    var old = queue.Dequeue();
                    while (old != ch)
                    {
                        hash.Remove(old);
                        old = queue.Dequeue();
                    }

                    hash.Add(ch);
                    queue.Enqueue(ch);
                }
            }

            return max;
        }

        public int LengthOfLongestPalendrome(string s)
        {
            var hash = new HashSet<char>();
            var queue = new Queue<char>();
            int max = 0;
            "avabbavaacdsdmnghjy"

            foreach (var ch in s)
            {
                if (!hash.Contains(ch))
                {
                    hash.Add(ch);
                    queue.Enqueue(ch);
                    max = Math.Max(max, queue.Count);
                }
                else
                {
                    var old = queue.Dequeue();
                    while (old != ch)
                    {
                        hash.Remove(old);
                        old = queue.Dequeue();
                    }

                    hash.Add(ch);
                    queue.Enqueue(ch);
                }
            }

            return max;
        }
    }
}
