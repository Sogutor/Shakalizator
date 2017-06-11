using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shakalizator.Servises
{
    public class FolderLoad
    {
        public FolderLoad() {
        }
        public List<string> LoadFolder(string _directory)
        {
            FileInfo f = new FileInfo(_directory);
            var newFolderList = new List<string>();
            string t = "";
            string[] str2 = Directory.GetDirectories(_directory);
            foreach (string s2 in str2) {
                f = new FileInfo(@s2);
                t = s2.Substring(s2.LastIndexOf('\\') + 1);
                newFolderList.Add(t);
            }
            return newFolderList;
        }
    }
    
}
