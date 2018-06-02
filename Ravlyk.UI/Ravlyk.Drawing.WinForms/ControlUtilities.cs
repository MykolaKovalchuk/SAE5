using System;
using System.Windows.Forms;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// Helper class with useful methods for <see cref="Control"/>.
	/// </summary>
	public static class ControlUtilities
	{
		/// <summary>
		/// Unsubscribes event handler from event Disposed on control object, if it is not null.
		/// </summary>
		/// <param name="component">Control object.</param>
		/// <param name="handler">Disposed event handler.</param>
		public static void UnsubscribeDisposed(object component, EventHandler handler)
		{
			var control = component as Control;
			if (control != null)
			{
				control.Disposed -= handler;
			}
		}
	}
}
