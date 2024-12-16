using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Pika.Base.Utils
{
    public class CipherUtil
    {
        private CipherUtil()
        {
        }

        private static string md5(byte[] v)
        {
            char[] hexDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
            try
            {
                var md5 = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5"));
                byte[] mdCode = md5.ComputeHash(v);

                int mdCodeLength = mdCode.Length;
                char[] strMd5 = new char[mdCodeLength * 2];
                int k = 0;
                for (int i = 0; i < mdCodeLength; i++)
                {
                    byte byte0 = mdCode[i];
                    strMd5[(k++)] = hexDigits[((int)((uint)byte0 >> 4) & 0xF)];
                    strMd5[(k++)] = hexDigits[(byte0 & 0xF)];
                }
                return new string(strMd5);
            }
            catch (Exception e)
            {
            }
            return "";
        }

        /**
         * <p>MD5Encode.</p>
         *
         * @param s a {@link String} object.
         * @return a {@link String} object.
         */
        public static String MD5Encode(string s)
        {
            return md5(UTF8Encoding.Default.GetBytes(s));
        }

        /**
         * <p>MD5Bytes.</p>
         *
         * @param v an array of {@link byte} objects.
         * @return an array of {@link byte} objects.
         */
        public static byte[] MD5Bytes(byte[] v)
        {

            return UTF8Encoding.Default.GetBytes(md5(v));
        }
    }
}
