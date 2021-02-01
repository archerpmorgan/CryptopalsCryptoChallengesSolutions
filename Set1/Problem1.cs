using System.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Set1Problem1
{
    class Program
    {
        
        static Dictionary<char, uint> map = new Dictionary<char, uint>();

        static byte HexCoupletToByte(char[] hexCouplet){
            //take the first nibble, shift it, add second nibble
            return (byte) ((map[hexCouplet[0]] << 4) + map[hexCouplet[1]]);
        }

        // given three bytes, represent as four base64 chars in a string
        static string ByteTripletToBase64Quad(byte[] triplet) {
            return Convert.ToBase64String(triplet);
        }
        static string ConvertHexToBase64(string hex) {

            char[] chars = hex.ToCharArray();
            Debug.Assert(chars.Length % 2 == 0);

            // fill byte array with binary representation of the hex string 
            byte[] bytes = new byte[chars.Length / 2];
            for (int i = 0; i < chars.Length; i = i + 2) {
                bytes[(int)(i/2)] = HexCoupletToByte(
                    chars.Skip(i)
                    .Take(2)
                    .ToArray()
                );
            }

            // take triplets of the byte array and convert them to three base64 characters (sextets), add them to string builder to build return value
            Debug.Assert(bytes.Length % 3 == 0);
            StringBuilder sb = new StringBuilder("", hex.Length);

            for (int i = 0; i < bytes.Length; i = i + 3) {
                sb.Append(ByteTripletToBase64Quad(
                    bytes.Skip(i)
                    .Take(3)
                    .ToArray()
                ));
            }

            return sb.ToString();
        }

        static void Main(string[] args)
        {  
            // for parsing the hex string
            map.Add('0', 0b0000);
            map.Add('1', 0b0001);
            map.Add('2', 0b0010);
            map.Add('3', 0b0011);
            map.Add('4', 0b0100);
            map.Add('5', 0b0101);
            map.Add('6', 0b0110);
            map.Add('7', 0b0111);
            map.Add('8', 0b1000);
            map.Add('9', 0b1001);
            map.Add('a', 0b1010);
            map.Add('b', 0b1011);
            map.Add('c', 0b1100);
            map.Add('d', 0b1101);
            map.Add('e', 0b1110);
            map.Add('f', 0b1111);

            var result = ConvertHexToBase64("49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d");
            var answer = "SSdtIGtpbGxpbmcgeW91ciBicmFpbiBsaWtlIGEgcG9pc29ub3VzIG11c2hyb29t";
            Debug.Assert(result == answer);
        }
    }
}
