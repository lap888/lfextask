using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Yoyo.Core
{
    public class SecurityUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "Error";
            }
            var md5 = System.Security.Cryptography.MD5.Create();
            string a = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(str)));
            a = a.Replace("-", "");
            return a;
        }

        public static bool CheckAddressSign(string sign, string key, string timeSpan)
        {
            string str = $"{key}_{timeSpan}";
            return SecurityUtil.MD5(str).Equals(sign, StringComparison.OrdinalIgnoreCase);
        }

        public static string getMD5(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static string getHex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }
        /// <summary>
        /// 生成签名
        /// 需要签名的数组,正序排序后(不区分大小写),md5加密,截取从第5位开始后的24位
        /// </summary>
        /// <param name="strs"></param>
        /// <returns>签名后的字符串</returns>
        public static string Sign(params string[] strs)
        {
            if (strs == null || strs.Length == 0)
            {
                throw new Exception("加密对象不能为空!");
            }
            var list = strs.Select(t => t.ToUpper()).OrderBy(t => t).Where(t => !string.IsNullOrEmpty(t)).ToArray();

            if (list.Length == 0)
            {
                throw new Exception("加密对象不能为空!");
            }
            var value = MD5(string.Join("", list)).ToCharArray(5, 24);
            return new string(value);
        }
        /// <summary>
        /// 验证签名正确性
        /// </summary>
        /// <param name="sign">需要验证的签名</param>
        /// <param name="strs">需要验证的数组</param>
        /// <returns></returns>
        public static bool ValidSign(string sign, params string[] strs)
        {
            var _sign = Sign(strs);
            return _sign.Equals(sign, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 微信小程序 encryptedData 解密
        /// </summary>
        /// <param name="encryptedDataStr"></param>
        /// <param name="key">session_key</param>
        /// <param name="iv">iv</param>
        /// <returns></returns>
        public static string AES_128_CBC_Decrypt(string encryptedDataStr, string key, string iv)
        {
            var aes = Aes.Create();
            //设置 cipher 格式 AES-128-CBC 
            aes.KeySize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(iv);
            byte[] encryptedData = Convert.FromBase64String(encryptedDataStr);
            //解密 
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            string result;
            using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        result = srDecrypt.ReadToEnd();
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 微信小程序验签
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="signature"></param>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static bool ValidWxUserSign(string rawData, string signature, string sessionKey)
        {
            var sha1 = SHA1.Create();
            var source = Encoding.UTF8.GetBytes(rawData + sessionKey);
            var target = sha1.ComputeHash(source);
            var result = BitConverter.ToString(target).Replace("-", "").ToLower();
            return result.Equals(signature);

        }

        #region Base64位加密解密
        /// <summary>
        /// 将字符串转换成base64格式,使用UTF8字符集
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <returns></returns>
        public static string Base64Encode(string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// 将base64格式，转换utf8
        /// </summary>
        /// <param name="content">解密内容</param>
        /// <returns></returns>
        public static string Base64Decode(string content)
        {
            byte[] bytes = Convert.FromBase64String(content);
            return Encoding.UTF8.GetString(bytes);
        }
        #endregion

        #region SDW 签名
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="channel"></param>
        /// <param name="nick"></param>
        /// <param name="uid"></param>
        /// <param name="avatar"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string SdwGenSign(string key, string channel, string nick, long uid, string avatar, string time)
        {
            var StringA = $"channel={channel}&openid={uid}&time={time}&nick={nick}&avatar={avatar}&sex=0&phone=0";
            var StringB = StringA + key;
            var sign = MD5(StringB).ToLower();
            return sign;
        }
        #endregion

        #region AES加密解密 
        /// <summary>
        /// 128位处理key 
        /// </summary>
        /// <param name="keyArray">原字节</param>
        /// <param name="key">处理key</param>
        /// <returns></returns>
        private static byte[] GetAesKey(byte[] keyArray, string key)
        {
            byte[] newArray = new byte[16];
            if (keyArray.Length < 16)
            {
                for (int i = 0; i < newArray.Length; i++)
                {
                    if (i >= keyArray.Length)
                    {
                        newArray[i] = 0;
                    }
                    else
                    {
                        newArray[i] = keyArray[i];
                    }
                }
            }
            return newArray;
        }
        /// <summary>
        /// 使用AES加密字符串,按128位处理key
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <param name="key">秘钥，需要128位、256位.....</param>
        /// <returns>Base64字符串结果</returns>
        public static string AesEncrypt(string content, string key, bool autoHandle = true)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            if (autoHandle)
            {
                keyArray = GetAesKey(keyArray, key);
            }
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(content);

            SymmetricAlgorithm des = Aes.Create();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = des.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray);
        }
        /// <summary>
        /// 使用AES解密字符串,按128位处理key
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="key">秘钥，需要128位、256位.....</param>
        /// <returns>UTF8解密结果</returns>
        public static string AesDecrypt(string content, string key, bool autoHandle = true)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            if (autoHandle)
            {
                keyArray = GetAesKey(keyArray, key);
            }
            byte[] toEncryptArray = Convert.FromBase64String(content);

            SymmetricAlgorithm des = Aes.Create();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = des.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
        #endregion
    }
}