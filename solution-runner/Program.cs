using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Globalization;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace solution_runner
{
    class Program
    {
        static int hammingDistance(byte[] a, byte[] b){
            int d = 0;
            for (int i = 0; i < a.Length; i++) {
                d += (a[i] & 0b_1000_0000) == (b[i] & 0b_1000_0000) ? 0 : 1;
                d += (a[i] & 0b_0100_0000) == (b[i] & 0b_0100_0000) ? 0 : 1;
                d += (a[i] & 0b_0010_0000) == (b[i] & 0b_0010_0000) ? 0 : 1;
                d += (a[i] & 0b_0001_0000) == (b[i] & 0b_0001_0000) ? 0 : 1;
                d += (a[i] & 0b_0000_1000) == (b[i] & 0b_0000_1000) ? 0 : 1;
                d += (a[i] & 0b_0000_0100) == (b[i] & 0b_0000_0100) ? 0 : 1;
                d += (a[i] & 0b_0000_0010) == (b[i] & 0b_0000_0010) ? 0 : 1;
                d += (a[i] & 0b_0000_0001) == (b[i] & 0b_0000_0001) ? 0 : 1;
            }
            return d;
        }
        static string ByteArrayToHexString(byte[] ba)
        {
        StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
        }
        static string repeatingKeyXOR(string key, string text) {

            var expandedKey = string.Empty;
            while (expandedKey.Length < text.Length) {
                expandedKey += key;
            }
            byte[] keyBytes = Encoding.ASCII.GetBytes(expandedKey);
            byte[] textBytes = Encoding.ASCII.GetBytes(text);
            var xorData = new byte[textBytes.Length];

            for (var i = 0; i < xorData.Length; i++)
                xorData[i] = (byte) (textBytes[i] ^ keyBytes[i]);  

            return ByteArrayToHexString(xorData);
        }

        static void Main(string[] args)
        {  
            var keysize = 2;

            int result = hammingDistance(Encoding.ASCII.GetBytes(str1),Encoding.ASCII.GetBytes(str2));
            Debug.Assert(result==37);
        }
    }
}
