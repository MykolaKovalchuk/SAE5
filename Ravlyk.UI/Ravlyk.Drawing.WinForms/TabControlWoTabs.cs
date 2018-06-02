using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// TabControl without Tabs.
	/// </summary>
	public class TabControlWoTabs : TabControl
	{
		/// <summary>
		/// Gets the display area of the control's tab pages.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Drawing.Rectangle"/> that represents the display area of the tab pages.
		/// </returns>
		/// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
		public override Rectangle DisplayRectangle => DesignMode ? base.DisplayRectangle : new Rectangle(0, 0, Width, Height);
	}
}
