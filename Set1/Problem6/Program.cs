using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Problem6;

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
        static string repeatingKeyXOR(byte[] keyBytes, string textBase64) {

            var expandedKey = string.Empty;
            var keyString = Encoding.ASCII.GetString(keyBytes);
            byte[] textBytes = System.Convert.FromBase64String(textBase64);
            while (expandedKey.Length < textBytes.Length) {
                expandedKey += keyString;
            }
            byte[] expandedKeyBytes = Encoding.ASCII.GetBytes(expandedKey);
            var xorData = new byte[textBytes.Length];

            for (var i = 0; i < xorData.Length; i++)
                xorData[i] = (byte) (textBytes[i] ^ expandedKeyBytes[i]);  
            return Encoding.ASCII.GetString(xorData);
        }

        static void Main(string[] args)
        {  
            var path = @"Input.txt";
            string text = System.IO.File.ReadAllText(path);

            var keysize = findKeySize(text);

            //split text into blocks each XORd by the same byte in the key. 
            // There will be <KEYSIZE> such blocks, each of length num_bytes_in_text/ks
            var blocks = getBlocksBySameXORKey(keysize, text);

            // construct key from blocks
            byte[] key = new byte[keysize];
            int index = 0;
            foreach (byte[] block in blocks) {
                var scores = FindXORCipher(Convert.ToBase64String(block));
                key[index] = scores[0].b;
                index++;
            }

            var decryptedText = repeatingKeyXOR(key, text);

            Debug.Assert(decryptedText=="I'm back and I'm ringin' the bell \nA rockin' on the mike while the fly girls yell \nIn ecstasy in the back of me \nWell that's my DJ Deshay cuttin' all them Z's \nHittin' hard and the girlies goin' crazy \nVanilla's on the mike, man I'm not lazy. \n\nI'm lettin' my drug kick in \nIt controls my mouth and I begin \nTo just let it flow, let my concepts go \nMy posse's to the side yellin', Go Vanilla Go! \n\nSmooth 'cause that's the way I will be \nAnd if you don't give a damn, then \nWhy you starin' at me \nSo get off 'cause I control the stage \nThere's no dissin' allowed \nI'm in my own phase \nThe girlies sa y they love me and that is ok \nAnd I can dance better than any kid n' play \n\nStage 2 -- Yea the one ya' wanna listen to \nIt's off my head so let the beat play through \nSo I can funk it up and make it sound good \n1-2-3 Yo -- Knock on some wood \nFor good luck, I like my rhymes atrocious \nSupercalafragilisticexpialidocious \nI'm an effect and that you can bet \nI can take a fly girl and make her wet. \n\nI'm like Samson -- Samson to Delilah \nThere's no denyin', You can try to hang \nBut you'll keep tryin' to get my style \nOver and over, practice makes perfect \nBut not if you're a loafer. \n\nYou'll get nowhere, no place, no time, no girls \nSoon -- Oh my God, homebody, you probably eat \nSpaghetti with a spoon! Come on and say it! \n\nVIP. Vanilla Ice yep, yep, I'm comin' hard like a rhino \nIntoxicating so you stagger like a wino \nSo punks stop trying and girl stop cryin' \nVanilla Ice is sellin' and you people are buyin' \n'Cause why the freaks are jockin' like Crazy Glue \nMovin' and groovin' trying to sing along \nAll through the ghetto groovin' this here song \nNow you're amazed by the VIP posse. \n\nSteppin' so hard like a German Nazi \nStartled by the bases hittin' ground \nThere's no trippin' on mine, I'm just gettin' down \nSparkamatic, I'm hangin' tight like a fanatic \nYou trapped me once and I thought that \nYou might have it \nSo step down and lend me your ear \n'89 in my time! You, '90 is my year. \n\nYou're weakenin' fast, YO! and I can tell it \nYour body's gettin' hot, so, so I can smell it \nSo don't be mad and don't be sad \n'Cause the lyrics belong to ICE, You can call me Dad \nYou're pitchin' a fit, so step back and endure \nLet the witch doctor, Ice, do the dance to cure \nSo come up close and don't be square \nYou wanna battle me -- Anytime, anywhere \n\nYou thought that I was weak, Boy, you're dead wrong \nSo come on, everybody and sing this song \n\nSay -- Play that funky music Say, go white boy, go white boy go \nplay that funky music Go white boy, go white boy, go \nLay down and boogie and play that funky music till you die. \n\nPlay that funky music Come on, Come on, let me hear \nPlay that funky music white boy you say it, say it \nPlay that funky music A little louder now \nPlay that funky music, white boy Come on, Come on, Come on \nPlay that funky music \n");
        }

        static List<ScoredByte> FindXORCipher(string textBase64) {

            byte[] bytes = System.Convert.FromBase64String(textBase64);
            List<ScoredByte> scores = new List<ScoredByte>();
            // there are only so many possible chars. do the XOR against each, score the text, sort the results, pick the top
            // do the XOR
            for (int i = 0; i < 256; i++) {
                scores.Add(new ScoredByte((byte)i, bytes));
            }
            
            scores.Sort();
            return scores;
        }
        private static List<byte[]> getBlocksBySameXORKey(int keysize, string textBase64)
        {
            List<byte[]> l = new List<byte[]>();
            byte[] bytes = System.Convert.FromBase64String(textBase64);
            var blocklength = (int) Math.Ceiling( (double) bytes.Count() / keysize );

            for (int i = 0; i < keysize; i++) {
                l.Add(new byte[blocklength]);
            }

            // copy bytes from text to blocks
            for (int i = 0; i < bytes.Count(); i++) {
                int blocknum = i % keysize;
                int byteInBlock = i / keysize;
                l[blocknum][byteInBlock] = bytes[i];
            }

            return l;
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

                for (int i = 1; i<50; i++)
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
