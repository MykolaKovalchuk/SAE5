using System;
using System.Drawing;
using System.Windows.Forms;

using Ravlyk.SAE.Drawing.Processor;

using Point = Ravlyk.Common.Point;
using Size = Ravlyk.Common.Size;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// Base control for <see cref="VisualBoxedController"/>.
	/// </summary>
	public class VisualControl : Control
	{
		/// <summary>
		/// Initializes new VisualControl instance.
		/// </summary>
		public VisualControl()
		{
			DoubleBuffered = true;
			MinimumSize = new System.Drawing.Size(VisualBoxedController.MinimumWidth, VisualBoxedController.MinimumWidth);
		}

		/// <summary>
		/// Initializes new VisualControl instance.
		/// </summary>
		/// <param name="initialController"><see cref="VisualBoxedController"/> instance to initialize <see cref="Controller"/>.</param>
		public VisualControl(VisualBoxedController initialController)
			: this()
		{
			Controller = initialController;
		}

		#region Painting

		/// <summary>
		/// Paints image from <see cref="IndexedImage"/> on control's surface.
		/// </summary>
		/// <param name="pe">Paint event arguments.</param>
		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);

			#if DEBUG
			if (DesignMode)
			{
				using (var framePen = new Pen(System.Drawing.Color.DeepSkyBlue))
				{
					pe.Graphics.DrawRectangle(framePen, 0, 0, Width - 1, Height - 1);
				}
			}
			#endif

			if (Controller != null)
			{
				Bitmap bitmap = null;
				if (tempBitmap == null || !tempBitmap.TryGetTarget(out bitmap) || bitmap.Width != Controller.VisualImage.Size.Width || bitmap.Height != Controller.VisualImage.Size.Height)
				{
					bitmap?.Dispose();
					bitmap = Controller.VisualImage.ToBitmap();
					tempBitmap = new WeakReference<Bitmap>(bitmap);
				}
				else
				{
					Controller.VisualImage.UpdateBitmap(bitmap);
				}

				dx = Math.Max((Width - Controller.VisualImage.Size.Width) / 2, 0);
				dy = Math.Max((Height - Controller.VisualImage.Size.Height) / 2, 0);
				pe.Graphics.DrawImage(bitmap, dx, dy);
			}
		}

		WeakReference<Bitmap> tempBitmap;
		int dx, dy;

		#endregion

		#region VisualBoxedController

		/// <summary>
		/// Gets or sets <see cref="VisualBoxedController"/> associated with this control.
		/// </summary>
		public VisualBoxedController Controller
		{
			get { return controller; }
			set
			{
				if (value == controller)
				{
					return;
				}

				if (controller != null)
				{
					controller.VisualImageChanged -= OnVisualImageChanged;
				}

				controller = value;

				if (controller != null)
				{
					controller.VisualImageChanged += OnVisualImageChanged;
					controller.ImageBoxSize = new Size(Width, Height);
				}

				OnVisualImageChanged(this, EventArgs.Empty);
			}
		}
		VisualBoxedController controller;

		void OnVisualImageChanged(object sender, EventArgs e)
		{
			Invalidate();
			Update();
		}

		#endregion

		#region Mouse Interaction

		/// <summary>
		/// Controller touched event arguments.
		/// </summary>
		public class ControllerTouchedEventArgs : EventArgs
		{
			/// <summary>
			/// Initializes instance of ControllerTouchedEventArgs.
			/// </summary>
			/// <param name="touchPoint">Touch point.</param>
			public ControllerTouchedEventArgs(Point touchPoint)
			{
				TouchPoint = touchPoint;
			}

			/// <summary>
			/// Touch point.
			/// </summary>
			public Point TouchPoint { get; }
		}

		/// <summary>
		/// Occurs when left mouse button is pressed down.
		/// </summary>
		public event EventHandler<ControllerTouchedEventArgs> ControllerTouched;

		void OnControllerMouseTouched(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ControllerTouched?.Invoke(this, new ControllerTouchedEventArgs(ToControllerPoint(e.X, e.Y)));
			}
		}

		/// <summary>
		/// Handles mouse down event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			Controller?.OnTouched(ToControllerPoint(e.X, e.Y));
			OnControllerMouseTouched(e);
		}

		/// <summary>
		/// Handles mouse up event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			Controller?.OnUntouched(ToControllerPoint(e.X, e.Y));
		}

		/// <summary>
		/// Handles mouse move event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (Controller != null)
			{
				var controllerPoint = ToControllerPoint(e.X, e.Y);
				Controller.OnShift(controllerPoint);

				if (overrideCursor != null)
				{
					Cursor = overrideCursor;
				}
				else
				{
					switch (Controller.GetTouchPointerStyle(controllerPoint))
					{
						case VisualController.TouchPointerStyle.ResizeHorizontal:
							Cursor = Cursors.SizeWE;
							break;
						case VisualController.TouchPointerStyle.ResizeVertical:
							Cursor = Cursors.SizeNS;
							break;
						case VisualController.TouchPointerStyle.ResizeLeftTop_RightBottom:
							Cursor = Cursors.SizeNWSE;
							break;
						case VisualController.TouchPointerStyle.ResizeRightTop_LeftBottom:
							Cursor = Cursors.SizeNESW;
							break;
						case VisualController.TouchPointerStyle.ResizeAll:
							Cursor = Cursors.SizeAll;
							break;
						case VisualController.TouchPointerStyle.Shift:
							Cursor = Cursors.Hand;
							break;
						case VisualController.TouchPointerStyle.Cross:
							Cursor = Cursors.Cross;
							break;
						default:
							Cursor = DefaultCursor;
							break;
					}
				}
			}
		}

		/// <summary>
		/// Overrides mouse cursor pointer.
		/// </summary>
		public Cursor OverrideCursor
		{
			get { return overrideCursor; }
			set
			{
				overrideCursor = value;
				Cursor = value ?? DefaultCursor;
			}
		}
		Cursor overrideCursor;

		/// <summary>
		/// Handles mouse wheel event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			Controller?.OnMouseWheel(e.Delta);
		}

		Point ToControllerPoint(int x, int y)
		{
			return new Point(x, y);
		}

		#endregion

		#region Implementation

		/// <summary>
		/// Updates <see cref="VisualBoxedController.ImageBoxSize"/> with new size.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			if (Controller != null)
			{
				Controller.ImageBoxSize = new Size(Width, Height);
			}
		}

		/// <summary>
		/// Disposes used data.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			Controller = null;

			if (disposing && tempBitmap != null)
			{
				Bitmap bitmap;
				tempBitmap.TryGetTarget(out bitmap);
				bitmap?.Dispose();
				tempBitmap = null;
			}
		}

		#endregion
	}
}
