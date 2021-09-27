using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWork_Delegates
{
    class FileWatcher
    {
        public string directory { get; set; }

        public event EventHandler<FileArgs> FileFound;

        public FileWatcher(string dir)
        {
            directory = dir;
        }

        public void Search(string searchPattern)
        {
            foreach (var file in Directory.GetFiles(directory, searchPattern))
            {
                var args = new FileArgs(file);
                FileFound?.Invoke(this, args);
                if (args.CancelRequested)
                    break;
            }
        }
    }
    public class FileArgs : EventArgs
    {
        public string NameFile { get; }
        public bool CancelRequested { get; set; }

        public FileArgs(string fileName)
        {
            NameFile = fileName;
        }
    }
}
