using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Yoyo.IPlugins.Utils
{
    /// <summary>
    /// 微信Xml
    /// </summary>
    public class WeXmlDoc : XmlDocument
    {
        public WeXmlDoc()
        {
            XmlElement rootElement = this.CreateElement("xml");
            this.AppendChild(rootElement);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            string strVal = null;
            if (value == null)
            {
                strVal = null;
            }
            XmlNode root = this.SelectSingleNode("xml");
            XmlElement node = this.CreateElement(key);
            if (value is string)
            {
                if (!string.IsNullOrWhiteSpace((string)value))
                {
                    XmlCDataSection cdata = this.CreateCDataSection((string)value);
                    node.AppendChild(cdata);
                    root.AppendChild(node);
                }
                return;
            }
            if (value is Nullable<Int32>)
            {
                strVal = (value as Nullable<Int32>).Value.ToString();
            }
            if (value is Nullable<Int64>)
            {
                strVal = (value as Nullable<Int64>).Value.ToString();
            }
            if (!String.IsNullOrWhiteSpace(strVal))
            {
                node.InnerText = strVal;
                root.AppendChild(node);
            }
        }
        /// <summary>
        /// 转成字符串
        /// </summary>
        /// <returns></returns>
        public string ToXmlStr()
        {
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, null);
            writer.Formatting = Formatting.Indented;
            this.Save(writer);
            StreamReader sr = new StreamReader(stream, Encoding.UTF8);
            stream.Position = 0;
            string xmlString = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return xmlString;
        }

        /// <summary>
        /// 获取签名
        /// </summary>
        /// <returns></returns>
        public string GetSign(string key)
        {
            XmlNode rootXml = this.SelectSingleNode("xml");
            XmlNodeList xmlList = rootXml.ChildNodes;
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            foreach (XmlNode item in xmlList)
            {
                dic.Add(item.Name, item.InnerText);
            }
            StringBuilder signStr = new StringBuilder();
            dic.Aggregate(signStr, (s, i) => s.Append($"{i.Key}={i.Value.ToString()}&"));
            signStr.Append("key=");
            signStr.Append(key);
            return MD5(signStr.ToString());
        }

        /// <summary>
        /// MD5加密[大写]
        /// </summary>
        /// <param name="value">需要加密的字符串</param>
        /// <param name="IsShort">是否使用16位加密[默认:false]</param>
        /// <returns></returns>
        private static string MD5(string value, bool IsShort = false)
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




    }
}
