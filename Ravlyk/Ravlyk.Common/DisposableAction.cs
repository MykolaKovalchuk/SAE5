using System;
using System.Diagnostics;

namespace Ravlyk.Common
{
	/// <summary>
	/// Temporary-state action - executes <see cref="disposed"/> call-back method on disposing.
	/// </summary>
	public class DisposableAction : IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DisposableAction"/> class. 
		/// </summary>
		/// <param name="disposed">Call-back method.</param>
		public DisposableAction(Action disposed)
		{
			Debug.Assert(disposed != null, "Dispose action should not be null.");

			this.disposed = disposed;
		}

		readonly Action disposed;

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			disposed();
		}
	}
}
