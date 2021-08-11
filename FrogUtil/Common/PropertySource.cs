using System.Collections;
using System.IO;

namespace Frog.Util.Common
{
    public class PropertySource
    {

        private readonly Hashtable hashtable = new Hashtable();

        public PropertySource(string filePath)
        {
            LoadProperties(filePath);
        }

        private void LoadProperties(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return;
            }
            char[] convertBuf = new char[1024];
            int limit;
            int keyLen;
            int valueStart;
            char c;
            string bufLine = string.Empty;
            bool hasSep;
            bool precedingBackslash;
            using (StreamReader sr = new StreamReader(filePath))
            {
                while (sr.Peek() >= 0)
                {
                    bufLine = sr.ReadLine();
                    limit = bufLine.Length;
                    keyLen = 0;
                    valueStart = limit;
                    hasSep = false;
                    precedingBackslash = false;
                    if (bufLine.StartsWith("#"))
                        keyLen = bufLine.Length;
                    while (keyLen < limit)
                    {
                        c = bufLine[keyLen];
                        if ((c == '=' || c == ':') & !precedingBackslash)
                        {
                            valueStart = keyLen + 1;
                            hasSep = true;
                            break;
                        }
                        else if ((c == ' ' || c == '\t' || c == '\f') & !precedingBackslash)
                        {
                            valueStart = keyLen + 1;
                            break;
                        }
                        if (c == '\\')
                        {
                            precedingBackslash = !precedingBackslash;
                        }
                        else
                        {
                            precedingBackslash = false;
                        }
                        keyLen++;
                    }
                    while (valueStart < limit)
                    {
                        c = bufLine[valueStart];
                        if (c != ' ' && c != '\t' && c != '\f')
                        {
                            if (!hasSep && (c == '=' || c == ':'))
                            {
                                hasSep = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                        valueStart++;
                    }
                    string key = bufLine.Substring(0, keyLen);
                    string values = bufLine.Substring(valueStart, limit - valueStart);
                    if (key == "")
                        key += "#";
                    while (key.StartsWith("#") & hashtable.Contains(key))
                    {
                        key += "#";
                    }
                    hashtable.Add(key, values);
                }
            }
        }

        public string GetProperty(string name)
        {
            return this.GetProperty(name, null);
        }

        public string GetProperty(string name, string defaultValue)
        {
            object obj = hashtable[name];
            return obj == null ? defaultValue : obj.ToString();
        }

    }
}
