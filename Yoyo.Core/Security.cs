using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace Yoyo.Core
{
    /// <summary>
    /// 安全类
    /// </summary>
    public static class Security
    {
        #region MD5加密方法
        /// <summary>
        /// MD5加密[大写]
        /// </summary>
        /// <param name="value">需要加密的字符串</param>
        /// <param name="IsShort">是否使用16位加密[默认:false]</param>
        /// <returns></returns>
        public static string MD5(string value, bool IsShort = false)
        {
            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] bs = Encoding.UTF8.GetBytes(value);
                bs = md5.ComputeHash(bs);
                string CryptoStr;
                if (IsShort)
                {
                    CryptoStr = BitConverter.ToString(bs, 4, 8).Replace("-", "");
                }
                else
                {
                    StringBuilder s = new StringBuilder();
                    foreach (byte b in bs) { s.Append(b.ToString("x2")); }
                    CryptoStr = s.ToString();
                }
                return CryptoStr.ToUpper();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Base64加密
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="value">需要加密的字符串</param>
        /// <returns></returns>
        public static string Base64Encrypt(string value)
        {
            return Base64Encrypt(Encoding.UTF8, value);
        }
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="encode">加密编码格式</param>
        /// <param name="value">需要加密的字符串</param>
        /// <returns></returns>
        public static string Base64Encrypt(Encoding encode, string value)
        {
            return Convert.ToBase64String(encode.GetBytes(value));
        }
        #endregion

        #region Base64解密
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="value">解密字符串</param>
        /// <returns></returns>
        public static string Base64Decrypt(string value)
        {
            return Base64Decrypt(Encoding.UTF8, value);
        }
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="encode">编码格式</param>
        /// <param name="value">解密字符串</param>
        /// <returns></returns>
        public static string Base64Decrypt(Encoding encode, string value)
        {
            byte[] bytes = Convert.FromBase64String(value);
            try
            {
                return encode.GetString(bytes);
            }
            catch
            {
                return value;
            }
        }
        #endregion

        #region Base64验证
        /// <summary>
        /// 验证是否是Base64格式字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBase64String(string str)
        {
            if (str != null)
            {
                return new Regex(@"[A-Za-z0-9\=\/\+]").IsMatch(str);
            }
            return true;
        }
        #endregion

        #region AES加密
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">需要加密的字符串</param>
        /// <param name="Key">密钥</param>
        /// <param name="Model">密码模式 默认:ECB</param>
        /// <param name="Padding">填充类型 默认:PKCS7</param>
        /// <param name="IV">加密辅助向量 默认:null</param>
        /// <returns>加密后的字符串</returns>
        public static string AESEncrypt(string value, string Key, CipherMode Model = CipherMode.ECB, PaddingMode Padding = PaddingMode.PKCS7, string IV = "")
        {
            try
            {
                byte[] keyArray = Encoding.UTF8.GetBytes(Key);
                byte[] toEncryptArray = Encoding.UTF8.GetBytes(value);
                RijndaelManaged rDel = new RijndaelManaged();
                if (!string.IsNullOrWhiteSpace(IV)) { rDel.IV = Encoding.UTF8.GetBytes(IV); }
                rDel.Key = keyArray;
                rDel.Mode = Model;
                rDel.Padding = Padding;
                ICryptoTransform cTransform = rDel.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                rDel.Dispose();
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region AES解密
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">需要解密的字符串</param>
        /// <param name="Key">密钥</param>
        /// <param name="Model">密码模式 默认:ECB</param>
        /// <param name="Padding">填充类型 默认:PKCS7</param>
        /// <param name="IV">加密辅助向量 默认:null</param>
        /// <returns>解密后的字符串 异常返回null</returns>
        public static string AESDecrypt(string value, string Key, CipherMode Model = CipherMode.ECB, PaddingMode Padding = PaddingMode.PKCS7, string IV = "")
        {
            try
            {
                byte[] keyArray = Encoding.UTF8.GetBytes(Key);
                byte[] toEncryptArray = Convert.FromBase64String(value);
                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                if (!string.IsNullOrWhiteSpace(IV)) { rDel.IV = Encoding.UTF8.GetBytes(IV); }
                rDel.Mode = Model;
                rDel.Padding = Padding;
                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SHA加密

        #region SHA-UTF-8加密
        /// <summary>
        /// SHA-UTF-8加密
        /// </summary>
        /// <param name="value">需要加密的字符串</param>
        /// <param name="shaType">加密方式</param>
        /// <returns></returns>
        public static string SHA(string value, SHAType shaType = SHAType.SHA1) => SHA(value, Encoding.UTF8, shaType);
        #endregion

        #region SHA指定编码加密
        /// <summary>
        /// SHA指定编码加密
        /// </summary>
        /// <param name="value">需要加密的字符串</param>
        /// <param name="encoding">编码格式</param>
        /// <param name="shaType">加密方式</param>
        /// <returns>返回加密后的字符串</returns>
        public static string SHA(string value, Encoding encoding, SHAType shaType = SHAType.SHA1)
        {
            try
            {
                HashAlgorithm ShaEn = null;
                switch (shaType)
                {
                    case SHAType.SHA256: ShaEn = new SHA256CryptoServiceProvider(); break;
                    case SHAType.SHA512: ShaEn = new SHA512CryptoServiceProvider(); break;
                    default: ShaEn = new SHA1CryptoServiceProvider(); break;
                }
                byte[] data = encoding.GetBytes(value);
                byte[] result = ShaEn.ComputeHash(data);
                ShaEn.Dispose();
                var _Result = BitConverter.ToString(result).Replace("-", "");
                return _Result;
            }
            catch (Exception ex) { throw ex; }
        }
        #endregion

        #region SHA加密枚举
        /// <summary>
        /// SHA加密类型
        /// </summary>
        public enum SHAType
        {
            /// <summary>
            /// SHA1
            /// </summary>
            SHA1,
            /// <summary>
            /// SHA256
            /// </summary>
            SHA256,
            /// <summary>
            /// SHA512
            /// </summary>
            SHA512
        }
        #endregion

        #endregion

        #region HMACSHA256
        /// <summary>
        /// HMACSHA256加密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string HMACSHA256(string content, string key)
        {
            string result = "";
            var enc = Encoding.Default;
            byte[]
            baText2BeHashed = enc.GetBytes(content),
            baSalt = enc.GetBytes(key);
            System.Security.Cryptography.HMACSHA256 hasher = new HMACSHA256(baSalt);
            byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
            result = string.Join("", baHashedText.ToList().Select(b => b.ToString("x2")).ToArray());
            return result.ToUpper();
        }
        #endregion

        #region AES_256_GCM
        /// <summary>
        /// AES_256_GCM解密
        /// </summary>
        /// <param name="context">内容</param>
        /// <param name="key">秘钥</param>
        /// <param name="nonce">随机串</param>
        /// <param name="associated">随机串</param>
        /// <returns></returns>
        public static String GcmDecrypt(String context, String key, String nonce, String associated)
        {
            try
            {
                Byte[] keybytes = Encoding.UTF8.GetBytes(key);
                Byte[] noncebytes = Encoding.UTF8.GetBytes(nonce);
                Byte[] associatedbytes = Encoding.UTF8.GetBytes(associated);
                Byte[] contextbytes = Convert.FromBase64String(context);
                GcmBlockCipher gcmBlockCipher = new GcmBlockCipher(new AesEngine());
                AeadParameters aeadParameters = new AeadParameters(new KeyParameter(keybytes), 128, noncebytes, associatedbytes);
                gcmBlockCipher.Init(false, aeadParameters);
                byte[] array2 = new byte[gcmBlockCipher.GetOutputSize(contextbytes.Length)];
                int num = gcmBlockCipher.ProcessBytes(contextbytes, 0, contextbytes.Length, array2, 0);
                gcmBlockCipher.DoFinal(array2, num);
                return Encoding.UTF8.GetString(array2);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// AES_256_GCM加密
        /// </summary>
        /// <param name="context">内容</param>
        /// <param name="key">秘钥</param>
        /// <param name="nonce">随机串</param>
        /// <param name="associated">随机串</param>
        /// <returns></returns>
        public static String GcmEncrypt(String context, String key, String nonce, String associated)
        {
            try
            {
                byte[] keybytes = Encoding.UTF8.GetBytes(key);
                byte[] noncebytes = Encoding.UTF8.GetBytes(nonce);
                byte[] contextbytes = Encoding.UTF8.GetBytes(context);
                byte[] associatedbytes = Encoding.UTF8.GetBytes(associated);
                GcmBlockCipher gcmBlockCipher = new GcmBlockCipher(new AesEngine());
                AeadParameters aeadParameters = new AeadParameters(new KeyParameter(keybytes), 128, noncebytes, associatedbytes);
                gcmBlockCipher.Init(true, aeadParameters);
                byte[] array2 = new byte[gcmBlockCipher.GetOutputSize(contextbytes.Length)];
                int num = gcmBlockCipher.ProcessBytes(contextbytes, 0, contextbytes.Length, array2, 0);
                gcmBlockCipher.DoFinal(array2, num);
                return Convert.ToBase64String(array2, 0, array2.Length);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region DES 加密
        /// <summary>
        /// Des加密（Hex）
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥(8位任意字符)</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="cipher">运算模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static string DesEncryptToHexString(string encryptString, string encryptKey, Encoding encoding, CipherMode cipher = CipherMode.ECB, PaddingMode padding = PaddingMode.PKCS7)
        {
            var keyBytes = encoding.GetBytes(encryptKey);
            var inputBytes = encoding.GetBytes(encryptString);
            var outputBytes = EncryptToDesBytes(inputBytes, keyBytes, cipher, padding);
            var sBuilder = new StringBuilder();
            foreach (var b in outputBytes)
            {
                sBuilder.Append(b.ToString("X2"));//
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// Des加密（base64）
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥(8位任意字符)</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="cipher">运算模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static string DesEncryptToBase64String(string encryptString, string encryptKey, Encoding encoding, CipherMode cipher = CipherMode.ECB, PaddingMode padding = PaddingMode.Zeros)
        {
            var keyBytes = encoding.GetBytes(encryptKey);
            var inputBytes = encoding.GetBytes(encryptString);
            var outputBytes = EncryptToDesBytes(inputBytes, keyBytes, cipher, padding);
            return Convert.ToBase64String(outputBytes);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="encryptBytes">待加密的字节数组</param>
        /// <param name="keyBytes">加密密钥字节数组</param>
        /// <param name="cipher">运算模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static byte[] EncryptToDesBytes(byte[] encryptBytes, byte[] keyBytes, CipherMode cipher = CipherMode.ECB, PaddingMode padding = PaddingMode.PKCS7)
        {
            var des = new DESCryptoServiceProvider
            {
                Key = keyBytes,
                IV = keyBytes,
                Mode = cipher,
                Padding = padding
            };
            var outputBytes = des.CreateEncryptor().TransformFinalBlock(encryptBytes, 0, encryptBytes.Length);
            return outputBytes;
        }
        #endregion

        #region DES 解密
        /// <summary>
        /// 解密（Hex）
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="cipher">运算模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static string DesDecryptByHexString(string decryptString, string decryptKey, Encoding encoding, CipherMode cipher = CipherMode.ECB, PaddingMode padding = PaddingMode.PKCS7)
        {
            var keyBytes = encoding.GetBytes(decryptKey);
            var inputBytes = new byte[decryptString.Length / 2];
            for (var i = 0; i < inputBytes.Length; i++)
            {
                inputBytes[i] = Convert.ToByte(decryptString.Substring(i * 2, 2), 16);
            }
            var outputBytes = DecryptByDesBytes(inputBytes, keyBytes, cipher, padding);
            return encoding.GetString(outputBytes).TrimEnd('\0');
        }
        /// <summary>
        /// 解密（Base）
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="cipher">运算模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static string DesDecryptByBase64String(string decryptString, string decryptKey, Encoding encoding, CipherMode cipher = CipherMode.ECB, PaddingMode padding = PaddingMode.PKCS7)
        {
            var keyBytes = encoding.GetBytes(decryptKey);
            var inputBytes = Convert.FromBase64String(decryptString);
            var outputBytes = DecryptByDesBytes(inputBytes, keyBytes, cipher, padding);
            return encoding.GetString(outputBytes).TrimEnd('\0');
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="decryptBytes">待解密的字节数组</param>
        /// <param name="keyBytes">解密密钥字节数组</param>
        /// <param name="cipher">运算模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static byte[] DecryptByDesBytes(byte[] decryptBytes, byte[] keyBytes, CipherMode cipher = CipherMode.ECB, PaddingMode padding = PaddingMode.PKCS7)
        {
            var des = new DESCryptoServiceProvider
            {
                Key = keyBytes,
                IV = keyBytes,
                Mode = cipher,
                Padding = padding
            };
            var outputBytes = des.CreateDecryptor().TransformFinalBlock(decryptBytes, 0, decryptBytes.Length);
            return outputBytes;
        }
        #endregion

        #region RSA加密

        #region 创建RSA对象
        /// <summary>
        /// 创建RSA对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="IsPrivate"></param>
        /// <returns></returns>
        private static RSA CreateRSAProvider(string key, bool IsPrivate)
        {
            return IsPrivate ? CreateRsaProviderFromPrivateKey(key) : CreateRsaProviderFromPublicKey(key);
        }
        #endregion

        #region RSA加密
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="rsa">RSA对象</param>
        /// <param name="data">原始数据</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        private static String Encrypt(this RSA rsa, string data, RSAEncryptionPadding padding)
        {
            var waitBytes = Encoding.UTF8.GetBytes(data);
            var enBytes = rsa.Encrypt(waitBytes, padding);
            return Convert.ToBase64String(enBytes);
        }
        #endregion

        #region RSA解密
        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="rsa">RSA对象</param>
        /// <param name="data">原始数据</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        private static String Decrypt(this RSA rsa, string data, RSAEncryptionPadding padding)
        {
            var waitBytes = Convert.FromBase64String(data);
            var deBytes = rsa.Decrypt(waitBytes, padding);
            return Encoding.UTF8.GetString(deBytes);
        }
        #endregion

        #region RSA加签
        /// <summary>
        /// RSA加签
        /// </summary>
        /// <param name="rsa">RSA对象</param>
        /// <param name="data">原始数据</param>
        /// <param name="hash">HASH算法</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static String SignData(this RSA rsa, string data, HashAlgorithmName hash, RSASignaturePadding padding)
        {
            var waitBytes = Encoding.UTF8.GetBytes(data);
            var signBytes = rsa.SignData(waitBytes, hash, padding);
            return Convert.ToBase64String(signBytes);
        }
        #endregion

        #region RSA验签
        /// <summary>
        /// RSA验签
        /// </summary>
        /// <param name="rsa">RSA对象</param>
        /// <param name="data">原始数据</param>
        /// <param name="sign">签名</param>
        /// <param name="hash">HASH算法</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static Boolean VerifyData(this RSA rsa, string data, string sign, HashAlgorithmName hash, RSASignaturePadding padding)
        {
            var verifyBytes = Encoding.UTF8.GetBytes(data);
            var signBytes = Convert.FromBase64String(sign);
            return rsa.VerifyData(verifyBytes, signBytes, hash, padding);
        }
        #endregion

        #region 根据公钥创建RSA
        public static RSA CreateRsaProviderFromPublicKey(string publicKeyString)
        {
            byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];
            var x509Key = Convert.FromBase64String(publicKeyString);
            using (MemoryStream mem = new MemoryStream(x509Key))
            {
                using (BinaryReader binr = new BinaryReader(mem))
                {
                    byte bt = 0;
                    ushort twobytes = 0;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                    { binr.ReadByte(); }
                    else if (twobytes == 0x8230)
                    { binr.ReadInt16(); }
                    else
                    { return null; }

                    seq = binr.ReadBytes(15);
                    if (!CompareBytearrays(seq, seqOid))
                    { return null; }

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103)
                    { binr.ReadByte(); }
                    else if (twobytes == 0x8203)
                    { binr.ReadInt16(); }
                    else
                    { return null; }

                    bt = binr.ReadByte();
                    if (bt != 0x00)
                    { return null; }

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                    { binr.ReadByte(); }
                    else if (twobytes == 0x8230)
                    { binr.ReadInt16(); }
                    else
                    { return null; }

                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102)
                    { lowbyte = binr.ReadByte(); }
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte();
                        lowbyte = binr.ReadByte();
                    }
                    else
                    { return null; }
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    int modsize = BitConverter.ToInt32(modint, 0);

                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {
                        binr.ReadByte();
                        modsize -= 1;
                    }

                    byte[] modulus = binr.ReadBytes(modsize);
                    if (binr.ReadByte() != 0x02)
                    { return null; }
                    int expbytes = (int)binr.ReadByte();
                    byte[] exponent = binr.ReadBytes(expbytes);

                    var rsa = RSA.Create();
                    RSAParameters rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    rsa.ImportParameters(rsaKeyInfo);

                    return rsa;
                }

            }
        }
        #endregion

        #region 根据私钥创建RSA
        public static RSA CreateRsaProviderFromPrivateKey(string privateKey)
        {
            var privateKeyBits = Convert.FromBase64String(privateKey);

            var rsa = RSA.Create();
            var rsaParameters = new RSAParameters();

            using (BinaryReader binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                { binr.ReadByte(); }
                else if (twobytes == 0x8230)
                { binr.ReadInt16(); }
                else
                { throw new Exception("Unexpected value read binr.ReadUInt16()"); }

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102) { throw new Exception("Unexpected version"); }

                bt = binr.ReadByte();
                if (bt != 0x00) { throw new Exception("Unexpected value read binr.ReadByte()"); }

                rsaParameters.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.D = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.P = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.Q = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.DP = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.DQ = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }

            rsa.ImportParameters(rsaParameters);
            return rsa;
        }
        #endregion

        #region 私有导入密钥算法
        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
            { return 0; }
            bt = binr.ReadByte();

            if (bt == 0x81)
            { count = binr.ReadByte(); }
            else if (bt == 0x82)
            {
                var highbyte = binr.ReadByte();
                var lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            { return false; }
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                { return false; }
                i++;
            }
            return true;
        }

        #endregion

        #endregion
    }
}
