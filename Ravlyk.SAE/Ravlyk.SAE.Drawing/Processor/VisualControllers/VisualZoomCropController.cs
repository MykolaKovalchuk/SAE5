using System;
using System.Collections.Generic;
using Ravlyk.Common;
using Ravlyk.Drawing;
using Ravlyk.Drawing.ImageProcessor;
using System.Threading.Tasks;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class VisualZoomCropController : VisualZoomController
	{
		public VisualZoomCropController(ImageCropController controller, Size imageBoxSize = default(Size)) : base(controller, imageBoxSize)
		{
			Controller = controller;
		}

		protected override void OnSourceImageChanged()
		{
			FitImage();
			base.OnSourceImageChanged();
		}

		public ImageCropController Controller { get; }

		public Rectangle CropRect
		{
			get
			{
				if (ZoomPercent == 100)
				{
					return Controller.CropRect;
				}
				else if (zoomedCropRect != default(Rectangle))
				{
					return zoomedCropRect;
				}
				else
				{
					var cropRect = Controller.CropRect;
					zoomedCropRect = new Rectangle(cropRect.Left * ZoomPercent / 100, cropRect.Top * ZoomPercent / 100, cropRect.Width * ZoomPercent / 100, cropRect.Height * ZoomPercent / 100);
					return zoomedCropRect;
				}
			}
			private set
			{
				var oldCropRect = CropRect;
				if (value == oldCropRect)
				{
					return;
				}

				if (ZoomPercent == 100)
				{
					Controller.CropRect = value;
				}
				else
				{
					zoomedCropRect = value;

					var imageCorpRect = new Rectangle(zoomedCropRect.Left * 100 / ZoomPercent, zoomedCropRect.Top * 100 / ZoomPercent, zoomedCropRect.Width * 100 / ZoomPercent, zoomedCropRect.Height * 100 / ZoomPercent);
					Controller.CropRect = imageCorpRect;
					if (Controller.CropRect != imageCorpRect)
					{
						zoomedCropRect = default(Rectangle);
					}
				}

				quickUpdate = !IsUpdateVisualImageSuspended;
				dirtyAreas = quickUpdate && CropKind == ImageCropper.CropKind.Rectangle ? GetDirtyAreas(CropRect, oldCropRect) : null;

				UpdateVisualImage();
			}
		}
		Rectangle zoomedCropRect = default(Rectangle);

		public ImageCropper.CropKind CropKind
		{
			get { return Controller.CropKind; }
			set
			{
				if (value != CropKind)
				{
					using (Controller.SuspendCallManipulations())
					{
						var wasNone = CropKind == ImageCropper.CropKind.None;
						Controller.CropKind = value;

						quickUpdate = !IsUpdateVisualImageSuspended;
						dirtyAreas = null;

						if (wasNone)
						{
							zoomedCropRect = default(Rectangle);

							var dx = Controller.Manipulator.SourceImage.Size.Width / 10;
							var dy = Controller.Manipulator.SourceImage.Size.Height / 10;
							Controller.CropRect = new Rectangle(dx, dy, Controller.Manipulator.SourceImage.Size.Width - dx * 2, Controller.Manipulator.SourceImage.Size.Height - dy * 2);
						}

						UpdateVisualImage();
					}
				}
			}
		}

		#region Update visual image

		IEnumerable<Rectangle> GetDirtyAreas(Rectangle newCropRect, Rectangle oldCropRect)
		{
			var result = new List<Rectangle>();
			result.AddRange(newCropRect.Xor(oldCropRect));
			AddDragingPointDirtyAreas(newCropRect.Left, newCropRect.Top, oldCropRect.Left, oldCropRect.Top, result);
			AddDragingPointDirtyAreas(newCropRect.Left + newCropRect.Width / 2, newCropRect.Top, oldCropRect.Left + oldCropRect.Width / 2, oldCropRect.Top, result);
			AddDragingPointDirtyAreas(newCropRect.RightExclusive - 1, newCropRect.Top, oldCropRect.RightExclusive - 1, oldCropRect.Top, result);
			AddDragingPointDirtyAreas(newCropRect.Left, newCropRect.Top + newCropRect.Height / 2, oldCropRect.Left, oldCropRect.Top + oldCropRect.Height / 2, result);
			AddDragingPointDirtyAreas(newCropRect.RightExclusive - 1, newCropRect.Top + newCropRect.Height / 2, oldCropRect.RightExclusive - 1, oldCropRect.Top + oldCropRect.Height / 2, result);
			AddDragingPointDirtyAreas(newCropRect.Left, newCropRect.BottomExclusive - 1, oldCropRect.Left, oldCropRect.BottomExclusive - 1, result);
			AddDragingPointDirtyAreas(newCropRect.Left + newCropRect.Width / 2, newCropRect.BottomExclusive - 1, oldCropRect.Left + oldCropRect.Width / 2, oldCropRect.BottomExclusive - 1, result);
			AddDragingPointDirtyAreas(newCropRect.RightExclusive - 1, newCropRect.BottomExclusive - 1, oldCropRect.RightExclusive - 1, oldCropRect.BottomExclusive - 1, result);
			return result;
		}

		void AddDragingPointDirtyAreas(int x1, int y1, int x2, int y2, List<Rectangle> list)
		{
			if (x1 != x2 || y1 != y2)
			{
				var rect1 = new Rectangle(x1 - DragginPointRadius, y1 - DragginPointRadius, DragginPointWidth, DragginPointWidth);
				var rect2 = new Rectangle(x2 - DragginPointRadius, y2 - DragginPointRadius, DragginPointWidth, DragginPointWidth);
				list.AddRange(rect1.Xor(rect2));
			}
		}

		bool quickUpdate;
		int[] originalVisualImagePixels;
		IEnumerable<Rectangle> dirtyAreas;

		protected override void UpdateParameters()
		{
			base.UpdateParameters();
			zoomedCropRect = default(Rectangle);
			originalVisualImagePixels = null;
		}

		protected override void UpdateVisualImageCore()
		{
			if (!quickUpdate || originalVisualImagePixels == null)
			{
				base.UpdateVisualImageCore();
				originalVisualImagePixels = new int[VisualImage.Pixels.Length];
				VisualImage.Pixels.CopyTo(originalVisualImagePixels, 0);
				quickUpdate = false;
				dirtyAreas = null;
			}

			if (quickUpdate && dirtyAreas != null)
			{
				foreach (var area in dirtyAreas)
				{
					UpdateVisualImageArea(area);
				}
			}
			else
			{
				UpdateVisualImageArea(VisualImageFrame);
			}

			quickUpdate = false;
			dirtyAreas = null;
		}

		void UpdateVisualImageArea(Rectangle area)
		{
			if (area.Left < VisualImageFrame.Left) area.Left = VisualImageFrame.Left;
			if (area.Top < VisualImageFrame.Top) area.Top = VisualImageFrame.Top;
			if (area.RightExclusive > VisualImageFrame.RightExclusive) area.Width = VisualImageFrame.RightExclusive - area.Left;
			if (area.BottomExclusive > VisualImageFrame.BottomExclusive) area.Height = VisualImageFrame.BottomExclusive - area.Top;

			if (area.Width > 0 && area.Height > 0)
			{
				Parallel.For(area.Top, area.BottomExclusive,
					frameY =>
					{
						for (int frameX = area.Left, visualIndex = (frameY - VisualImageFrame.Top) * VisualImage.Size.Width + area.Left - VisualImageFrame.Left;
							frameX < area.RightExclusive;
							frameX++, visualIndex++)
						{
							if (IsZoomedPixelAboveDraggingPoint(frameX, frameY))
							{
								VisualImage.Pixels[visualIndex] = originalVisualImagePixels[visualIndex].HalfToneLow();
							}
							else if (CropKind == ImageCropper.CropKind.None || IsZoomedPixelInsideCropRect(frameX, frameY))
							{
								VisualImage.Pixels[visualIndex] = originalVisualImagePixels[visualIndex];
							}
							else
							{
								VisualImage.Pixels[visualIndex] = originalVisualImagePixels[visualIndex].ThreeQuarterToneHigh();
							}
						}
					});
			}
		}

		bool IsZoomedPixelInsideCropRect(int x, int y)
		{
			bool result = false;
			switch (CropKind)
			{
				case ImageCropper.CropKind.Arc:
					result = ImageCropper.IsPointInsideArc(new Point(x - CropRect.Left, y - CropRect.Top), CropRect);
					break;
				case ImageCropper.CropKind.Rectangle:
					result =
						x >= CropRect.Left && x < CropRect.RightExclusive &&
						y >= CropRect.Top && y < CropRect.BottomExclusive;
					break;
			}
			return result;
		}

		bool IsZoomedPixelAboveDraggingPoint(int x, int y)
		{
			if (CropKind == ImageCropper.CropKind.None)
			{
				return false;
			}

			var xNearCenter = IsInsideRadius(x, CropRect.Left + CropRect.Width / 2);
			var yNearCenter = IsInsideRadius(y, CropRect.Top + CropRect.Height / 2);
			var xNearDraggingWidth = xNearCenter || IsInsideRadius(x, CropRect.Left) || IsInsideRadius(x, CropRect.RightExclusive - 1);
			var yNearDraggingHeight = yNearCenter || IsInsideRadius(y, CropRect.Top) || IsInsideRadius(y, CropRect.BottomExclusive - 1);

			return xNearDraggingWidth && yNearDraggingHeight && !(xNearCenter && yNearCenter);
		}

		internal static bool IsInsideRadius(int x1, int x2, int radius = DragginPointRadius)
		{
			return Math.Abs(x1 - x2) <= radius;
		}

		const int DragginPointRadius = 5;
		const int DragginPointWidth = DragginPointRadius * 2 + 1;

		#endregion

		#region Touch actions

		protected override TouchPointerStyle GetTouchPointerStyleCore(Point imagePoint)
		{
			if (CropKind != ImageCropper.CropKind.None)
			{
				if (IsTouching)
				{
					return originalTouchPointerStyle;
				}

				if (IsInsideRadius(imagePoint.X, CropRect.Left) && IsInsideRadius(imagePoint.Y, CropRect.Top) ||
					IsInsideRadius(imagePoint.X, CropRect.RightExclusive - 1) && IsInsideRadius(imagePoint.Y, CropRect.BottomExclusive - 1))
				{
					return TouchPointerStyle.ResizeLeftTop_RightBottom;
				}
				if (IsInsideRadius(imagePoint.X, CropRect.Left) && IsInsideRadius(imagePoint.Y, CropRect.BottomExclusive - 1) ||
					IsInsideRadius(imagePoint.X, CropRect.RightExclusive - 1) && IsInsideRadius(imagePoint.Y, CropRect.Top))
				{
					return TouchPointerStyle.ResizeRightTop_LeftBottom;
				}
				if ((IsInsideRadius(imagePoint.X, CropRect.Left) || IsInsideRadius(imagePoint.X, CropRect.RightExclusive - 1)) && IsInsideRadius(imagePoint.Y, CropRect.Top + CropRect.Height / 2))
				{
					return TouchPointerStyle.ResizeHorizontal;
				}
				if (IsInsideRadius(imagePoint.X, CropRect.Left + CropRect.Width / 2) && (IsInsideRadius(imagePoint.Y, CropRect.Top) || IsInsideRadius(imagePoint.Y, CropRect.BottomExclusive - 1)))
				{
					return TouchPointerStyle.ResizeVertical;
				}
				if (IsZoomedPixelInsideCropRect(imagePoint.X, imagePoint.Y) &&
					(CropRect.Left > 0 || CropRect.Top > 0 || CropRect.Width < VisualImage.Size.Width || CropRect.Height < VisualImage.Size.Height))
				{
					return TouchPointerStyle.ResizeAll;
				}
			}

			return base.GetTouchPointerStyleCore(imagePoint);
		}
		TouchPointerStyle originalTouchPointerStyle;

		protected override void OnShiftCore(Point imagePoint, Size shiftSize)
		{
			if (IsTouching && CropKind != ImageCropper.CropKind.None)
			{
				switch (originalTouchPoint)
				{
					case TouchPoint.Left:
						CropRect = new Rectangle(CropRect.Left + shiftSize.Width, CropRect.Top, CropRect.Width - shiftSize.Width, CropRect.Height);
						break;
					case TouchPoint.Right:
						CropRect = new Rectangle(CropRect.Left, CropRect.Top, CropRect.Width + shiftSize.Width, CropRect.Height);
						break;
					case TouchPoint.Top:
						CropRect = new Rectangle(CropRect.Left, CropRect.Top + shiftSize.Height, CropRect.Width, CropRect.Height - shiftSize.Height);
						break;
					case TouchPoint.Bottom:
						CropRect = new Rectangle(CropRect.Left, CropRect.Top, CropRect.Width, CropRect.Height + shiftSize.Height);
						break;
					case TouchPoint.LeftTop:
						CropRect = new Rectangle(CropRect.Left + shiftSize.Width, CropRect.Top + shiftSize.Height, CropRect.Width - shiftSize.Width, CropRect.Height - shiftSize.Height);
						break;
					case TouchPoint.RightBottom:
						CropRect = new Rectangle(CropRect.Left, CropRect.Top, CropRect.Width + shiftSize.Width, CropRect.Height + shiftSize.Height);
						break;
					case TouchPoint.LeftBottom:
						CropRect = new Rectangle(CropRect.Left + shiftSize.Width, CropRect.Top, CropRect.Width - shiftSize.Width, CropRect.Height + shiftSize.Height);
						break;
					case TouchPoint.RightTop:
						CropRect = new Rectangle(CropRect.Left, CropRect.Top + shiftSize.Height, CropRect.Width + shiftSize.Width, CropRect.Height - shiftSize.Height);
						break;
					case TouchPoint.Middle:
						CropRect = new Rectangle(CropRect.Left + shiftSize.Width, CropRect.Top + shiftSize.Height, CropRect.Width, CropRect.Height);
						break;
					default:
						base.OnShiftCore(imagePoint, shiftSize);
						break;
				}
			}
			else
			{
				base.OnShiftCore(imagePoint, shiftSize);
			}
		}

		protected override void OnTouchedCore(Point imagePoint)
		{
			base.OnTouchedCore(imagePoint);
			UnsuspendControllerSafe();
			controllerSuspender = Controller.SuspendCallManipulations();
			originalTouchPointerStyle = GetTouchPointerStyleCore(imagePoint);
			originalTouchPoint = GetTouchPoint(imagePoint);
		}

		protected override void OnUntouchedCore(Point imagePoint)
		{
			base.OnUntouchedCore(imagePoint);
			UnsuspendControllerSafe();
			originalTouchPoint = TouchPoint.None;
			originalTouchPointerStyle = TouchPointerStyle.None;
		}

		internal enum TouchPoint
		{
			None,
			LeftTop,
			Top,
			RightTop,
			Left,
			Right,
			LeftBottom,
			Bottom,
			RightBottom,
			Middle
		}
		TouchPoint originalTouchPoint;

		TouchPoint GetTouchPoint(Point imagePoint)
		{
			if (CropKind == ImageCropper.CropKind.None)
			{
				return TouchPoint.None;
			}

			return GetTouchPoint(imagePoint, GetTouchPointerStyleCore(imagePoint), CropRect);
		}

		internal static TouchPoint GetTouchPoint(Point imagePoint, TouchPointerStyle touchPointerStyle, Rectangle cropRect)
		{
			switch (touchPointerStyle)
			{
				case TouchPointerStyle.ResizeHorizontal:
					return IsInsideRadius(imagePoint.X, cropRect.Left) ? TouchPoint.Left : TouchPoint.Right;
				case TouchPointerStyle.ResizeVertical:
					return IsInsideRadius(imagePoint.Y, cropRect.Top) ? TouchPoint.Top : TouchPoint.Bottom;
				case TouchPointerStyle.ResizeLeftTop_RightBottom:
					return IsInsideRadius(imagePoint.X, cropRect.Left) ? TouchPoint.LeftTop : TouchPoint.RightBottom;
				case TouchPointerStyle.ResizeRightTop_LeftBottom:
					return IsInsideRadius(imagePoint.X, cropRect.Left) ? TouchPoint.LeftBottom : TouchPoint.RightTop;
				case TouchPointerStyle.ResizeAll:
					return TouchPoint.Middle;
				default:
					return TouchPoint.None;
			}
		}

		void UnsuspendControllerSafe()
		{
			if (controllerSuspender != null)
			{
				var localReference = controllerSuspender;
				controllerSuspender = null;
				localReference.Dispose();
			}
		}

		IDisposable controllerSuspender;

		#endregion

		#region Test stuff
		#if DEBUG

		internal void SetCropRectForTest(Rectangle newCropRect)
		{
			CropRect = newCropRect;
		}

		#endif
		#endregion
	}
}
