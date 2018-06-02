using System;

namespace Ravlyk.SAE.Drawing.Processor
{
	public interface IImageProvider
	{
		CodedImage Image { get; }
		bool SupportsChangedEvent { get; }
		event EventHandler ImageChanged;
	}
}
