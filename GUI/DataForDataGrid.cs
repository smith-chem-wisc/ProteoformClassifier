using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    public class DataForDataGrid
    {
        public string FileName { get; private set; }
        public string FilePath { get; private set; }

        public DataForDataGrid(string path)
        {
            FileName = Path.GetFileName(path);
            FilePath = path;
        }
    }
}