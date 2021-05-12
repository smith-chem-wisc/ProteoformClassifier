using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineLayer
{
    public class StringEventArgs : EventArgs
    {
        public string S { get; }

        public StringEventArgs(string s)
        {
            S = DateTime.Now.ToString("h:mm:ss tt") + ": " + s;
        }
    }
}
