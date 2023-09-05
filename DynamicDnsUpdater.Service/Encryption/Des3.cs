using System;
using System.Security.Cryptography;
using System.Text;

namespace DynamicDnsUpdater.Service.Encryption
{
	/// <summary>
	/// Simple Des3 encryption/decryption, it is symmetric so it is not extremely secure. 
	/// </summary>
	public static class Des3 
    {
        /// <summary>
        /// Encrypt text
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText, string key)
        {
            TripleDESCryptoServiceProvider DES = new  TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5 = new  MD5CryptoServiceProvider();

            DES.Key = hashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
            DES.Mode = CipherMode.ECB;
            byte[] Buffer =  ASCIIEncoding.ASCII.GetBytes(plainText.Trim());
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(
                Convert.ToBase64String(DES.CreateEncryptor().TransformFinalBlock(Buffer, 0, Buffer.Length)).Trim()));

        }

        /// <summary>
        /// Decrypt text
        /// </summary>
        /// <param name="base64Text"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string base64Text, string key)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();

            DES.Key = hashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
            DES.Mode = CipherMode.ECB;

            byte[] Buffer = Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Text.Trim())));
            return  ASCIIEncoding.ASCII.GetString(DES.CreateDecryptor().TransformFinalBlock(Buffer, 0, Buffer.Length)).Trim();


        }

    }
}
