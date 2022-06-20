using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Utils {
	public static class EncryptionUtils {
        public static string ToHexString(this byte[] b)
        {
            var ret = new StringBuilder();
            foreach (byte theByte in b) ret.Append(theByte.ToString("x2"));
            return ret.ToString();
        }

        public static byte[] FromHexString(string hex) => Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

        public static string MD5(this string s) => MD5Bytes(s).ToHexString();

        public static string MD5(this byte[] s) => MD5Bytes(s).ToHexString();

        public static byte[] MD5Bytes(this byte[] s)
        {
            using (MD5 a = System.Security.Cryptography.MD5.Create()) return a.ComputeHash(s);
        }

        public static byte[] MD5Bytes(this string s) => Encoding.UTF8.GetBytes(s).MD5Bytes();

        public static string SHA256(this string s) => Encoding.UTF8.GetBytes(s).SHA256();

        public static string SHA256(this byte[] s) => s.SHA256Bytes().ToHexString();

        public static byte[] SHA256Bytes(this string s) => Encoding.UTF8.GetBytes(s).SHA256Bytes();

        public static byte[] SHA256Bytes(this byte[] s) => new SHA256Managed().ComputeHash(s);

        public static byte[] AESEncrypt(this string plainText, byte[] Key)
        {
            byte[] encrypted;
            byte[] IV;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.GenerateIV();
                IV = aesAlg.IV;
                aesAlg.Mode = CipherMode.CBC;
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                            swEncrypt.Write(plainText);
                        encrypted = msEncrypt.ToArray();
                    }
            }
            var combinedIvCt = new byte[IV.Length + encrypted.Length];
            Array.Copy(IV, 0, combinedIvCt, 0, IV.Length);
            Array.Copy(encrypted, 0, combinedIvCt, IV.Length, encrypted.Length);
            return combinedIvCt;
        }
        public static byte[] AESEncrypt(this byte[] plainText, byte[] Key) => plainText.AESEncrypt(Key);
        public static byte[] AESEncrypt(this byte[] plainText ,string Key) => Encoding.UTF8.GetString(plainText).AESEncrypt(Key);
        public static byte[] AESEncrypt(this string plainText, string Key)
        {
            var k = new byte[16];
            var t = Encoding.UTF8.GetBytes(Key);
            Buffer.BlockCopy(t, 0, k, 0, Math.Min(t.Length, 16));
            return plainText.AESEncrypt(k);
        }

        public static bool AESTryDecrypt(this byte[] cipherTextCombined, byte[] Key,out string decrypted)
        {
            try {
                decrypted = AESDecrypt(cipherTextCombined,Key);
            } catch(Exception) {
                decrypted = "";
                return false;
            }
            return true;
        }

        public static bool AESTryDecrypt(this string cipherTextCombined, byte[] Key,out string decrypted) => Encoding.UTF8.GetBytes(cipherTextCombined).AESTryDecrypt(Key,out decrypted);
        public static bool AESTryDecrypt(this byte[] cipherTextCombined, string Key, out string decrypted) => cipherTextCombined.AESTryDecrypt(Encoding.UTF8.GetBytes(Key),out decrypted);
        public static bool AESTryDecrypt(this string cipherTextCombined, string Key, out string decrypted) => cipherTextCombined.AESTryDecrypt(Encoding.UTF8.GetBytes(Key), out decrypted);

        public static string AESDecrypt(this byte[] cipherTextCombined, byte[] Key)
        {
            string plaintext = null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                byte[] IV = new byte[aesAlg.BlockSize / 8];
                byte[] cipherText = new byte[cipherTextCombined.Length - IV.Length];
                Array.Copy(cipherTextCombined, IV, IV.Length);
                Array.Copy(cipherTextCombined, IV.Length, cipherText, 0, cipherText.Length);
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(cipherText))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                    plaintext = srDecrypt.ReadToEnd();
            }
            return plaintext;
        }
        public static string AESDecrypt(this string cipherTextCombined, byte[] Key) => Encoding.UTF8.GetBytes(cipherTextCombined).AESDecrypt(Key);
        public static string AESDecrypt(this byte[] cipherTextCombined, string Key)
        {
            var k = new byte[16];
            var t = Encoding.UTF8.GetBytes(Key);
            Buffer.BlockCopy(t, 0, k, 0, Math.Min(t.Length, 16));
            return cipherTextCombined.AESDecrypt(k);
        }

        public static string AESDecrypt(this string cipherTextCombined, string Key) => Encoding.UTF8.GetBytes(cipherTextCombined).AESDecrypt(Key);

        public static string[] RSAEncrypt(this string data,RSA key,int blockSize)
        {
            string[] ret = new string[0];
            while (data != "")
            {
                int l = data.Length < blockSize ? data.Length : blockSize;
                ret = ret.Append(Convert.ToBase64String(key.Encrypt(Encoding.UTF8.GetBytes(data.Substring(0, l)), RSAEncryptionPadding.Pkcs1))).ToArray();
                data = data.Substring(l);
            }
            return ret;
        }
    }
}