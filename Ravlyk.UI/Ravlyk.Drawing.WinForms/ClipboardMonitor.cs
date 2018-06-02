using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// Clipboard monitor.
	/// </summary>
	public static class ClipboardMonitor
	{
		static class NativeMethods
		{
			/// <summary>
			/// Places the given window in the system-maintained clipboard format listener list.
			/// </summary>
			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool AddClipboardFormatListener(IntPtr hwnd);

			/// <summary>
			/// Removes the given window from the system-maintained clipboard format listener list.
			/// </summary>
			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

			/// <summary>
			/// Changes the parent window of the specified child window.
			/// </summary>
			/// <returns></returns>
			[DllImport("user32.dll", SetLastError = true)]
			public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

			/// <summary>
			/// Sent when the contents of the clipboard have changed.
			/// </summary>
			public const int WM_CLIPBOARDUPDATE = 0x031D;

			/// <summary>
			/// To find message-only windows, specify HWND_MESSAGE in the hwndParent parameter of the FindWindowEx function.
			/// </summary>
			public static readonly IntPtr HWND_MESSAGE = new IntPtr(-3);
		}

		class NotificationForm : Form
		{
			public NotificationForm(Control monitorOwner, Action clipboardChangedHandler)
			{
				this.monitorOwner = monitorOwner;
				this.clipboardChangedHandler = clipboardChangedHandler;

				NativeMethods.SetParent(Handle, NativeMethods.HWND_MESSAGE);
				NativeMethods.AddClipboardFormatListener(Handle);
			}

			Control monitorOwner;
			Action clipboardChangedHandler;

			protected override void WndProc(ref Message m)
			{
				if (m.Msg == NativeMethods.WM_CLIPBOARDUPDATE)
				{
					OnClipboardChanged();
				}

				base.WndProc(ref m);
			}

			void OnClipboardChanged()
			{
				if (clipboardChangedHandler != null)
				{
					if (monitorOwner != null && monitorOwner.InvokeRequired)
					{
						monitorOwner.Invoke(clipboardChangedHandler);
					}
					else
					{
						clipboardChangedHandler.Invoke();
					}
				}
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					try
					{
						NativeMethods.RemoveClipboardFormatListener(Handle);
					}
					catch
					{
#if DEBUG
						throw;
#endif
						// Ignore any error in release.
					}

					clipboardChangedHandler = null;
					monitorOwner = null;
				}

				base.Dispose(disposing);
			}
		}

		/// <summary>
		/// Starts monitoring Clipboard.
		/// </summary>
		/// <returns>Disposable object to keep monitoring Clipboard. If hooking was unsuccessfull, it will return null.</returns>
		public static IDisposable StartMonitor(Control owner, Action clipboardChangedHandler)
		{
			try
			{
				return new NotificationForm(owner, clipboardChangedHandler);
			}
			catch
			{
				return null;
			}
		}
	}
}
