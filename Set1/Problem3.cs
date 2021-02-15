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

        // frequencies of letters in english (http://www.macfreek.nl/memory/Letter_Distribution)
        static private Dictionary<char, double> charFreqsReference = new Dictionary<char, double> {
            { ' ', .1828846265 },
            { 'a', .06532 },
            { 'b', .01258 },
            { 'c', .02234 },
            { 'd', .03282 },
            { 'e', .10266 },
            { 'f', .01983 },
            { 'g', .01624 },
            { 'h', .04978 },
            { 'i', .05668 },
            { 'j', .00097 },
            { 'k', .00561 },
            { 'l', .03317 },
            { 'm', .02206 },
            { 'n', .05712 },
            { 'o', .06159 },
            { 'p', .01504 },
            { 'q', .00084 },
            { 'r', .04987 },
            { 's', .05317 },
            { 't', .07517 },
            { 'u', .02276 },
            { 'v', .00796 },
            { 'w', .01704 },
            { 'x', .00141 },
            { 'y', .01427 },
            { 'z', .00051 },
        };

        // given a string, calculate a measure of how different the char frequencies are from what we would expect in an english sentence.
        private double getObservedDistance(string text){

            double accumulator = 0;
            Dictionary<char, int> characterCount = text.ToLower()
                .GroupBy(c => c)
                .ToDictionary(k => k.Key, v => v.Count());
            foreach(KeyValuePair<char, int> entry in characterCount)
            {
                var expected = charFreqsReference.ContainsKey(entry.Key) ? (charFreqsReference[entry.Key]*text.Length): 0;
                accumulator += Math.Abs(entry.Value - expected);
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

            return (char) scores[0].b;
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