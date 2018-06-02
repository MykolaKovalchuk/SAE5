using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Ravlyk.Drawing.WinForms;
using Ravlyk.SAE.Drawing;
using Ravlyk.SAE.Drawing.Serialization;

namespace ImageConverter
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		void buttonLoad_Click(object sender, EventArgs e)
		{
			const string SupportedExtensions = "*.sa4; *.png; *.jpg; *.bmp; *.gif; *.tif";
		
			using (var openDialog = new OpenFileDialog { Filter = "Images (" + SupportedExtensions + ")|" + SupportedExtensions })
			{
				if (openDialog.ShowDialog(this) == DialogResult.OK)
				{
					if (Path.GetExtension(openDialog.FileName).Equals(".sa4", StringComparison.OrdinalIgnoreCase))
					{
						using (var stream = new FileStream(openDialog.FileName, FileMode.Open))
						{
							pictureBox1.Image?.Dispose();
							pictureBox1.Image = ImageSerializer.LoadFromStream(stream).ToBitmap();
						}
					}
					else
					{
						pictureBox1.Image = new Bitmap(openDialog.FileName);
					}
				}
			}
		}

		void buttonSave_Click(object sender, EventArgs e)
		{
			var bitmap = pictureBox1.Image as Bitmap;
			if (bitmap == null)
			{
				return;
			}

			using (var saveDialog = new SaveFileDialog { Filter = "SAE scheme (*.sa4)|*.sa4|PNG image (*.png)|*.png|JPG image (*.jpg)|*.jpg" })
			{
				if (saveDialog.ShowDialog(this) == DialogResult.OK)
				{
					switch (saveDialog.FilterIndex)
					{
						case 1:
							var image = CodedImage.FromIndexedImage(IndexedImageExtensions.FromBitmap(bitmap));
							using (var stream = new FileStream(saveDialog.FileName, FileMode.Create))
							{
								image.SaveToStream(stream);
							}
							break;
						case 2:
							bitmap.Save(saveDialog.FileName, ImageFormat.Png);
							break;
						case 3:
							bitmap.Save(saveDialog.FileName, ImageFormat.Jpeg);
							break;
						default:
							bitmap.Save(saveDialog.FileName);
							break;
					}
				}
			}
		}
	}
}
