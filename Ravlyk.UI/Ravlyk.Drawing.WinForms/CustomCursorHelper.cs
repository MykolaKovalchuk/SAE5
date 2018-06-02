using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ravlyk.SAE.Resources;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// Custom cursor helper.
	/// </summary>
	public static class CustomCursorHelper
	{
		/// <summary>
		/// Custom cursor type
		/// </summary>
		public enum CursorType
		{
			/// <summary>
			/// Pen (pencil).
			/// </summary>
			Pen,
			/// <summary>
			/// Fill region (bucket).
			/// </summary>
			Fill
		}

		/// <summary>
		/// Creates custom cursor.
		/// </summary>
		/// <param name="cursorType">Cursor type.</param>
		/// <param name="cursorArgb">Cursor color.</param>
		/// <returns>Custom cursor of specified type and color.</returns>
		/// <remarks>Returns Cursors.Default if required resources have not been found.</remarks>
		public static Cursor GetCursor(CursorType cursorType, int cursorArgb)
		{
			var resourceName = GetCursorResourceName(cursorType);
			if (resourceName != null)
			{
				using (var stream = SAEResources.GetRawResourceStream(resourceName))
				{
					if (stream != null)
					{
						var image = IndexedImageExtensions.FromBitmap(new Bitmap(stream));
						if (image != null)
						{
							//ColorCursorImage(image, cursorArgb);
							return BitmapToCursor(image.ToBitmap());
						}
					}
				}
			}

			return Cursors.Default;
		}

		static string GetCursorResourceName(CursorType cursorType)
		{
			const string PenCursorResourceName = "pencil.png";
			const string FillCursorResourceName = "brush.png";

			switch (cursorType)
			{
				case CursorType.Pen:
					return PenCursorResourceName;
				case CursorType.Fill:
					return FillCursorResourceName;
				default:
					throw new ArgumentException("Unsupported cursor type", nameof(cursorType));
			}
		}

		static void ColorCursorImage(IndexedImage image, int argb)
		{
			int[] pixels;
			using (image.LockPixels(out pixels))
			{
				Parallel.For(0, pixels.Length,
					index =>
					{
						var pixelArgb = pixels[index];
						var a = pixelArgb.Alpha();
						if (a != 0) // Skip transparent
						{
							var r = pixelArgb.Red();
							var g = pixelArgb.Green();
							var b = pixelArgb.Blue();
							if (Math.Abs(r - g) + Math.Abs(g - b) + Math.Abs(b - r) <= 12) // Close to grey colors
							{
								pixels[index] = ColorBytes.ShadeColor(pixelArgb, argb);
							}
						}
					});
			}
		}

		static Cursor BitmapToCursor(Bitmap bitmap)
		{
			if (lastCursor != IntPtr.Zero)
			{
				DestroyIcon(lastCursor);
				lastCursor = IntPtr.Zero;
			}

			bitmap.MakeTransparent(bitmap.GetPixel(0, 0));
			var hIcon = bitmap.GetHicon();
			ICONINFO iconInfo;
			GetIconInfo(hIcon, out iconInfo);
			iconInfo.xHotspot = 1;
			iconInfo.yHotspot = bitmap.Height - 2;
			iconInfo.fIcon = false; // Cursor, not icon
			var hCursor = CreateIconIndirect(ref iconInfo); // Create the cursor

			lastCursor = hCursor;
			return new Cursor(hCursor);
		}

		static IntPtr lastCursor = IntPtr.Zero;

		#region API

		[StructLayout(LayoutKind.Sequential)]
		struct ICONINFO
		{
			public bool fIcon;         // Specifies whether this structure defines an icon or a cursor. A value of TRUE specifies an icon; FALSE specifies a cursor.
			public Int32 xHotspot;     // Specifies the x-coordinate of a cursor's hot spot. If this structure defines an icon, the hot spot is always in the center of the icon, and this member is ignored.
			public Int32 yHotspot;     // Specifies the y-coordinate of the cursor's hot spot. If this structure defines an icon, the hot spot is always in the center of the icon, and this member is ignored.
			public IntPtr hbmMask;     // (HBITMAP) Specifies the icon bitmask bitmap. If this structure defines a black and white icon,
									   // this bitmask is formatted so that the upper half is the icon AND bitmask and the lower half is
									   // the icon XOR bitmask. Under this condition, the height should be an even multiple of two. If
									   // this structure defines a color icon, this mask only defines the AND bitmask of the icon.
			public IntPtr hbmColor;    // (HBITMAP) Handle to the icon color bitmap. This member can be optional if this
									   // structure defines a black and white icon. The AND bitmask of hbmMask is applied with the SRCAND
									   // flag to the destination; subsequently, the color bitmap is applied (using XOR) to the
									   // destination by using the SRCINVERT flag.
		}

		[DllImport("user32.dll")]
		static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

		[DllImport("user32.dll")]
		static extern IntPtr CreateIconIndirect([In] ref ICONINFO piconinfo);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern bool DestroyIcon(IntPtr handle);


		#endregion
	}
}
