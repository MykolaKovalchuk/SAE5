using System;
using System.Collections.Generic;
using System.Text;
using Ravlyk.Drawing.ImageProcessor;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class ImageRotateManipulator : ImageManipulator
	{
		public ImageRotateManipulator(CodedImage sourceImage) : base(sourceImage) { }

		public ImageRotateManipulator(ImageManipulator parentManipulator) : base(parentManipulator)
		{
			RestoreManipulationsWhenParentManipulatorChanged = false;
		}

		public void RotateCW()
		{
			ImageRotator.RotateCWInPlace(ManipulatedImage);
			RotateManupulations.Add(CW);
			OnImageChanged();
		}

		public void RotateCCW()
		{
			ImageRotator.RotateCCWInPlace(ManipulatedImage);
			RotateManupulations.Add(CCW);
			OnImageChanged();
		}

		public void FlipHorizontally()
		{
			ImageRotator.FlipHorizontallyInPlace(ManipulatedImage);
			RotateManupulations.Add(H);
			OnImageChanged();
		}

		public void FlipVertically()
		{
			ImageRotator.FlipVerticallyInPlace(ManipulatedImage);
			RotateManupulations.Add(V);
			OnImageChanged();
		}

		#region Restore manipulations

		internal void RestoreManipulations(string manipulations)
		{
			SetManipulations(manipulations);
			RestoreManipulations();
		}

		internal string GetManipulationsHistory()
		{
			NormilizeManipulationsHistory();
			return ManipulationsToString()?.ToString();
		}

		const string H = "H";
		const string V = "V";
		const string CW = "W";
		const string CCW = "C";

		protected override void ResetCore()
		{
			base.ResetCore();
			rotateManupulations = null;
		}

		protected override void RestoreManipulationsCore()
		{
			base.RestoreManipulationsCore();

			NormilizeManipulationsHistory();
			if (rotateManupulations == null)
			{
				return;
			}

			foreach (var manipulation in rotateManupulations.ToArray())
			{
				switch (manipulation)
				{
					case CW:
						RotateCW();
						break;
					case CCW:
						RotateCCW();
						break;
					case H:
						FlipHorizontally();
						break;
					case V:
						FlipVertically();
						break;
				}
			}
		}

		void NormilizeManipulationsHistory()
		{
			if (rotateManupulations == null)
			{
				return;
			}

			var sb = ManipulationsToString();

			int initialLenght;
			do
			{
				initialLenght = sb.Length;

				sb.Replace(H + H, "");
				sb.Replace(V + V, "");
				sb.Replace(CW + CW + CW + CW, "");
				sb.Replace(CCW + CCW + CCW + CCW, "");
				sb.Replace(CW + CCW, "");
				sb.Replace(CCW + CW, "");

				sb.Replace(H + V + H + V, "");
				sb.Replace(V + H + V + H, "");

				sb.Replace(H + V + CW + CW, "");
				sb.Replace(H + V + CCW + CCW, "");
				sb.Replace(V + H + CW + CW, "");
				sb.Replace(V + H + CCW + CCW, "");

				sb.Replace(CW + CW + H + V, "");
				sb.Replace(CCW + CCW + H + V, "");
				sb.Replace(CW + CW + V + H, "");
				sb.Replace(CCW + CCW + V + H, "");

				sb.Replace(H + CW + CW, V);
				sb.Replace(H + CCW + CCW, V);
				sb.Replace(CW + CW + H, V);
				sb.Replace(CCW + CCW + H, V);

				sb.Replace(V + CW + CW, H);
				sb.Replace(V + CCW + CCW, H);
				sb.Replace(CW + CW + V, H);
				sb.Replace(CCW + CCW + V, H);

				sb.Replace(CW + H + CW, H);
				sb.Replace(CCW + H + CCW, H);
				sb.Replace(CW + V + CW, V);
				sb.Replace(CCW + V + CCW, V);

			} while (sb.Length < initialLenght);

			SetManipulations(sb.ToString());
		}

		void SetManipulations(string manipulations)
		{
			rotateManupulations = null;
			if (manipulations.Length > 0)
			{
				foreach (var mChar in manipulations)
				{
					RotateManupulations.Add(mChar.ToString());
				}
			}
		}

		StringBuilder ManipulationsToString()
		{
			if (rotateManupulations == null)
			{
				return null;
			}

			var sb = new StringBuilder();
			foreach (var manipulation in rotateManupulations)
			{
				sb.Append(manipulation);
			}
			return sb;
		}

		List<string> RotateManupulations => rotateManupulations ?? (rotateManupulations = new List<string>());
		List<string> rotateManupulations;

		#endregion
	}
}
