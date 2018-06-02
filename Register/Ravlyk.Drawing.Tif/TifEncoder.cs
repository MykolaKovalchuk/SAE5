using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ravlyk.Drawing.Tif
{
	public static class TifEncoder
	{
		public static string GetKey(string userName)
		{
			const int HalfInt = int.MaxValue / 2;

			var code = Math.Abs(GetStringHashCode(userName.ToLowerInvariant()) ^ GetStringHashCode("Stitch Art Easy! 5"));
			var code2 = int.MaxValue - code;
			if (code < HalfInt) { code += HalfInt; }
			if (code % 10 == 0 && code < int.MaxValue) { code++; }
			if (code2 < HalfInt) { code2 += HalfInt; }
			if (code2 % 10 == 0 && code2 < int.MaxValue) { code2++; }
			var bigCode = (long)code * (long)code2;

			var chars = bigCode.ToString("X").ToUpperInvariant();
			var sb = new StringBuilder(chars.Length + 5);
			var counter = 0;
			var checkSum = 0;
			foreach (var c in chars.Take(15))
			{
				if (sb.Length > 0 && counter % 4 == 0)
				{
					sb.Append('-');
				}
				sb.Append(c);
				checkSum += c - (char.IsDigit(c) ? '0' : 'A');
				counter++;
			}
			while (counter < 16)
			{
				var c = (15 - checkSum % 16).ToString("X")[0];
				sb.Append(c);
				checkSum += c - (char.IsDigit(c) ? '0' : 'A');
				counter++;
			}

			return sb.ToString();
		}

		/// <summary>
		/// Calculates hash code of string using 32-bit implementation.
		/// </summary>
		/// <param name="str">String to calculate hash code.</param>
		/// <returns>Integer value representing string's hash code.</returns>
		static int GetStringHashCode(string str)
		{
			unsafe
			{
				fixed (char* src = str)
				{
					int hash1 = (5381 << 16) + 5381;
					int hash2 = hash1;

					int* pint = (int*)src;
					int len = str.Length;
					while (len > 2)
					{
						hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
						hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];
						pint += 2;
						len -= 4;
					}

					if (len > 0)
					{
						hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
					}
					return hash1 + (hash2 * 1566083941);
				}
			}
		}

		public static RegoStatus IsKeyCorrect(string userName, string key)
		{
			var key1 = GetKey(userName).Trim().Replace("-", "");
			var key2 = key.Trim().Replace("-", "");
			var isCorrect = key1.Equals(key2, StringComparison.OrdinalIgnoreCase);
			var status = isCorrect
				? CancelledRefundedRegoPairs.Any(p => p.Item1.Equals(userName, StringComparison.CurrentCultureIgnoreCase) && p.Item2.Equals(key, StringComparison.OrdinalIgnoreCase))
					? RegoStatus.CancelledRefunded
					: RegoStatus.Ok
				: RegoStatus.UnmatchingKey;
			return status;
		}

		public enum RegoStatus
		{
			Ok,
			UnmatchingKey,
			CancelledRefunded
		}

		static IEnumerable<Tuple<string, string>> CancelledRefundedRegoPairs => new List<Tuple<string, string>>
		{
			new Tuple<string, string>("ИРЕНА ГЕОРГИЕВА", "234E-E4B4-39F3-1DBC"),
			new Tuple<string, string>("Delores Vollmer", "22D3-FD68-7E57-3F5B"),
			new Tuple<string, string>("sabrina roma", "23FA-7B67-58AE-4380")
		};
	}
}
