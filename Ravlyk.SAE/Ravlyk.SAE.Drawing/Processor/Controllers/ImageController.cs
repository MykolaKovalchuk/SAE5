using System;
using System.ComponentModel;
using System.Diagnostics;
using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.Processor
{
	public abstract class ImageController : IImageProvider, INotifyPropertyChanged
	{
		protected ImageController(ImageManipulator manipulator)
		{
			Debug.Assert(manipulator != null, "Image manipulator controller should have non-null image manipulator.");

			Manipulator = manipulator;
			Manipulator.ImageChanged += Manipulator_ImageChanged;
		}

		public ImageManipulator Manipulator { get; }

		public void CallManipulations()
		{
			if (internalPreventCallManipulations)
			{
				return;
			}

			needCallManipulations = true;
			if (!callManipulationsLock.IsLocked())
			{
				CallManipulationsCore();
				needCallManipulations = false;
			}
		}

		protected virtual void CallManipulationsCore()
		{
			// Add logic code in overridden classes
		}

		void Manipulator_ImageChanged(object sender, EventArgs e)
		{
			if (!needCallManipulations && !internalPreventCallManipulations) // We have not just changed the image
			{
				internalPreventCallManipulations = true;
				try
				{
					UpdateValuesFromManipulatedImage();
				}
				finally
				{
					internalPreventCallManipulations = false;
				}
			}
		}

		protected virtual void UpdateValuesFromManipulatedImage() { }

		#region Defaults

		public void RestoreDefaults()
		{
			RestoreDefaultsCore();
		}
		protected abstract void RestoreDefaultsCore();

		public void SaveDefaults()
		{
			SaveDefaultsCore();
		}
		protected abstract void SaveDefaultsCore();

		public void RestoreImageSettings(CodedImage image)
		{
			RestoreImageSettingsCore(image);
		}
		protected virtual void RestoreImageSettingsCore(CodedImage image) { }

		public void SaveImageSettings(CodedImage image)
		{
			SaveImageSettingsCore(image);
		}
		protected virtual void SaveImageSettingsCore(CodedImage image) { }

		#endregion

		#region SuspendCallManipulations

		public IDisposable SuspendCallManipulations()
		{
			return DisposableLock.Lock(ref callManipulationsLock, CallManipulationsIfNeeded);
		}
		DisposableLock callManipulationsLock;

		public void CallManipulationsIfNeeded()
		{
			if (needCallManipulations)
			{
				CallManipulations();
			}
		}
		bool needCallManipulations;

		bool internalPreventCallManipulations;

		#endregion

		#region IImageProvider members

		CodedImage IImageProvider.Image => Manipulator.SourceImage;

		bool IImageProvider.SupportsChangedEvent => Manipulator.ParentManipulator != null;

		event EventHandler IImageProvider.ImageChanged
		{
			add { Manipulator.SourceImageChanged += value; }
			remove { Manipulator.SourceImageChanged -= value; }
		}

		#endregion

		#region INotifyPropertyChanged

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}

	public abstract class ImageController<T> : ImageController
		where T : ImageManipulator
	{
		protected ImageController(T manipulator) : base(manipulator) { }

		public new T Manipulator => (T)base.Manipulator;
	}
}

