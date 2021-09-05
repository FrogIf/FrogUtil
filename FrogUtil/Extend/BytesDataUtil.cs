using Frog.Util.Common;
using System;
using System.Text;

namespace Frog.Util.Extend
{
    /// <summary>
    /// 字节消息工具类
    /// </summary>
    public class BytesDataUtil
    {
        /// <summary>
        /// 将字节数组转换为hex
        /// </summary>
        /// <param name="bytes">原始字节数组</param>
        /// <param name="pretty">是否优化样式, true - 是, false - 否</param>
        /// <returns>hex</returns>
        public static string ConvertToHex(byte[] bytes, bool pretty = true)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                if (b <= 0x0F) { sb.Append('0'); }
                sb.Append(Convert.ToString(b, 16).ToUpper());
                if (pretty)
                {
                    sb.Append(' ');
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将hex转换为字节数组
        /// </summary>
        /// <param name="hex">原始hex</param>
        /// <returns>转换后的字节数组</returns>
        public static byte[] ConertToBytesViaHex(string hex)
        {
            hex = hex.Replace(" ", "");
            char[] chars = hex.ToCharArray();
            byte[] bytes = new byte[chars.Length];
            
            for(int i = 0, j = 0; i < bytes.Length; i += 2, j++)
            {
                bytes[j] = Convert.ToByte(chars[i] + "" + chars[i + 1], 16);
            }

            return bytes;
        }

        /// <summary>
        /// 校验hex数据是否正确
        /// 校验规则:
        /// 1. 所有非空字符个数是否为偶数
        /// 2. 除了空字符外, 是否有0-9|A-F|a-f  以外的字符
        /// </summary>
        /// <param name="message">待校验的原始数据</param>
        /// <returns>结果: true - 正确; false - 错误</returns>
        public static bool HexDataValid(string message, ref string tip)
        {
            if (message == null && StringUtil.IsBlank(message)) { return false; }
            char[] chars = message.ToCharArray();
            int count = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                char ch = chars[i];
                if (ch == ' ') { continue; }
                if (ch < '0' || (ch > '9' && (ch < 'A' || ch > 'F') && (ch < 'a' || ch > 'f')))
                {
                    tip = "HEX格式不正确, pos : " + i + ", char : " + ch;
                    return false;
                }
                count++;
            }
            if(count % 2 != 0)
            {
                tip = "HEX格式不正确, len : " + count;
                return false;
            }
            return true;
        }
    }
}
