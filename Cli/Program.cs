using System;
using Pwnasaur.Encryption.Stenography.ImageWrappers;
using Pwnasaur.Encryption.Stenography.FileMuxxing;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Cli
{
	class MainClass
	{
		private static bool checkFile(string filepath, string name)
		{
			if (string.IsNullOrWhiteSpace (filepath)) 
			{
				Console.WriteLine ("No value given for " + name);
				return false;
			}

			if (!File.Exists (filepath)) 
			{
				Console.WriteLine ("File \"" + filepath + "\" does not exist");
				return false;
			}

			return true;
		}

		public static void Main (string[] args)
		{
			string inputImage = "original_image.png";
			/*TO do 
			{
				Console.Write ("Enter path to original image: ");
				inputImage = Console.ReadLine ();
			} 
			while(checkFile(inputImage, "original image"));
			*/
			string inputFile = "input.txt";
			/*TO do 
			{
				Console.Write ("Enter path to file to hide: ");
				inputFile = Console.ReadLine ();
			} 
			while(checkFile(inputFile, "file to hide"));
			*/
			var inputImageParts = inputImage.Split(new char[]{'\\','/'}).ToList();
			var outputImagePath =inputImageParts.Any() ?  string.Join("\\", inputImageParts.Take (inputImageParts.Count - 1)) : null;
			var outputImage = (string.IsNullOrWhiteSpace(outputImagePath) ? string.Empty : (outputImagePath + "\\")) + "result.png";

			IImageWrapper imageWrapper = new SlowImageWrapper ();

			imageWrapper.LoadFile (inputImage);

			var ms = new MemoryStream ();
			using (var fs = new FileStream(inputFile, FileMode.Open)) {
				fs.CopyTo (ms);
				ms.Position = 0;
			}

			var fileNameParts = inputFile.Split (new char[] { '/', '\\' }).ToList();
			var fileModel = new FileModel (ms, fileNameParts.Last());

			IFileMuxxer muxxer = new LosslessFileMuxxer ();
			using (var result = muxxer.Mux (fileModel, imageWrapper)) 
			{
				result.Save (outputImage);
			}

			using (var toDecrypt = new SlowImageWrapper ()) 
			{
				toDecrypt.LoadFile (outputImage);

				var reversed = muxxer.Demux (toDecrypt);

				using (var fs = new FileStream("result_" + reversed.FileName,FileMode.Create)) {
					reversed.FileContents.CopyTo (fs);
				}
			}
		}
	}
}
