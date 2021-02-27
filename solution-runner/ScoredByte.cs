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

        public ScoredByte(byte arg, byte[] cypherTextBytes) {

            b = arg;
            var xorData = new byte[cypherTextBytes.Length];
        
            for (var i = 0; i < xorData.Length; i++)
                xorData[i] = (byte) (cypherTextBytes[i] ^ arg);            // convert to ascii?
            ascii = Encoding.ASCII.GetString(xorData);

            score = getObservedDistance(ascii);
        }
    }
}