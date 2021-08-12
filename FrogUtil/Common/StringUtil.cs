namespace Frog.Util.Common
{
    class StringUtil
    {
        public static bool IsBlank(string str)
        {
            return str == null || "".Equals(str.Trim());
        }

        public static bool IsNotBlank(string str)
        {
            return str != null && !"".Equals(str.Trim());
        }

        public static bool IsEmpty(string str)
        {
            return str == null || "".Equals(str);
        }

        public static bool IsNotEmpty(string str)
        {
            return str != null && !"".Equals(str);
        }
    }
}
