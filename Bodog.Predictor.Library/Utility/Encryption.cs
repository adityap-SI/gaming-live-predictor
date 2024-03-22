using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Bodog.Predictor.Library.Utility
{
    public class Encryption
    {
        /// <summary>
        /// New Algorith for faster processing
        /// </summary>
        private static readonly string VIKey = "alnATsuSsjps4OgAmIReIg==";//Cannot contain symbols or special characters

        private static readonly string Key = "akAshiSeIjuR0icHig01g0==";//Cannot contain symbols or special characters

        private static readonly string AesSalt = "hei";

        public static String BaseEncrypt(String vPlainText)
        {
            byte[] rawPlaintext = Encoding.Unicode.GetBytes(vPlainText);
            byte[] cipherText = null;

            using (Aes aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Convert.FromBase64String(Key);
                aes.IV = Convert.FromBase64String(VIKey);//System.Text.Encoding.Unicode.GetBytes(VIKey);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(rawPlaintext, 0, rawPlaintext.Length);
                    }
                    cipherText = ms.ToArray();
                }
            }
            String mCryptoText = Convert.ToBase64String(cipherText);
            mCryptoText = ConvertStringToHex(mCryptoText, Encoding.Unicode);
            return mCryptoText;
        }

        public static String BaseDecrypt(String vCryptoText)
        {
            byte[] cipherText = Convert.FromBase64String(ConvertHexToString(vCryptoText, Encoding.Unicode));
            byte[] plainText = null;

            using (Aes aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Convert.FromBase64String(Key);
                aes.IV = Convert.FromBase64String(VIKey);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherText, 0, cipherText.Length);
                    }
                    plainText = ms.ToArray();
                }
            }

            String mPlainText = Encoding.Unicode.GetString(plainText);
            return mPlainText;
        }

        public static String AesEncrypt(String vPlainText)
        {
            vPlainText += AesSalt;

            byte[] rawPlaintext = Encoding.Unicode.GetBytes(vPlainText);
            byte[] cipherText = null;

            using (Aes aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Convert.FromBase64String(Key);
                aes.IV = Convert.FromBase64String(VIKey);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(rawPlaintext, 0, rawPlaintext.Length);
                    }
                    cipherText = ms.ToArray();
                }
            }

            String mCryptoText = Convert.ToBase64String(cipherText);
            return mCryptoText;
        }

        public static String AesDecrypt(String vCryptoText)
        {
            byte[] cipherText = Convert.FromBase64String(vCryptoText);
            byte[] plainText = null;

            using (Aes aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Convert.FromBase64String(Key);
                aes.IV = Convert.FromBase64String(VIKey);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherText, 0, cipherText.Length);
                    }
                    plainText = ms.ToArray();
                }
            }

            String mPlainText = Encoding.Unicode.GetString(plainText);

            mPlainText = mPlainText.Remove(mPlainText.LastIndexOf(AesSalt), AesSalt.Length);

            return mPlainText;
        }

        public static string ConvertStringToHex(String input, Encoding encoding)
        {
            Byte[] stringBytes = encoding.GetBytes(input);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        public static string ConvertHexToString(String hexInput, Encoding encoding)
        {
            int numberChars = hexInput.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return encoding.GetString(bytes);
        }
    }

    public class BareEncryption
    {
        //Below key should have maximum 16 characters i.e. 16 bytes
        private static readonly string IVKey = "veGetA4+t$unAE!=";

        private static readonly string Key = "k!rIt@goKUzeR0/=";

        //Encoding should be UTF8 only for 16 bytes encoding
        private static readonly Encoding encoding = Encoding.UTF8;

        public static string BaseEncrypt(string plain)
        {
            byte[] input = encoding.GetBytes(plain);

            byte[] key = encoding.GetBytes(Key);

            byte[] iv = encoding.GetBytes(IVKey);

            byte[] bytes = BouncyCastleCrypto(true, input, key, iv);

            string result = ByteArrayToString(bytes);

            return result;
        }

        public static string BaseDecrypt(string cipher)
        {
            byte[] key = encoding.GetBytes(Key);

            byte[] iv = encoding.GetBytes(IVKey);

            byte[] bytes = BouncyCastleCrypto(false, StringToByteArray(cipher), key, iv);

            string result = encoding.GetString(bytes);

            return result;
        }

        private static string ByteArrayToString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        private static byte[] StringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];

            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }

        private static byte[] BouncyCastleCrypto(bool forEncrypt, byte[] input, byte[] key, byte[] iv)
        {
            try
            {
                SicBlockCipher mode = new SicBlockCipher(new AesEngine());

                mode.Init(forEncrypt, new ParametersWithIV(new KeyParameter(key), iv));

                BufferedBlockCipher cipher = new BufferedBlockCipher(mode);

                return cipher.DoFinal(input);
            }
            catch (CryptoException)
            {
                throw;
            }
        }
    }
}