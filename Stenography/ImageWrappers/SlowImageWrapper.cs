using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Pwnasaur.Encryption.Stenography.ImageWrappers
{
    public class SlowImageWrapper : IImageWrapper
    {
        Bitmap _file;

        public void LoadFile(Stream stream)
        {
            this._file = Bitmap.FromStream(stream) as Bitmap;
        }

        public void LoadFile(string fileLocation)
        {
            if (!File.Exists(fileLocation))
            {
                throw new FileNotFoundException();
            }

            this._file = Bitmap.FromFile(fileLocation) as Bitmap;
        }

		public void Save(string fileLocation)
		{
			this._file.Save (fileLocation);
		}

        public void InitialiseEmpty(int width, int height)
        {
            this._file = new Bitmap(width, height);
        }

        public byte[] GetFullFile()
        {
            if (this._file == null)
            {
                throw new Exception("No file loaded!");
            }

            throw new NotImplementedException();
        }

        private Color getColourAt(int x, int y)
        {

            if (this._file == null)
            {
                throw new Exception("No file loaded!");
            }

            if (x >= this._file.Width || y >= this._file.Height)
            {
                throw new IndexOutOfRangeException();
            }

            var colour = this._file.GetPixel(x, y);

            return colour;
        }

        public byte GetRedAtPosition(int x, int y)
        {
            return getColourAt(x,y).R;
        }

        public byte GetGreenAtPosition(int x, int y)
        {
            return getColourAt(x, y).G;
        }

        public byte GetBlueAtPosition(int x, int y)
        {
            return getColourAt(x, y).B;
        }

        public byte GetAlphaAtPosition(int x, int y)
        {
            return getColourAt(x, y).A;
        }

        public int Width
        {
            get { return this._file.Width; }
        }

        public int Height
        {
            get { return this._file.Height; }
        }

        public void SetRedAtPosition(int x, int y, byte value)
        {
            var pixel = getColourAt(x, y);

            var newColour = Color.FromArgb(pixel.A, value, pixel.G, pixel.B);

            this._file.SetPixel(x, y, newColour);
        }

        public void SetGreenAtPosition(int x, int y, byte value)
        {
            var pixel = getColourAt(x, y);

            var newColour = Color.FromArgb(pixel.A, pixel.R, value, pixel.B);

            this._file.SetPixel(x, y, newColour);
        }

        public void SetBlueAtPosition(int x, int y, byte value)
        {
            var pixel = getColourAt(x, y);

            var newColour = Color.FromArgb(pixel.A, pixel.R, pixel.G, value);

            this._file.SetPixel(x, y, newColour);
        }

        public void SetAlphaAtPosition(int x, int y, byte value)
        {
            var pixel = getColourAt(x, y);

            var newColour = Color.FromArgb(value, pixel.R, pixel.G, pixel.B);

            this._file.SetPixel(x, y, newColour);
        }
    }
}
