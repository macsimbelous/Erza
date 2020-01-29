using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Nalsjn
{
    class MiniLogger
    {
        private string Delimiter;
        private FileStream file;
        private string file_name;
        public MiniLogger(string LogFile, bool ReplaceOldLogFile, string Delimiter)
        {
            if(Delimiter == null) { throw new NullReferenceException("Параметр Delimiter не может быть равен null!"); }
            this.Delimiter = Delimiter;
            if (File.Exists(LogFile) && ReplaceOldLogFile)
            {
                File.Delete(LogFile);
            }
            this.file = new FileStream(LogFile, FileMode.OpenOrCreate);
            this.file.Seek(this.file.Length, SeekOrigin.Begin);
            file_name = LogFile;
        }
        public void Write(string Event)
        {
            string temp = String.Format("{0}{1}{2}\r\n", DateTime.Now, this.Delimiter, Event);
            byte[] array = System.Text.Encoding.UTF8.GetBytes(temp);
            this.file.Write(array, 0, array.Length);
            this.file.Flush();
        }
        public void Close()
        {
            //this.file.Flush();
            this.file.Close();
        }
    }
}
