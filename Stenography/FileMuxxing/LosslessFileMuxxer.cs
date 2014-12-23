using Pwnasaur.Encryption.Stenography.ImageProviders;
using Pwnasaur.Encryption.Stenography.ImageWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pwnasaur.Encryption.Stenography.FileMuxxing
{
    public class LosslessFileMuxxer : IFileMuxxer
    {
        #region local constants

        private const byte BYTES_TO_HIDE = 2;

        #endregion

        Pixel[,] muxxImageAndFile(FileModel fileModel, IImageWrapper container)
        {
            Pixel[,] dataSet = new Pixel[container.Width, container.Height];

            // populate data

            return dataSet;
        }

        FileModel demuxxImage(Pixel[,] source)
        {
            FileModel fileModel = null;

            // extract data

            return fileModel;
        }

        #region non-specific interface jank

        IImageWrapper Mux(FileModel fileModel, IImageWrapper container)
        {
            ImageProviders.IImageWrapper result = new SlowImageWrapper();
            result.InitialiseEmpty(container.Width, container.Height);

            Pixel[,] dataSet = muxxImageAndFile(fileModel, container);

            for (var x = 0; x < container.Width; ++x)
            {
                for (var y = 0; y < container.Height; ++y)
                {
                    var pixel = dataSet[x,y];
                    result.SetRedAtPosition(x, y, pixel.R);
                    result.SetGreenAtPosition(x, y, pixel.G);
                    result.SetBlueAtPosition(x, y, pixel.B);
                }
            }

            return result;
        }

        FileModel Demux(IImageWrapper source)
        {
            Pixel[,] dataSet = new Pixel[source.Width, source.Height];

            for (var x = 0; x < source.Width; ++x)
            {
                for (var y = 0; y < source.Height; ++y)
                {
                    Pixel p = new Pixel();
                    p.R = source.GetRedAtPosition(x, y);
                    p.G = source.GetGreenAtPosition(x, y);
                    p.B = source.GetBlueAtPosition(x, y);

                    dataSet[x, y] = p;
                }
            }

            return demuxxImage(dataSet);
        }

        #endregion

        #region local data definitions

        private class Pixel
        {
            public byte R { get; set; }
            public byte G { get; set; }
            public byte B { get; set; }
        }

        private enum ByteOrder
        {
            Red = 0,
            Green = 1,
            Blue = 2,
        }

        #endregion
    }
}
