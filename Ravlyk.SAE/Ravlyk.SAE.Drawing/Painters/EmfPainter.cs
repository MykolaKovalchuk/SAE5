using System;
using System.Drawing;
using System.Drawing.Imaging;
using Ravlyk.Drawing;
using Size = Ravlyk.Common.Size;

namespace Ravlyk.SAE.Drawing.Painters
{
	/// <summary>
	/// EMF image painter.
	/// </summary>
	public class EmfPainter : GdiPainter
	{
		public EmfPainter(string fileName, IntPtr dc, Size imageSize, Func<IndexedImage, Bitmap> toBitmap) : base(null, toBitmap, false)
		{
			metafile = new Metafile(fileName, dc, new Rectangle(0, 0, imageSize.Width, imageSize.Height), MetafileFrameUnit.Pixel, EmfType.EmfPlusDual);
			GdiGraphics = Graphics.FromImage(metafile);
		}

		readonly Metafile metafile;

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				GdiGraphics.Dispose();
				metafile.Dispose();
			}

			base.Dispose(disposing);
		}
	}
}
