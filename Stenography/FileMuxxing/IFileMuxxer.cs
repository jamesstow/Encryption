using Pwnasaur.Encryption.Stenography.ImageWrappers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Pwnasaur.Encryption.Stenography.FileMuxxing
{
    public interface IFileMuxxer
    {
        IImageWrapper Mux(FileModel fileModel, IImageWrapper container);
        FileModel Demux(IImageWrapper source);
    }
}
