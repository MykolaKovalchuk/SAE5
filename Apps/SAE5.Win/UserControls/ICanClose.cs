using System;
using System.ComponentModel;

namespace Ravlyk.SAE5.WinForms.UserControls
{
	interface ICanClose
	{
		void OnClosing(CancelEventArgs e);
	}
}
