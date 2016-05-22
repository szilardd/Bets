using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Bets.Data
{
    public class TextToTraceWriter : TextWriter
    {
        public string Category { get; set; }

        public override Encoding Encoding => Encoding.Default;

        public override void WriteLine(string value)
        {
            if (Category == null)
            {
                Trace.WriteLine(value);
            }
            else
            {
                Trace.WriteLine(value, Category);
            }
        }

        public override void Write(string value)
        {
            if (Category == null)
            {
                Trace.Write(value);
            }
            else
            {
                Trace.Write(value, Category);
            }
        }
    }
}
