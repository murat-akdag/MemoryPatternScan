using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Jupiter;
namespace MemoryPatternScan
{
    class Program
    {
      
        static void Main(string[] args)
        {
            var test = PatternScan((IntPtr)0x7FF6A81A3A50, (IntPtr)0x7FF6A81A53CF, "01 ??");
            Console.ReadKey();
        }

        public static List<IntPtr> PatternScan(IntPtr firstAddr, IntPtr SecondAddr, string pattern)
        {
            MemoryModule memoryModule=new MemoryModule(1212);
            var patternAddresses = new List<IntPtr>();
            pattern = pattern.Replace("??", "..");
            //for regex library "??" replaced with ".."

            var range = SecondAddr.ToInt64() - firstAddr.ToInt64();
            //length between the second address and the first address

            var index = Convert.ToInt32(range / 1000);
            //index for "for" loop. Example: range = 46352 -> index=46
            //So we will memory read 46x1000 with the for loop.

            var carry = Convert.ToInt32(range - (index * 1000));
            //46352-46*1000=352 -> We will read the bytes remaining after 46x1000 memory reads.

            var sb = new StringBuilder();
            //We add the bytes of hex type to the stringbuilder.

            if (index > 0)
            {
                for (int i = 0; i < index; i++)
                {
                    var value = memoryModule.ReadVirtualMemory(firstAddr + (1000 * i), 1000);
                    //1000 byte memory read single loop

                    sb.Append(BitConverter.ToString(value).Replace("-", " "));
                    //converting the incoming bytes to hex code. Example: 01 00 0B....

                    sb.Append(" ");

                }
            }
            var endvalue = memoryModule.ReadVirtualMemory(firstAddr + (1000 * index), carry + 1);
            sb.Append(BitConverter.ToString(endvalue).Replace("-", " "));
            //remaining bytes are read

            sb.Append(" ");
            /////////////////////////////////////////////////////////////////////////////
            var m = Regex.Match(sb.ToString(), pattern, RegexOptions.IgnoreCase);
            while (m.Success)
            {
                patternAddresses.Add(firstAddr + (m.Index / 3));
                m = m.NextMatch();
            }
            //find all pattern
            return patternAddresses;
        }
    }
}
