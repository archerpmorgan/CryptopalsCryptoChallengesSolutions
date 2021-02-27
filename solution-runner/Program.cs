using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Globalization;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Runtime.ExceptionServices;

namespace solution_runner
{
    public class ScoredKeySize : IComparable
    {

        public double score { get; set; }
        public int keySize { get; set; }

        int IComparable.CompareTo(object obj)
        {
            ScoredKeySize o = (ScoredKeySize) obj;
            if (o.score == this.score)
            {
                return 0;
            }
            else
            {
                return this.score < o.score ? -1 : 1;
            }
        }
    }

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
            var path = @"C:\Users\armorgan\github\CryptopalsCryptoChallengesSolutions\solution-runner\Input.txt";
            string text = System.IO.File.ReadAllText(path);

            var keysize = findKeySize(text);

            Debug.Assert(4==37);
        }

        private static int findKeySize(string textBase64)
        {
            List<ScoredKeySize> l = new List<ScoredKeySize>();
            byte[] bytes = System.Convert.FromBase64String(textBase64);

            //try candidates and update our choice if better 
            for (int ks = 2; ks < 41; ks++)
            {
                // take summed hamming distance of first chunk of size ks with next 5 chunks
                double accumulator = 0;
                var firstChunk = bytes
                        .Take(ks)
                        .ToArray();

                for (int i = 1; i<10; i++)
                {
                    var chunk = bytes.Skip(i * ks)
                        .Take(ks)
                        .ToArray();

                    accumulator += (double) hammingDistance(firstChunk, chunk) / ks;
                }

                l.Add(new ScoredKeySize
                        {
                            score = accumulator,
                            keySize = ks
                        }
                    );
         
                accumulator = 0;
            }

            l.Sort();

            return l[0].keySize;
        }
    }
}
