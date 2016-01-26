using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Utility
    {
        public static byte[] StringToBytes(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static string BytesToString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes); 
        }
    }
}
