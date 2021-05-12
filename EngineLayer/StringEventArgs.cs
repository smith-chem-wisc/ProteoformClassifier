using System;

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
