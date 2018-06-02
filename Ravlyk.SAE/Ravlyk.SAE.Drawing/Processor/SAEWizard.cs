using System;
using System.Collections.Generic;
using System.Linq;

using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.Processor
{
	public abstract class SAEWizard
	{
		#region Image Controllers

		public ImageSetterController ImageSetter => imageSetter ?? (imageSetter = new ImageSetterController(new ImageSetterManipulator()));
		ImageSetterController imageSetter;

		public ImageRotateController ImageRotator => imageRotator ?? (imageRotator = new ImageRotateController(new ImageRotateManipulator(ImageSetter.Manipulator)));
		ImageRotateController imageRotator;

		public ImageCropController ImageCropper => imageCropper ?? (imageCropper = new ImageCropController(new ImageCropManipulator(ImageRotator.Manipulator)));
		ImageCropController imageCropper;

		public ImageSizeController ImageResizer => imageResizer ?? (imageResizer = new ImageSizeController(new ImageSizeManipulator(ImageCropper.Manipulator)));
		ImageSizeController imageResizer;

		public ImageColorsController ImageColorer
		{
			get
			{
				if (imageColorer == null)
				{
					imageColorer = new ImageColorsController(new ImageColorsManipulator(ImageResizer.Manipulator), preLoadedColorPalettes, initialPalette);
					preLoadedColorPalettes = null;
					initialPalette = null;
				}
				return imageColorer;
			}
		}
		ImageColorsController imageColorer;

		public ImageSymbolsController ImageSymboler
		{
			get
			{
				if (imageSymboler == null)
				{
					imageSymboler = new ImageSymbolsController(new ImageSymbolsManipulator(ImageColorer.Manipulator), preLoadedFonts, initialFont);
					preLoadedFonts = null;
					initialFont = null;
				}
				return imageSymboler;
			}
		}
		ImageSymbolsController imageSymboler;

		#endregion

		#region Images

		public CodedImage SourceImage => ImageSetter.Manipulator.SourceImage;

		public CodedImage RotatedCroppedImage => ImageCropper.Manipulator.ManipulatedImage;

		public CodedImage ColoredImage => ImageColorer.Manipulator.ManipulatedImage;

		public CodedImage FinalImage => ImageSymboler.Manipulator.ManipulatedImage;

		#endregion

		#region Load Image

		public string SourceImageFileName => SourceImage.SourceImageFileName;

		public void LoadSourceImageFromFile(string fileName)
		{
			ImageSetter.SetNewImage(LoadImage(fileName));
		}

		CodedImage LoadImage(string fileName)
		{
			var image = LoadImageCore(fileName);
			image.SourceImageFileName = fileName;
			return image;
		}

		protected abstract CodedImage LoadImageCore(string fileName);

		#endregion

		#region Load Palettes

		public void SetPalettes(IEnumerable<CodedPalette> allPalettes, string initialPaletteName)
		{
			if (imageColorer != null)
			{
				using (imageColorer.SuspendCallManipulations())
				{
					imageColorer.AddColorPalettes(allPalettes);
					imageColorer.PaletteName = initialPaletteName;
				}
			}
			else
			{
				if (preLoadedColorPalettes == null)
				{
					preLoadedColorPalettes = new List<CodedPalette>();
				}
				preLoadedColorPalettes.AddRange(allPalettes);
				initialPalette = preLoadedColorPalettes.FirstOrDefault(palette => palette.Name.Equals(initialPaletteName, StringComparison.OrdinalIgnoreCase));
			}
		}

		List<CodedPalette> preLoadedColorPalettes;
		CodedPalette initialPalette;

		#endregion

		#region Load Symbol fonts

		public void SetSymbolFonts(IEnumerable<TrueTypeFont> allFonts, string initialFontName = "")
		{
			if (imageSymboler != null)
			{
				using (imageSymboler.SuspendCallManipulations())
				{
					imageSymboler.AddSymbolsFonts(allFonts);
					imageSymboler.SymbolsFontName = initialFontName;
				}
			}
			else
			{
				if (preLoadedFonts == null)
				{
					preLoadedFonts = new List<TrueTypeFont>();
				}
				preLoadedFonts.AddRange(allFonts);
				initialFont = preLoadedFonts.FirstOrDefault(font => font.Name.Equals(initialFontName, StringComparison.OrdinalIgnoreCase));
			}
		}

		List<TrueTypeFont> preLoadedFonts;
		TrueTypeFont initialFont;

		#endregion

		#region Settings

		public void SaveDefaults()
		{
			imageSetter?.SaveDefaults();
			imageRotator?.SaveDefaults();
			imageCropper?.SaveDefaults();
			imageResizer?.SaveDefaults();
			imageColorer?.SaveDefaults();
			imageSymboler?.SaveDefaults();
			SAEWizardSettings.Default.Save();
		}

		public void SaveImageSettings(CodedImage image)
		{
			imageSetter?.SaveImageSettings(image);
			imageRotator?.SaveImageSettings(image);
			imageCropper?.SaveImageSettings(image);
			imageResizer?.SaveImageSettings(image);
			imageColorer?.SaveImageSettings(image);
			imageSymboler?.SaveImageSettings(image);
		}

		public void RestoreImageSettings(CodedImage image)
		{
			using (ImageSymboler.SuspendCallManipulations())
			{
				using (ImageColorer.SuspendCallManipulations())
				{
					using (ImageResizer.SuspendCallManipulations())
					{
						using (ImageColorer.SuspendCallManipulations())
						{
							using (ImageRotator.SuspendCallManipulations())
							{
								ImageRotator.RestoreImageSettings(image);
							}
							ImageCropper.RestoreImageSettings(image);
						}
						ImageResizer.RestoreImageSettings(image);
					}
					ImageColorer.RestoreImageSettings(image);
				}
				ImageSymboler.RestoreImageSettings(image);
			}
		}

		#endregion
	}
}
