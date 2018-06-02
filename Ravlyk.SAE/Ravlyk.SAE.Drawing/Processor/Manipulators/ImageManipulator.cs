using System;
using Ravlyk.Common;
using System.Diagnostics;

namespace Ravlyk.SAE.Drawing.Processor
{
	public abstract class ImageManipulator : IImageProvider
	{
		protected ImageManipulator(CodedImage sourceImage)
		{
			Debug.Assert(sourceImage != null, "Image manipulator should have non-null source image.");

			SourceImage = sourceImage;
		}

		protected ImageManipulator(ImageManipulator parentManipulator)
		{
			Debug.Assert(parentManipulator != null, "Parent manipulator here really cannot be null.");

			ParentManipulator = parentManipulator;
			RestoreManipulationsWhenParentManipulatorChanged = true;
			parentManipulator.ImageChanged += ParentManipulatorImageChanged;
		}

		protected internal ImageManipulator ParentManipulator { get; }

		public CodedImage SourceImage
		{
			get
			{
				if (sourceImage == null && ParentManipulator != null)
				{
					sourceImage = ParentManipulator.ManipulatedImage;
				}
				return sourceImage;
			}
			private set { sourceImage = value; }
		}
		CodedImage sourceImage;

		public CodedImage ManipulatedImage
		{
			get
			{
				if (manipulatedImage == null)
				{
					manipulatedImage = CreateManipulatedImage(SourceImage);
					RestoreManipulations();
				}
				return manipulatedImage;
			}
		}
		CodedImage manipulatedImage;

		public bool IsManipulatedImageInitialized => manipulatedImage != null;

		protected virtual CodedImage CreateManipulatedImage(CodedImage originalImage)
		{
			var newImage = originalImage.Clone(true);
			newImage.ResetAdditionalData();
			return newImage;
		}

		#region Reset and restore

		public bool RestoreManipulationsWhenParentManipulatorChanged { get; set; }

		public void Reset()
		{
			if (IsManipulatedImageInitialized)
			{
				using (DisposableLock.Lock(ref imageChangedLock, OnImageChanged))
				{
					CopySourceImage();
					ResetCore();
				}
			}
		}

		protected virtual void ResetCore()
		{
			// Add reset logic in overridden classes
		}

		public void RestoreManipulations()
		{
			if (IsManipulatedImageInitialized)
			{
				using (DisposableLock.Lock(ref imageChangedLock, OnImageChanged))
				{
					CopySourceImage();
					RestoreManipulationsCore();
				}
			}
		}

		protected virtual void RestoreManipulationsCore()
		{
			// Add restore logic in overridden classes
		}

		void CopySourceImage()
		{
			if (!ReferenceEquals(ManipulatedImage, SourceImage))
			{
				CopySourceImageCore();
			}
		}

		protected virtual void CopySourceImageCore()
		{
			ManipulatedImage.Size = SourceImage.Size;
			int[] sourcePixels, manipulatedPixels;
			using (SourceImage.LockPixels(out sourcePixels))
			using (ManipulatedImage.LockPixels(out manipulatedPixels))
			{
				sourcePixels.CopyTo(manipulatedPixels, 0);
			}
		}

		#endregion

		#region Source image changed handler

		protected void OnImageChanged()
		{
			if (IsManipulatedImageInitialized && ImageChanged != null && !imageChangedLock.IsLocked())
			{
				ImageChanged(this, EventArgs.Empty);
			}
		}

		public event EventHandler ImageChanged;
		DisposableLock imageChangedLock;

		void ParentManipulatorImageChanged(object sender, EventArgs e)
		{
			SourceImageChanged?.Invoke(sender, e);

			if (RestoreManipulationsWhenParentManipulatorChanged)
			{
				RestoreManipulations();
			}
			else
			{
				Reset();
			}
		}

		public event EventHandler SourceImageChanged;

		#endregion

		#region IImageProvider

		CodedImage IImageProvider.Image => ManipulatedImage;

		bool IImageProvider.SupportsChangedEvent => true;

		#endregion
	}
}
