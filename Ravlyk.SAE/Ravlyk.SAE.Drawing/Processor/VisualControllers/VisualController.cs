using System;

using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.Processor
{
	public abstract class VisualController
	{
		#region TouchPointerStyle

		public enum TouchPointerStyle
		{
			None,
			Shift,
			ResizeHorizontal,
			ResizeVertical,
			ResizeLeftTop_RightBottom,
			ResizeRightTop_LeftBottom,
			ResizeAll,
			Cross
		}

		#endregion

		protected VisualController(IImageProvider imageProvider)
		{
			ImageProvider = imageProvider;
			if (ImageProvider.SupportsChangedEvent)
			{
				ImageProvider.ImageChanged += SourceImageChanged;
			}
		}

		void SourceImageChanged(object sender, EventArgs e)
		{
			using (SuspendUpdateVisualImage())
			{
				OnSourceImageChanged();
			}
		}

		protected abstract void OnSourceImageChanged();

		public IImageProvider ImageProvider { get; }

		public CodedImage SourceImage => ImageProvider.Image;

		public CodedImage VisualImage
		{
			get
			{
				if (visualImage == null)
				{
					visualImage = CreateVisualImage();
					UpdateVisualImage();
				}
				return visualImage;
			}
		}
		CodedImage visualImage;

		protected virtual CodedImage CreateVisualImage()
		{
			return SourceImage.Clone(false);
		}

		#region Touch actions

		protected bool IsTouching { get; private set; }

		public Point GetImagePoint(Point controllerPoint)
		{
			return TranslatePoint(controllerPoint);
		}

		protected virtual Point TranslatePoint(Point controllerPoint)
		{
			return controllerPoint;
		}

		public TouchPointerStyle GetTouchPointerStyle(Point controllerPoint)
		{
			return GetTouchPointerStyleCore(TranslatePoint(controllerPoint));
		}

		protected virtual TouchPointerStyle GetTouchPointerStyleCore(Point imagePoint)
		{
			return TouchPointerStyle.None;
		}

		public void OnTouched(Point controllerPoint)
		{
			lastPoint = TranslatePoint(controllerPoint);
			justTouched = true;
			OnTouchedCore(lastPoint);
			IsTouching = true;
		}

		protected virtual void OnTouchedCore(Point imagePoint) { }

		public void OnUntouched(Point controllerPoint)
		{
			IsTouching = false;
			lastPoint = TranslatePoint(controllerPoint);
			OnUntouchedCore(lastPoint);
			if (justTouched)
			{
				justTouched = false;
				OnClickedCore(lastPoint);
			}
		}

		protected virtual void OnUntouchedCore(Point imagePoint) { }

		protected virtual void OnClickedCore(Point imagePoint) { }

		public void OnShift(Point controllerPoint)
		{
			var newPoint = TranslatePoint(controllerPoint);

			var shiftSize = new Size(newPoint.X - lastPoint.X, newPoint.Y - lastPoint.Y);
			if (!justTouched || Math.Abs(shiftSize.Width) >= ShiftFilterLength || Math.Abs(shiftSize.Height) >= ShiftFilterLength)
			{
				justTouched = false;
				OnShiftCore(lastPoint, shiftSize);
				lastPoint = newPoint;
			}

			OnHoverCore(newPoint);
		}

		protected virtual void OnShiftCore(Point imagePoint, Size shiftSize) { }

		protected virtual void OnHoverCore(Point imagePoint) { }

		Point lastPoint;
		bool justTouched;
		const int ShiftFilterLength = 5;
		const int MouseSensitivity = 10;

		public void OnMouseWheel(int delta)
		{
			OnMouseWheelCore(delta / MouseSensitivity);
		}

		protected virtual void OnMouseWheelCore(int delta) { }

		#endregion

		#region Image updating

		public void UpdateVisualImage()
		{
			needUpdateVisualImage = true;
			if (!IsUpdateVisualImageSuspended && !inUpdateVisualImage)
			{
				inUpdateVisualImage = true;
				UpdateVisualImageCore();
				inUpdateVisualImage = false;
				needUpdateVisualImage = false;
				OnVisualImageChanged();
			}
		}

		bool inUpdateVisualImage;

		protected virtual void UpdateVisualImageCore()
		{
			// Add logic code in overridden classes
		}

		public IDisposable SuspendUpdateVisualImage()
		{
			return DisposableLock.Lock(ref updateVisualImageLock, UpdateVisualImageIfNeeded);
		}
		DisposableLock updateVisualImageLock;

		public bool IsUpdateVisualImageSuspended => visualImage == null || updateVisualImageLock.IsLocked();

		public void UpdateVisualImageIfNeeded()
		{
			if (needUpdateVisualImage)
			{
				UpdateVisualImage();
			}
		}
		bool needUpdateVisualImage;

		protected void OnVisualImageChanged()
		{
			VisualImageChanged?.Invoke(this, EventArgs.Empty);
		}

		public event EventHandler VisualImageChanged;
		
		#endregion
	}
}
