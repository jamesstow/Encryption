using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Pwnasaur.Encryption.Stenography.ImageWrappers
{
    public interface IImageWrapper
    {
        int Width { get; }
        int Height { get; }
        void LoadFile(Stream stream);
        void LoadFile(string fileLocation);
		void Save (string fileLocation);
        void InitialiseEmpty(int width, int height);
        byte[] GetFullFile();
        byte GetRedAtPosition(int x, int y);
        byte GetGreenAtPosition(int x, int y);
        byte GetBlueAtPosition(int x, int y);
        byte GetAlphaAtPosition(int x, int y);
        void SetRedAtPosition(int x, int y, byte value);
        void SetGreenAtPosition(int x, int y, byte value);
        void SetBlueAtPosition(int x, int y, byte value);
        void SetAlphaAtPosition(int x, int y, byte value);
    }
}
