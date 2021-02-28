namespace aes_example
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    class Program
    {
        public static void Main()
        {
            var path = @"Input.txt";
            byte[] temp = System.IO.File.ReadAllBytes(path);
            byte[] cipherTextBytes = new byte[temp.Length + 16 - (temp.Length % 16) ];
            Array.Copy(temp, cipherTextBytes, temp.Length);
            Debug.Assert(cipherTextBytes.Length % 16 == 0); // make sure data fits in block size

            using (Aes aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 128;          // in bits
                aes.Key = Encoding.ASCII.GetBytes("YELLOW SUBMARINE"); // 16 bytes for 128 bit encryption
                aes.IV = new byte[128/8];   // AES needs a 16-byte IV
                // Should set Key and IV here.  Good approach: derive them from 
                // a password via Cryptography.Rfc2898DeriveBytes 
                byte[] plainText = null;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherTextBytes, 0, cipherTextBytes.Length);
                    }
                    
                    plainText = ms.ToArray();
                }
                string s = System.Text.Encoding.Unicode.GetString(plainText);
                Console.WriteLine(s);
            }
        }
    }
}

