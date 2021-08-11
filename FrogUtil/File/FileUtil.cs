using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frog.Util.File
{
    public class FileUtil
    {
        /// <summary>
        /// 文件重命名
        /// </summary>
        /// <param name="filePath">文件所在目录</param>
        /// <param name="oldName">旧文件名</param>
        /// <param name="newName">新文件名</param>
        public static void renameFile(string filePath, string oldName, string newName)
        {
            string oldFullName = filePath + "/" + oldName;
            string newFullName = filePath + "/" + newName;
            FileInfo fi = new FileInfo(oldFullName);
            fi.MoveTo(newFullName);
        }

        public static void createDirectory(string path)
        {
            
        }

    }
}
