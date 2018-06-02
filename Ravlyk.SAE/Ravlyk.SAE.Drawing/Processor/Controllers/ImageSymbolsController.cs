using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class ImageSymbolsController : ImageController<ImageSymbolsManipulator>
	{
		public ImageSymbolsController(ImageSymbolsManipulator manipulator, IEnumerable<TrueTypeFont> availableFonts = null, TrueTypeFont selectedFont = null) : base(manipulator)
		{
			Debug.Assert(selectedFont == null || availableFonts != null && availableFonts.Contains(selectedFont), "selectedFont should be included into availableFonts collection.");

			using (SuspendCallManipulations())
			{
				AvailableFonts = availableFonts;
				SymbolsFont = selectedFont ?? availableFonts?.FirstOrDefault();

				RestoreDefaults();
				CallManipulations();
			}

			manipulator.SourceImageChanged += Manipulator_SourceImageChanged;
		}

		#region Symbols font

		public TrueTypeFont SymbolsFont
		{
			get { return symbolsFont; }
			set
			{
				symbolsFont = value;
				RebuildAvailableSymbols();
				CallManipulations();
				OnPropertyChanged(nameof(SymbolsFontName));
			}
		}
		TrueTypeFont symbolsFont;

		public string SymbolsFontName
		{
			get { return SymbolsFont != null ? SymbolsFont.Name : ""; }
			set
			{
				if (SymbolsFontName.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					return;
				}

				var newFont = AvailableFonts?.FirstOrDefault(font => font.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
				if (newFont == null)
				{
					return;
				}

				SymbolsFont = newFont;

				OnPropertyChanged(nameof(SymbolsFontName));
			}
		}

		public IEnumerable<TrueTypeFont> AvailableFonts { get; private set; }

		public IEnumerable<string> AvailableFontsNames
		{
			get { return AvailableFonts?.Select(font => font.Name); }
		}

		internal void AddSymbolsFonts(IEnumerable<TrueTypeFont> newFonts)
		{
			Debug.Assert(newFonts != null, "New fonts collection should not be null.");

			AvailableFonts = AvailableFonts != null ? newFonts.Concat(AvailableFonts) : newFonts;

			if (SymbolsFont == null)
			{
				SymbolsFont = AvailableFonts.FirstOrDefault();
			}
		}

		#endregion

		#region Symbols options

		public bool IncludeNumbers
		{
			get { return includeNumbers; }
			set
			{
				if (value == includeNumbers)
				{
					return;
				}

				includeNumbers = value;
				using (SuspendCallManipulations())
				{
					RebuildAvailableSymbols();
					CallManipulations();
					OnPropertyChanged(nameof(IncludeNumbers));
				}
			}
		}
		bool includeNumbers;

		public bool IncludeLetters
		{
			get { return includeLetters; }
			set
			{
				if (value == includeLetters)
				{
					return;
				}

				includeLetters = value;
				using (SuspendCallManipulations())
				{
					RebuildAvailableSymbols();
					CallManipulations();
					OnPropertyChanged(nameof(IncludeLetters));
				}
			}
		}
		bool includeLetters;

		public bool IncludeSymbols
		{
			get { return includeSymbols; }
			set
			{
				if (value == includeSymbols)
				{
					return;
				}

				includeSymbols = value;
				using (SuspendCallManipulations())
				{
					RebuildAvailableSymbols();
					CallManipulations();
					OnPropertyChanged(nameof(IncludeSymbols));
				}
			}
		}
		bool includeSymbols;

		public int MaxSelectedSymbols { get; set; }

		#endregion

		#region Symbols

		public List<KeyValuePair<char, bool>> AvailableSymbols
		{
			get
			{
				if (availableSymbols == null)
				{
					RebuildAvailableSymbols();
				}
				return availableSymbols;
			}
		}
		List<KeyValuePair<char, bool>> availableSymbols;

		public IEnumerable<char> SelectedSymbols => AvailableSymbols.Where(kv => kv.Value).Select(kv => kv.Key);

		public int SelectedCount => SelectedSymbols.Count();

		public void SwitchSelection(char c, bool auto = false)
		{
			if (MaxSelectedSymbols > 0)
			{
				if (MaxSelectedSymbols == 1)
				{
					ClearAllSelection();
				}
				else if (SelectedCount >= MaxSelectedSymbols)
				{
					return;
				}
			}

			for (int i = 0; i < AvailableSymbols.Count; i++)
			{
				var kv = AvailableSymbols[i];
				if (kv.Key == c)
				{
					AvailableSymbols[i] = new KeyValuePair<char, bool>(c, !kv.Value);
					userSelected |= !auto;
					CallManipulations();
					break;
				}
			}
		}

		bool userSelected;

		void RebuildAvailableSymbols()
		{
			var oldSelectedSymbols = availableSymbols != null ? SelectedSymbols.ToArray() : new char[0];

			availableSymbols = new List<KeyValuePair<char, bool>>();
			availableSymbols.Add(new KeyValuePair<char, bool>(' ', oldSelectedSymbols.Length == 0 || oldSelectedSymbols.Contains(' ')));
			if (SymbolsFont != null)
			{
				foreach (var symbol in SymbolsFont.GetNonEmptyCodepoints())
				{
					if (IncludeSymbols && !IsDigit(symbol) && !IsLetter(symbol) && symbol != ' ' ||
						IncludeNumbers && IsDigit(symbol) ||
						IncludeLetters && IsLetter(symbol))
					{
						availableSymbols.Add(new KeyValuePair<char, bool>(symbol, oldSelectedSymbols.Contains(symbol)));
					}
				}

				AddRandomSymbols();
			}
		}

		public static bool IsDigit(char symbol)
		{
			return symbol >= '0' && symbol <= '9';
		}

		public static bool IsLetter(char symbol)
		{
			return symbol >= 'A' && symbol <= 'Z' || symbol >= 'a' && symbol <= 'z' || symbol >= 'А' && symbol <= 'Я' || symbol >= 'а' && symbol <= 'я';
		}

		public void AddRandomSymbols()
		{
			if (SelectedCount >= Manipulator.ManipulatedImage.Palette.Count)
			{
				return;
			}

			var random = new Random();
			for (int i = SelectedCount; i < Manipulator.ManipulatedImage.Palette.Count; i++)
			{
				var nextIndex = random.Next(AvailableSymbols.Count);
				var newSymbol = GetFirstUnselectedSymbol(nextIndex, AvailableSymbols.Count);
				if (newSymbol == 0)
				{
					GetFirstUnselectedSymbol(0, nextIndex);
				}
				if (newSymbol == 0)
				{
					break;
				}
			}

			userSelected = false;
			CallManipulations();
		}

		char GetFirstUnselectedSymbol(int from, int to)
		{
			for (int i = from; i < to; i++)
			{
				var kv = AvailableSymbols[i];
				if (!kv.Value)
				{
					AvailableSymbols[i] = new KeyValuePair<char, bool>(kv.Key, true);
					return kv.Key;
				}
			}
			return (char)0;
		}

		public void ClearAllSelection()
		{
			for (int i = 0; i < AvailableSymbols.Count; i++)
			{
				var kv = AvailableSymbols[i];
				if (kv.Value)
				{
					AvailableSymbols[i] = new KeyValuePair<char, bool>(kv.Key, false);
				}
			}
			userSelected = false;
			CallManipulations();
		}

		#endregion

		protected override void CallManipulationsCore()
		{
			Manipulator.ApplySymbols(SelectedSymbols, symbolsFont);
		}

		void Manipulator_SourceImageChanged(object sender, EventArgs e)
		{
			if (!userSelected && SelectedCount > Manipulator.ManipulatedImage.Palette.Count)
			{
				ClearAllSelection();
			}
			AddRandomSymbols();
		}

		#region Defaults

		protected override void RestoreDefaultsCore()
		{
			using (SuspendCallManipulations())
			{
				SymbolsFontName = SAEWizardSettings.Default.SymbolsFontName;
				IncludeNumbers = SAEWizardSettings.Default.SymbolsFontIncludeNumbers;
				IncludeLetters = SAEWizardSettings.Default.SymbolsFontIncludeLetters;
				IncludeSymbols = SAEWizardSettings.Default.SymbolsFontIncludeSymbols;

				var lastUsedSymbols = SAEWizardSettings.Default.SymbolsFontLastUsedSymbols;
				if (!string.IsNullOrEmpty(lastUsedSymbols))
				{
					ClearAllSelection();
					foreach (var c in lastUsedSymbols)
					{
						SwitchSelection(c);
					}
					userSelected = false;
				}
			}
		}

		protected override void SaveDefaultsCore()
		{
			SAEWizardSettings.Default.SymbolsFontName = SymbolsFontName;
			SAEWizardSettings.Default.SymbolsFontIncludeNumbers = IncludeNumbers;
			SAEWizardSettings.Default.SymbolsFontIncludeLetters = IncludeLetters;
			SAEWizardSettings.Default.SymbolsFontIncludeSymbols = IncludeSymbols;
			SAEWizardSettings.Default.SymbolsFontLastUsedSymbols = userSelected ? new string(SelectedSymbols.ToArray()) : string.Empty;
		}

		protected override void RestoreImageSettingsCore(CodedImage image)
		{
			base.RestoreImageSettingsCore(image);

			using (SuspendCallManipulations())
			{
				SymbolsFontName = image.Palette.SymbolsFont;
				var symbolChars = image.Palette.Cast<CodedColor>().Select(color => color.SymbolChar).Distinct().ToList();
				IncludeSymbols = true;
				IncludeNumbers |= symbolChars.Any(IsDigit);
				IncludeLetters |= symbolChars.Any(IsLetter);

				ClearAllSelection();
				foreach (var symbol in symbolChars)
				{
					SwitchSelection(symbol, true);
				}
				AddRandomSymbols();
				userSelected = false;
			}
		}

		#endregion
	}
}
