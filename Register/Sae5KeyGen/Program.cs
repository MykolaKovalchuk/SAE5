using System;
using Ravlyk.Drawing.Tif;

namespace Sae5KeyGen
{
	class Program
	{
		static void Main(string[] args)
		{
#if DEBUG
			if (args.Length <= 1)
			{
				Console.Write("User ID: ");
				var user = Console.ReadLine();
				var key = TifEncoder.GetKey(user);
				Console.WriteLine("Reg Key: " + key);
			}
			else
#endif
			{
				ShareItApi.Process(args);
			}
		}
	}
}
