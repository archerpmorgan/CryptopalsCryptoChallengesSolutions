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
                d += a[i] == b[i] ? 0 : 1;
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
            var text = "Burning 'em, if you ain't quick and nimble\nI go crazy when I hear a cymbal";
            var key = "ICE";

            var cyphertext = repeatingKeyXOR(key, text);
            // Debug.Assert();
        }
    }
}
