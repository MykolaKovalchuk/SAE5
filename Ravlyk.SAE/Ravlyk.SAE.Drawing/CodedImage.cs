using System;
using System.Collections.Generic;
using System.IO;
using Ravlyk.Drawing;
using Ravlyk.SAE.Drawing.Processor;

namespace Ravlyk.SAE.Drawing
{
	public class CodedImage : IndexedImage, IImageProvider
	{
		public new CodedPalette Palette
		{
			get { return (CodedPalette)base.Palette; }
			set
			{
				var palette = Palette;
				if (palette != null)
				{
					palette.ColorAttributesChanged -= Palette_ColorAttributesChanged;
					palette.ColorVisibilityChanged -= Palette_ColorVisibilityChanged;
				}

				base.Palette = value;

				palette = Palette;
				if (palette != null)
				{
					palette.ColorAttributesChanged += Palette_ColorAttributesChanged;
					palette.ColorVisibilityChanged += Palette_ColorVisibilityChanged;
				}
			}
		}

		void Palette_ColorAttributesChanged(object sender, CodedPalette.ColorAttributesChangedEventArgs e)
		{
			OnImageChanged();
		}

		void Palette_ColorVisibilityChanged(object sender, Palette.ColorChangedEventArgs e)
		{
			OnImageChanged();
		}

		protected override Palette NewPalette()
		{
			var palette  = new CodedPalette();
			palette.ColorAttributesChanged += Palette_ColorAttributesChanged;
			palette.ColorVisibilityChanged += Palette_ColorVisibilityChanged;
			return palette;
		}

		public new CodedColor this[int x, int y]
		{
			get { return (CodedColor)base[x, y]; }
			set
			{
				if (!value.Equals(base[x, y]))
				{
					base[x, y] = value;
				}
			}
		}

		#region Clone

		public new CodedImage Clone(bool withPalette, bool doComplete = false)
		{
			return (CodedImage)base.Clone(withPalette, doComplete);
		}

		protected override IndexedImage CloneCore()
		{
			var newImage = (CodedImage)base.CloneCore();
			newImage.ImageChanged = null;
			newImage.SourceImageFileName = SourceImageFileName;
			newImage.FileName = "";

			newImage.ResetAdditionalData();
			if (additionalData != null)
			{
				foreach (var dataPair in additionalData)
				{
					newImage.AdditionalData[dataPair.Key] = dataPair.Value;
				}
			}

			newImage.HasChanges = false;
			return newImage;
		}

		#endregion

		public string SourceImageFileName { get; set; }
		public string FileName { get; set; }

		public string Description
		{
			get
			{
				var name = FileName ?? SourceImageFileName;
				return !string.IsNullOrEmpty(name) ? Path.GetFileNameWithoutExtension(name) : null;
			}
		}

		public static CodedImage FromIndexedImage(IndexedImage sourceImage)
		{
			var codedImage = sourceImage as CodedImage;
			if (codedImage == null)
			{
				codedImage = new CodedImage();
				int[] pixels;
				using (sourceImage.LockPixels(out pixels))
				{
					codedImage.Pixels = pixels;
				}
				codedImage.Size = sourceImage.Size;
			}
			return codedImage;
		}

		public bool HasChanges { get; set; }

		#region Additionla data

		public Dictionary<string, string> AdditionalData => additionalData ?? (additionalData = new Dictionary<string, string>());
		Dictionary<string, string> additionalData;

		public void ResetAdditionalData()
		{
			additionalData = null;
		}

		public bool TryGetAdditionalDataAsString(string dataName, out string dataValue)
		{
			dataValue = null;
			return additionalData?.TryGetValue(dataName, out dataValue) ?? false;
		}

		public bool TryGetAdditionalValueAsInt(string dataName, out int dataValue)
		{
			string dataText = null;
			if ((additionalData?.TryGetValue(dataName, out dataText) ?? false) && !string.IsNullOrEmpty(dataText))
			{
				return int.TryParse(dataText, out dataValue);
			}
			dataValue = 0;
			return false;
		}

		public bool TryGetAdditionalValueAsBool(string dataName, out bool dataValue)
		{
			string dataText = null;
			if ((additionalData?.TryGetValue(dataName, out dataText) ?? false) && !string.IsNullOrEmpty(dataText))
			{
				return bool.TryParse(dataText, out dataValue);
			}
			dataValue = false;
			return false;
		}

		#endregion

		#region IImageProvider members

		CodedImage IImageProvider.Image => this;

		bool IImageProvider.SupportsChangedEvent => true;

		void OnImageChanged()
		{
			ImageChanged?.Invoke(this, EventArgs.Empty);
		}

		public event EventHandler ImageChanged;

		public void TriggerImageChanged()
		{
			OnImageChanged();
		}

		#endregion
	}
}
