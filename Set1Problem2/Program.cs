using System.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace solution_runner
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
        static string XORHex(string hex1, string hex2) {

            char[] chars1 = hex1.ToCharArray();
            char[] chars2 = hex2.ToCharArray();

            // some assumptions
            Debug.Assert(chars1.Length % 2 == 0 && chars2.Length % 2 == 0 && chars1.Length == chars2.Length);

            // fill byte arrays with binary representation of the hex string 
            byte[] bytes1 = new byte[chars1.Length / 2];
            for (int i = 0; i < chars1.Length; i = i + 2) {
                bytes1[(int)(i/2)] = HexCoupletToByte(
                    chars1.Skip(i)
                    .Take(2)
                    .ToArray()
                );
            }
            byte[] bytes2 = new byte[chars2.Length / 2];
            for (int i = 0; i < chars2.Length; i = i + 2) {
                bytes2[(int)(i/2)] = HexCoupletToByte(
                    chars2.Skip(i)
                    .Take(2)
                    .ToArray()
                );
            }

            // do the XOR
            for (int i = 0; i < bytes1.Length; i++) {
                bytes1[i] = (byte) (bytes1[i] ^ bytes2[i]);
            }

            return Convert.ToHexString(bytes1);
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

            var result = XORHex("1c0111001f010100061a024b53535009181c","686974207468652062756c6c277320657965");
            var answer = "746865206B696420646F6E277420706C6179";
            Debug.Assert( result == answer );
        }

    }
}
