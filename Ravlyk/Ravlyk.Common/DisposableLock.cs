using System;

namespace Ravlyk.Common
{
	/// <summary>
	/// Temporary-state lock flag.
	/// </summary>
	public class DisposableLock
	{
		/// <summary>
		/// Private constructor. This class can be instantiated only by method Lock.
		/// </summary>
		DisposableLock() { }

		/// <summary>
		/// Lock counter.
		/// </summary>
		int Counter { get; set; }

		/// <summary>
		/// Increments lock counter on a lock flag and returns <see cref="IDisposable"/> object.
		/// </summary>
		/// <param name="lockHolder">Reference to a field of type <see cref="DisposableLock"/> containing a flag to lock. If it is null, it will be initialized with new instance of DisposableLock.</param>
		/// <param name="unlockAction">Optional action, that will be called when returned <see cref="IDisposable"/> object is disposed, and lock is unlocked. This action will not be called if lock is still locked.</param>
		/// <returns>Instance of <see cref="IDisposable"/>, disposing of which will decrease lock counter on <see cref="lockHolder"/> and call <see cref="unlockAction"/> if lock is unlocked.</returns>
		public static IDisposable Lock(ref DisposableLock lockHolder, Action unlockAction = null)
		{
			if (lockHolder == null)
			{
				lockHolder = new DisposableLock();
			}

			lockHolder.Counter++;

			var lockHolderLocalReference = lockHolder; // ref variables cannot be passed to anonymous methods.
			return new DisposableAction(() =>
			{
				if (--lockHolderLocalReference.Counter == 0)
				{
					unlockAction?.Invoke();
				}
			});
		}

		/// <summary>
		/// Checks if lock holder is locked.
		/// </summary>
		/// <param name="lockHolder"><see cref="DisposableLock"/> object, can be null.</param>
		/// <returns>True if <see cref="lockHolder"/> is not null and <see cref="Counter"/> is more then 0. Otherwise returns False.</returns>
		internal static bool IsLockedInternal(DisposableLock lockHolder)
		{
			return lockHolder?.Counter > 0;
		}
	}

	/// <summary>
	/// Extension methods for <see cref="DisposableLock"/>.
	/// </summary>
	public static class DisposableLockExtensions
	{
		/// <summary>
		/// Checks if lock holder is locked.
		/// </summary>
		/// <param name="disposableLock"><see cref="DisposableLock"/> object, can be null.</param>
		/// <returns>True if <see cref="disposableLock"/> is not null and <see cref="DisposableLock.Counter"/> is more then 0. Otherwise returns False.</returns>
		public static bool IsLocked(this DisposableLock disposableLock)
		{
			return DisposableLock.IsLockedInternal(disposableLock);
		}
	}
}

