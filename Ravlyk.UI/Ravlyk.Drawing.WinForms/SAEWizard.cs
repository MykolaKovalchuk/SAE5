using System;
using System.Drawing;

using Ravlyk.SAE.Drawing;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// WinForms extension to <see cref="Ravlyk.SAE.Drawing.Processor.SAEWizard"/>.
	/// </summary>
	public class SAEWizard : Ravlyk.SAE.Drawing.Processor.SAEWizard
	{
		/// <summary>
		/// Loads image from a file.
		/// </summary>
		/// <param name="fileName">Name of file with image.</param>
		/// <returns><see cref="CodedImage"/> instance with pixels data initialized from loaded file.</returns>
		protected override CodedImage LoadImageCore(string fileName)
		{
			var bitmap = new Bitmap(fileName);
			var indexedImage = IndexedImageExtensions.FromBitmap(bitmap);
			var codedImage = CodedImage.FromIndexedImage(indexedImage);

			return codedImage;
		}
	}
}
