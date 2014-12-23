using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Pwnasaur.Encryption.Stenography.FileMuxxing
{
    public class FileModel
    {
        public FileModel() { }
        public FileModel(Stream fileContents, string fileName)
        {
            this.FileContents = fileContents;
            this.FileName = fileName;
        }

        public Stream FileContents { get; set; }
        public string FileName { get; set; }
    }
}
