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

		private const byte BITS_USED_PER_CHANNEL = 0x02;
		private const byte BITS_PER_CHANNEL = 0x08;

        #endregion

		private byte getByteSpread()
		{
			byte byteSpread = (BITS_PER_CHANNEL / BITS_USED_PER_CHANNEL) + (BITS_PER_CHANNEL % BITS_USED_PER_CHANNEL);

			return byteSpread;
		}

		private bool encryptionFeasible(FileModel fileModel, IImageWrapper container)
		{
			var spread = this.getByteSpread ();
			var numberOfChannels = Enum.GetValues (typeof(LosslessFileMuxxer.ByteOrder)).GetLength(0);
			var totalBytesInImage = container.Width * container.Height * numberOfChannels;

			var bytesAvailableForData = totalBytesInImage - EncryptionHeader.HEADER_LENGTH;
			var bytesNeededForData = fileModel.FileContents.Length * spread;

			return bytesAvailableForData >= bytesNeededForData;
		}

        private Pixel[,] muxxImageAndFile(FileModel fileModel, IImageWrapper container)
        {
			// warning, this is very verbose code.
			// i could have written this in linq much more elegantly,
			// however i wrote this so that others can pick it up without needing to know c# / linq / lambdas

			if (!this.encryptionFeasible (fileModel, container)) 
			{
				throw new Exception ("image is too small to hide this file in");
			}

			int byteSpread = this.getByteSpread ();
			int bitShiftCount = (BITS_PER_CHANNEL - BITS_USED_PER_CHANNEL);
			int maxValue = (0x01 << BITS_PER_CHANNEL) - 0x01;
			int partMask = maxValue >> bitShiftCount;

			var header = new EncryptionHeader ();
			header.FileName = fileModel.FileName;
			header.FileSize = (int)fileModel.FileContents.Length;

			var headerBytesUnspread = header.ToBytes ();
			var fileBytesUnspread = new byte[fileModel.FileContents.Length];

			fileModel.FileContents.Read (fileBytesUnspread, 0, (int)fileModel.FileContents.Length);

			var unspreadBytes = new byte[headerBytesUnspread.Length + fileBytesUnspread.Length];
			var spreadBytes = new byte[unspreadBytes.Length * byteSpread];

			int unspreadIndex = 0;
			for (var i = 0; i < headerBytesUnspread.Length; ++i) 
			{
				unspreadBytes [unspreadIndex] = headerBytesUnspread [i];
				++unspreadIndex;
			}
			
			for (var i = 0; i < fileBytesUnspread.Length; ++i) 
			{
				unspreadBytes [unspreadIndex] = fileBytesUnspread [i];
				++unspreadIndex;
			}

			int spreadIndex = 0;
			for (var i = 0; i < unspreadBytes.Length; ++i) 
			{
				var toSpread = unspreadBytes [i];

				for (var s = 0; s < byteSpread; ++s) // going least significant bit to most
				{
					var spreadShift = s * BITS_USED_PER_CHANNEL;
					var shifted = toSpread >> spreadShift;
					var masked = shifted & partMask;

					spreadBytes [spreadIndex] = (byte)masked;
					++spreadIndex;
				}
			}

			// sanity check
			var maxSpreadValue = (0x01 << (BITS_USED_PER_CHANNEL)) - 1;
			for (var i = 0; i < spreadBytes.Length; ++i) 
			{
				var sb = spreadBytes [i];
				if (sb > maxSpreadValue) 
				{
					throw new Exception("the author is a moron");
				}
			}

            Pixel[,] dataSet = new Pixel[container.Width, container.Height];

			var numberOfChannels = Enum.GetValues (typeof(LosslessFileMuxxer.ByteOrder)).GetLength (0);
			var channelIndex = 0;
			spreadIndex = 0;

			for (var x = 0; x < container.Width; ++x) 
			{
				for (var y = 0; y < container.Height; ++y) 
				{
					Pixel p = new Pixel ();
					p.R = container.GetRedAtPosition (x, y);
					p.G = container.GetGreenAtPosition (x, y);
					p.B = container.GetBlueAtPosition (x, y);

					if (spreadIndex < spreadBytes.Length) 
					{
						var byteToHide = spreadBytes [spreadIndex];
						var whichChannel = (ByteOrder)channelIndex;
						switch (whichChannel) 
						{
							case ByteOrder.Red:
							p.R = p.R & byteToHide;
								break;
							case ByteOrder.Green:
							p.G = p.G & byteToHide;
								break;
							case ByteOrder.Blue:
							p.B = p.B & byteToHide;
								break;
						}

						channelIndex = channelIndex == numberOfChannels ? 0 : channelIndex + 1
					}
					
					dataSet [x, y] = p;
					++spreadIndex;
				}
			}
            // populate data

            return dataSet;
        }

        private FileModel demuxxImage(Pixel[,] source)
        {
            FileModel fileModel = null;

            // extract data

            return fileModel;
        }

        #region non-specific interface jank

        public IImageWrapper Mux(FileModel fileModel, IImageWrapper container)
        {
            IImageWrapper result = new SlowImageWrapper();
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

        public FileModel Demux(IImageWrapper source)
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

		private class EncryptionHeader
		{
			public const byte HEADER_LENGTH = 36;
			private const byte MAX_FILENAME_LENGTH = 32;

			public EncryptionHeader() { }
			public EncryptionHeader(byte[] contents)
			{
				var eh = EncryptionHeader.FromBytes(contents);

				this.FileName = eh.FileName;
				this.FileSize = eh.FileSize;
			}

			public static EncryptionHeader FromBytes(byte[] contents)
			{
				var eh = new EncryptionHeader();

				if (contents == null)
				{
					throw new ArgumentNullException("contents"); // dumbass.
				}

				if (contents.Length < HEADER_LENGTH)
				{
					throw new Exception("Invalid header. Cannot decrypt");
				}

				var headerBytes = contents.Take(HEADER_LENGTH).ToArray();

				int index = 0;
				for (var i = 0; i < 4; ++index, ++i)
				{
					var part = headerBytes[i];
					eh.FileSize += (part << (i * 0x08));
				}

				eh.FileName = ASCIIEncoding.ASCII.GetString(headerBytes, 4, MAX_FILENAME_LENGTH).Replace("\0", string.Empty);

				return eh;
			}

			public byte[] ToBytes()
			{
				var header = new byte[HEADER_LENGTH];
				//var fileSizeBytes = BitConverter.GetBytes(this.FileSize);
				int index = 0;
				for (var i = 0; i < 4; ++index, ++i)
				{
					var part = (byte)(this.FileSize >> (i * 0x08) & 0x08);
					header[index] = part;
				}

				var fileNameBytes = ASCIIEncoding.ASCII.GetBytes(this.FileName);

				for (var i = 0; i < fileNameBytes.Length; ++index, ++i)
				{
					header[index] = fileNameBytes[i];
				}

				for (; index < HEADER_LENGTH; ++index)
				{
					header[index] = 0;
				}

				return header;
			}

			public int FileSize { get; set; }
			private string _fileName;
			public string FileName
			{
				get
				{
					return this._fileName;
				}
				set
				{
					if (value == null)
					{
						throw new Exception("You need a filename, dumbass");
					}

					if (value.Length > MAX_FILENAME_LENGTH)
					{
						throw new Exception(string.Format("Filename too big, needs to be {0} characters or shorter", MAX_FILENAME_LENGTH));
					}

					this._fileName = value;
				}
			}
		}

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
