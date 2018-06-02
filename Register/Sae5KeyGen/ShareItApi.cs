/** 
	C# key generator example

	(c) 2004-2006 element 5 
	(c) 2006 Digital River GmbH

	written by Stefan Weber

	SDK 3 File Revision 3
*/

// define UNICODE_GEN to support UTF8 encoded in-/output files
#define UNICODE_GEN

using System;
using System.IO;
using System.Text;
using System.Collections;
using Ravlyk.Drawing.Tif;

namespace Sae5KeyGen
{
	/// <summary>
	/// This class implements a key generation link.
	/// </summary>
	class ShareItApi
	{
		// possible key generator exit codes - do not modify
		public enum KeyGenReturnCode
		{
			// success
			ERC_SUCCESS = 00,
			ERC_SUCCESS_BIN = 01,
			// failure
			ERC_ERROR = 10,
			ERC_MEMORY = 11,
			ERC_FILE_IO = 12,
			ERC_BAD_ARGS = 13,
			ERC_BAD_INPUT = 14,
			ERC_EXPIRED = 15,
			ERC_INTERNAL = 16
		}

		// key generator exception class
		public class KeyGenException : Exception
		{
			public KeyGenReturnCode ERC;

			public KeyGenException(string message, KeyGenReturnCode e) : base(message)
			{
				ERC = e;
			}
		}

		// input/output file encoding
#if UNICODE_GEN
		static readonly Encoding fileEncoding = new UTF8Encoding();
#else
		static readonly Encoding fileEncoding = Encoding.GetEncoding("ISO-8859-1");
#endif

		// list of input values
		static readonly SortedList Inputs = new SortedList();

		// generated key data
		static string KeyResult1; // the key for the user
		static string KeyResult2; // the cckey for the publisher

		// get input string values, return empty string if not defined
		public static string GetValue(string key)
		{
			return Inputs.ContainsKey(key) ? Inputs[key].ToString() : "";
		}

		// a simple example algorithm using MD5 message digests
		public static void GenerateKey()
		{
			// get user name
			var userName = GetValue("REG_NAME");

			// result 1 - key for the customer
			KeyResult1 = TifEncoder.GetKey(userName);

			// result 2 - cckey for the publisher
			KeyResult2 = GetValue("REG_NAME") + " " + KeyResult1;
		}

		// split a string at the first equals sign and add key/value to Inputs[]
		public static void AddInputLine(string line)
		{
			var posEqual = line.IndexOf('=');
			if (posEqual > 0)
			{
				var aKey = line.Remove(posEqual, line.Length - posEqual);
				var aValue = line.Substring(posEqual + 1);

				if (aValue.Length > 0)
				{
					Inputs.Add(aKey, aValue);
				}
			}
		}

		// read the input file and parse its lines into the Inputs[] list
		public static void ReadInput(string pathname)
		{
			Inputs.Clear();

			// attempt to open the input file for read-only access
			using (var fsIn = new FileStream(pathname, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (var sr = new StreamReader(fsIn, fileEncoding, true))
			{
				// process every line in the file
				for (var line = sr.ReadLine(); line != null; line = sr.ReadLine())
				{
					AddInputLine(line.Trim());
				}
				// explicitly close the StreamReader to properly flush all buffers
				sr.Close(); // this also closes the FileStream (fsIn)
			}

			// check the input encoding
			var encodingName = GetValue("ENCODING");
#if UNICODE_GEN
			if (encodingName != "UTF8")
			{
				throw new KeyGenException("bad input encoding, expected UTF-8", KeyGenReturnCode.ERC_BAD_INPUT);
			}
#else
			if ((encodingName != "") && (encodingName != "ISO-8859-1"))
			{
				throw new KeyGenException("bad input encoding, expected ISO-8859-1", KeyGenReturnCode.ERC_BAD_INPUT);
			};
#endif

			// check for valid input
			var userName = GetValue("REG_NAME");
			if (userName.Length < 8)
			{
				throw new KeyGenException("REG_NAME must have at least 8 characters", KeyGenReturnCode.ERC_BAD_INPUT);
			}
		}

		// write a string to an output file using the encoding specified in the input file
		public static void WriteOutput(string pathname, string data)
		{
			// Create an instance of StreamWriter to write text to a file.
			// The using statement also closes the StreamWriter.
			using (var fsOut = new FileStream(pathname, FileMode.Create))
			using (var sw = new StreamWriter(fsOut, fileEncoding))
			{
				sw.Write(data);
			}
		}

		public static void Process(string[] args)
		{
			Console.WriteLine("SAE5 ShareIt key generator");

			try
			{
				if (args.Length == 3)
				{
					Console.Write("> reading input file: ");
					Console.WriteLine(args[0]);
					ReadInput(args[0]);

					Console.WriteLine("> processing ... ");
					GenerateKey();

					Console.WriteLine("> writing output files: ");

					WriteOutput(args[1], KeyResult1);
					WriteOutput(args[2], KeyResult2);
					Environment.ExitCode = (int) KeyGenReturnCode.ERC_SUCCESS;
				}
				else
				{
					Console.WriteLine("Usage: <input> <output1> <output2>");
					Environment.ExitCode = (int)KeyGenReturnCode.ERC_BAD_ARGS;
				}
			}
			catch (KeyGenException e)
			{
				Console.WriteLine("* KeyGen Exception: " + e.Message);

				// set the exit code to the ERC of the exception object
				Environment.ExitCode = (int)e.ERC;

				// and write the error message to output file #1
				try
				{
					WriteOutput(args[1], e.Message);
				}
				catch { }
			}
			catch (Exception e)
			{
				// for general exceptions return ERC_ERROR
				Environment.ExitCode = (int)KeyGenReturnCode.ERC_ERROR;
				Console.WriteLine("* CLR Exception: " + e.Message);

				// and write the error message to output file #1
				try
				{
					WriteOutput(args[1], e.Message);
				}
				catch { }
			}

			Console.WriteLine("ExitCode: {0}", Environment.ExitCode);
		}
	}
}