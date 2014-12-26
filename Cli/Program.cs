using System;
using Pwnasaur.Encryption.Stenography.ImageWrappers;
using Pwnasaur.Encryption.Stenography.FileMuxxing;
using System.IO;

namespace Cli
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			IImageWrapper imageWrapper = new SlowImageWrapper ();

			imageWrapper.LoadFile ("data/destination.png");
			var filename = "id_rsa";
			var ms = new MemoryStream ();
			using (var fs = new FileStream("data/" + filename, FileMode.Open)) {
				fs.CopyTo (ms);
				ms.Position = 0;
			}

			var fileModel = new FileModel (ms, filename);

			IFileMuxxer muxxer = new LosslessFileMuxxer ();
			using (var result = muxxer.Mux (fileModel, imageWrapper)) 
			{
				result.Save ("data/result.png");
			}

			using (var toDecrypt = new SlowImageWrapper ()) 
			{
				toDecrypt.LoadFile ("data/result.png");

				var reversed = muxxer.Demux (toDecrypt);

				using (var fs = new FileStream("data/result_" + reversed.FileName,FileMode.Create)) {
					reversed.FileContents.CopyTo (fs);
				}
			}
		}
	}
}
