using System;
using System.Diagnostics;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using Bets.Data;

namespace Bets.Infrastructure
{
    public class CustomTextWriterTraceListener : TextWriterTraceListener
    {
        private string fileName;
        private string name;
        private FileInfo filePath;
        private bool valid;

        public CustomTextWriterTraceListener(string fileName)
        {
            this.fileName = fileName;
            SetFilePath();
        }

        public CustomTextWriterTraceListener(string fileName, string name)
        {
            this.fileName = fileName;
            this.name = name;
            SetFilePath();
        }

        private void SetFilePath()
        {
            string path;

            //if it is a full path (c:\log.txt) use that
            if (fileName.Contains(":"))
            {
                path = fileName;
            }
            //otherwise map relative path
            else
            {
                path = HostingEnvironment.MapPath(@"~\" + fileName);
            }

            this.filePath = new FileInfo(path);

            try
            {
                if (!this.filePath.Directory.Exists)
                    this.filePath.Directory.Create();

                this.valid = true;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                this.valid = false;
            }
        }

        private void WriteToFile(string line, bool appendNewLine = false)
        {
            if (!this.valid)
                return;

            try
            {
                //prepend current date to log file
                var logFilePath = Path.Combine(filePath.DirectoryName, DateTime.Now.ToString("yyyy-MM-dd") + "_" + filePath.Name);

                StreamWriter sw = new StreamWriter(logFilePath, true);
                var content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + line;

                if (appendNewLine)
                {
                    sw.WriteLine(content);
                }
                else
                {
                    sw.Write(content);
                }

                sw.Close();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void WriteLineToFile(string line)
        {
            WriteToFile(line, true);
        }


        public override void Write(string message)
        {
            WriteToFile(message);
        }

        public override void Write(object o)
        {
            WriteToFile(o.ToJSON());
        }

        public override void Write(string message, string category)
        {
            WriteToFile(category + " " + message);
        }

        public override void Write(object o, string category)
        {
            WriteToFile(category + " " + o.ToJSON());
        }

        public override void WriteLine(object o)
        {
            WriteLineToFile(o.ToJSON());
        }

        public override void WriteLine(object o, string category)
        {
            WriteLineToFile(category + " " + o.ToJSON());
        }

        public override void WriteLine(string message)
        {
            WriteLineToFile(message);
        }

        public override void WriteLine(string message, string category)
        {
            WriteLineToFile(category + " " + message);
        }

        protected override void WriteIndent()
        {
            WriteToFile(@"\t");
        }
    }
}