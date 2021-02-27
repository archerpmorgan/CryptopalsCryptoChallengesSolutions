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

    // a class for packaging a byte with the how likely it is that it was the XOR cypher byte
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
            { '\'', 0 },
            { '"', 0 },
            { ',', 0 },
            { '.', 0 },
        };

        static private Dictionary<char, uint> map = new Dictionary<char, uint> {
            {'0', 0b0000},
            {'1', 0b0001},
            {'2', 0b0010},
            {'3', 0b0011},
            {'4', 0b0100},
            {'5', 0b0101},
            {'6', 0b0110},
            {'7', 0b0111},
            {'8', 0b1000},
            {'9', 0b1001},
            {'a', 0b1010},
            {'b', 0b1011},
            {'c', 0b1100},
            {'d', 0b1101},
            {'e', 0b1110},
            {'f', 0b1111},
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

        private byte HexCoupletToByte(char[] hexCouplet){
            //take the first nibble, shift it, add second nibble
            return (byte) ((map[hexCouplet[0]] << 4) + map[hexCouplet[1]]);
        }

        public ScoredByte(byte arg, string keyHexExpanded, string cyphertextHex) {

            b = arg; 

            char[] keychars = keyHexExpanded.ToCharArray();
            char[] cypherTextChars = cyphertextHex.ToCharArray();

            // fill byte arrays with binary representation of the hex string 
            byte[] keyBytes = new byte[keychars.Length / 2];
            for (int i = 0; i < keychars.Length; i = i + 2) {
                keyBytes[(int)(i/2)] = HexCoupletToByte(
                    keychars.Skip(i)
                    .Take(2)
                    .ToArray()
                );
            }
            byte[] cypherTextBytes = new byte[cypherTextChars.Length / 2];
            for (int i = 0; i < cypherTextChars.Length; i = i + 2) {
                cypherTextBytes[(int)(i/2)] = HexCoupletToByte(
                    cypherTextChars.Skip(i)
                    .Take(2)
                    .ToArray()
                );
            }

            var xorData = new byte[cypherTextBytes.Length];
        
            for (var i = 0; i < xorData.Length; i++)
                xorData[i] = (byte) (cypherTextBytes[i] ^ keyBytes[i]);            // convert to ascii?
            ascii = Encoding.ASCII.GetString(xorData);

            score = getObservedDistance(ascii);
        }
    }
    class Program
    {
        // assuming hex is an english string that has been XORd against one single byte character, find the most probable
        static List<ScoredByte> FindXORCipher(string hexCypherText) {

            List<ScoredByte> scores = new List<ScoredByte>();

            // there are only so many possible chars. do the XOR against each, score the text, sort the results, pick the top
            for (int i = 0; i < 127; i++) {
                var keyAsHex = Convert.ToString(i, 16);
                var expandedKey = string.Empty;
                while (expandedKey.Length < hexCypherText.Length) {
                    expandedKey += keyAsHex;
                }
                scores.Add(new ScoredByte((byte)i, expandedKey, hexCypherText));
            }
            return scores;
        }

        static void Main(string[] args)
        {  
            var path = "Input.txt";
            string text = System.IO.File.ReadAllText(path);
            var lines = text.Split('\n');
            var scores = new List<ScoredByte>();

            foreach (var line in lines) {
                scores.AddRange(FindXORCipher(line));
            }
            scores.Sort();
            var answer = scores[0];
            Debug.Assert(answer.ascii == "Now that the party is jumping\n");
        }
    }
}