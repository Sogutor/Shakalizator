using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shakalizator.Servises
{
    public class LoadImages
    {
        public LoadImages()
        {
        }
        public string GetDirectory()
        {
            string s;
            var fbd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) {
                return s = fbd.SelectedPath;
            }
            return null;
        }
        public List<string> Load(string folder)
        {
            var list = new List<string>();
            try {
                list.AddRange(Directory.GetFiles(folder, "*.jpg"));
                return list;
            }
            catch { return list; }
        }
        public List<string> LoadName(string folder)
        {
            var list = new List<string>();
            string[] files = Directory.GetFiles(folder, "*.jpg");
            foreach (string file in files) {
                list.Add(Path.GetFileName(file));
            }
            return list;
            
        }

        public List<string> LoadInfo(string folder)
        {
            List<string> list;
            list = new List<string>();
            string[] files = Directory.GetFiles(folder, "*.jpg");
            foreach (string file in files) {
                list.Add(" (" + Converter(GetFileSize(file)) + "," + GetResolution(file) + "), ");
            }
            return list;
           
        }
        private string GetResolution(string path)
        {
            var img = System.Drawing.Image.FromFile(path);
            return img.Width + "x" + img.Height;
        }

        private long GetFileSize(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            return fi.Length;
        }

        private string Converter(long value)
        {
            string s;
            string suffix;
            Decimal size = (int)value;
            if (size >= OneGigaByte) {
                size /= OneGigaByte;
                suffix = "гб ";
            } else if (size >= OneMegaByte) {
                size /= OneMegaByte;
                suffix = "мб ";
            } else if (size >= OneKiloByte) {
                size /= OneKiloByte;
                suffix = "кб ";
            } else {
                suffix = "б ";
            }

            return ((int)size).ToString() + suffix;

        }

        private const Decimal OneKiloByte = 1024M;
        private const Decimal OneMegaByte = OneKiloByte * 1024M;
        private const Decimal OneGigaByte = OneMegaByte * 1024M;
    }
}
