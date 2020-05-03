using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MapleUtility.Plugins.Helpers
{
    public class AES256EncryptHelper
    {
        private static SHA256Managed sha256Managed = new SHA256Managed();
        private static RijndaelManaged aes = new RijndaelManaged()
        {
            KeySize = 256,
            BlockSize = 128,
            Mode = CipherMode.CBC,
            Padding = PaddingMode.PKCS7
        };


        //AES_256 암호화
        public static byte[] AESEncrypt256(string encryptData, String password)
        {
            var byteArray = Encoding.UTF8.GetBytes(encryptData);

            // Salt는 비밀번호의 길이를 SHA256 해쉬값으로 한다.
            var salt = sha256Managed.ComputeHash(Encoding.UTF8.GetBytes(password.Length.ToString()));

            //PBKDF2(Password-Based Key Derivation Function)
            //반복은 65535번
            var PBKDF2Key = new Rfc2898DeriveBytes(password, salt, 65535);
            var secretKey = PBKDF2Key.GetBytes(aes.KeySize / 8);
            var iv = PBKDF2Key.GetBytes(aes.BlockSize / 8);

            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(secretKey, iv), CryptoStreamMode.Write))
                {
                    cs.Write(byteArray, 0, byteArray.Length);
                }
                xBuff = ms.ToArray();
            }

            return xBuff;
        }

        //AES_256 복호화
        public static string AESDecrypt256(byte[] decryptData, String password)
        {
            // Salt는 비밀번호의 길이를 SHA256 해쉬값으로 한다.
            var salt = sha256Managed.ComputeHash(Encoding.UTF8.GetBytes(password.Length.ToString()));

            //PBKDF2(Password-Based Key Derivation Function)
            //반복은 65535번
            var PBKDF2Key = new Rfc2898DeriveBytes(password, salt, 65535);
            var secretKey = PBKDF2Key.GetBytes(aes.KeySize / 8);
            var iv = PBKDF2Key.GetBytes(aes.BlockSize / 8);

            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(secretKey, iv), CryptoStreamMode.Write))
                {
                    cs.Write(decryptData, 0, decryptData.Length);
                }
                xBuff = ms.ToArray();
            }

            return Encoding.UTF8.GetString(xBuff);
        }
    }
}
