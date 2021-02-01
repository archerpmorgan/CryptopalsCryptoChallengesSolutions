using System.Net.NetworkInformation;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace solution_runner
{

    // a class for packaging a byte with the how likely it is that it was the cypher byte
    public class ScoredByte: IComparable {

        public double score { get; set; }
        public byte b { get; set; }
        public string ascii { get; set; }

        int IComparable.CompareTo(object obj)
        {
            ScoredByte o = (ScoredByte) obj;
            if (o.score == this.score){
                return 0;
            }
            else {
            return this.score < o.score ? -1 : 1;
            }
        }


        // frequencies of letters in english (source: https://www3.nd.edu/~busiforc/handouts/cryptography/letterfrequencies.html)
        static private Dictionary<char, double> charFreqsReference = new Dictionary<char, double> {
            { 'a', 8.4966 },
            { 'b', 10.56 },
            { 'c', 4.5388 },
            { 'd', 3.3844 },
            { 'e', 11.1607 },
            { 'f', 1.8121 },
            { 'g', 2.4705 },
            { 'h', 3.0034 },
            { 'i', 7.5448 },
            { 'j', 0.1965 },
            { 'k', 1.1016 },
            { 'l', 5.4893 },
            { 'm', 3.0129 },
            { 'n', 6.6544 },
            { 'o', 7.1635 },
            { 'p', 3.1671 },
            { 'q', 0.1962 },
            { 'r', 7.5809 },
            { 's', 5.7351 },
            { 't', 6.9509 },
            { 'u', 3.6308 },
            { 'v', 1.0074 },
            { 'w', 1.2899 },
            { 'x', 0.2902 },
            { 'y', 1.7779 },
            { 'z', 0.2722 },
        };

        // given a string, calculate a measure of how different the char frequencies are from what we would expect in an english sentence.
        private double getObservedDistance(string text){

            double accumulator = 0;
            Dictionary<char, int> characterCount = text.ToLower()
                .Where(c => Char.IsLetter(c))
                .GroupBy(c => c)
                .ToDictionary(k => k.Key, v => v.Count());
            foreach(KeyValuePair<char, double> entry in charFreqsReference)
            {
                // calculate observed freq for char in string
                var freq = characterCount.ContainsKey(entry.Key) ? ((double) characterCount[entry.Key] / (double) text.Length) : 0 ;
                accumulator += Math.Pow((freq - entry.Value), 2);
            }
            return accumulator;
        }

        public ScoredByte(byte arg, byte[] cyphertext) {
            b = arg; 

            // do the XOR, as XOR is its own inverse
            for (int i = 0; i < cyphertext.Length; i++) {
                cyphertext[i] = (byte) (cyphertext[i] ^ arg);
            }
            
            // convert to ascii?
            ascii = Encoding.ASCII.GetString(cyphertext);
            var chars = ascii.ToCharArray();

            score = getObservedDistance(ascii);
        }
    }
    class Program
    {
        
        static Dictionary<char, uint> map = new Dictionary<char, uint>();

        static byte HexCoupletToByte(char[] hexCouplet){
            //take the first nibble, shift it, add second nibble
            return (byte) ((map[hexCouplet[0]] << 4) + map[hexCouplet[1]]);
        }

        // assuming hex is an english string that has been XORd against one single byte character, find the most probable
        static char FindXORCipher(string hex) {

            char[] chars = hex.ToCharArray();

            // fill byte arrays with binary representation of the hex string 
            byte[] bytes = new byte[chars.Length / 2];
            for (int i = 0; i < chars.Length; i = i + 2) {
                bytes[(int)(i/2)] = HexCoupletToByte(
                    chars.Skip(i)
                    .Take(2)
                    .ToArray()
                );
            }
            List<ScoredByte> scores = new List<ScoredByte>();

            // there are only so many possible chars. do the XOR against each, score the text, sort the results, pick the top
            // do the XOR
            for (int i = 0; i < 256; i++) {
                scores.Add(new ScoredByte((byte)i, bytes));
            }
            
            scores.Sort();

            //should return all ties here, but its clearly the second one
            return (char) scores[1].b;
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

            var result = FindXORCipher("1b37373331363f78151b7f2b783431333d78397828372d363c78373e783a393b3736");
            var answer = 88;
            Debug.Assert( result == answer );
        }

    }
}
