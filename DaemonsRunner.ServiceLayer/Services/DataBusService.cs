using DaemonsRunner.BuisnessLayer.Services.Interfaces;

namespace DaemonsRunner.BuisnessLayer.Services;

public class DataBusService : IDataBus
{
	private readonly Dictionary<Type, IEnumerable<WeakReference>> _subscriptions = new();
	private readonly ReaderWriterLockSlim _locker = new();
	private class Subscription<T> : IDisposable
	{
		private readonly WeakReference<DataBusService> _bus;
		public Action<T> Handler { get; }

		public Subscription(DataBusService bus, Action<T> handler)
		{
			_bus = new(bus);
			Handler = handler;
		}

		public void Dispose()
		{
			if (!_bus.TryGetTarget(out var bus))
				return;

			var Lock = bus._locker;
			Lock.EnterWriteLock();
			var messageType = typeof(T);
			try
			{
				if (!bus._subscriptions.TryGetValue(messageType, out var refs))
					return;

				var updatedRefs = refs.Where(r => r.IsAlive).ToList();

				WeakReference? currentReference = null;
				foreach (var item in updatedRefs)
				{
					if (ReferenceEquals(item.Target, this))
					{
						currentReference = item;
						break;
					}
				}
				if (currentReference == null)
					return;

				updatedRefs.Remove(currentReference);
				bus._subscriptions[messageType] = updatedRefs;

			}
			finally
			{
				Lock.ExitWriteLock();
			}
		}
	}

	public IDisposable RegisterHandler<T>(Action<T> handler)
	{
		var subscription = new Subscription<T>(this, handler);

		_locker.EnterWriteLock();
		try
		{
			var weakRef = new WeakReference(subscription);
			var messageType = typeof(T);

			_subscriptions[messageType] = _subscriptions.TryGetValue(messageType, out var subscriptions)
				? subscriptions.Append(weakRef) : new[] { weakRef };
		}
		finally
		{
			_locker.ExitWriteLock();
		}

		return subscription;
	}

	public void Send<T>(T message)
	{
		if (GetHandlers<T>() is not { } handlers)
			return;

		foreach (var item in handlers)
		{
			item(message);
		}
	}

	private IEnumerable<Action<T>>? GetHandlers<T>()
	{
		var handlers = new List<Action<T>>();
		var messageType = typeof(T);
		bool isDiedRefsExist = false;

		_locker.EnterReadLock();
		try
		{
			if (!_subscriptions.TryGetValue(messageType, out var refs))
				return null;

			foreach (var item in refs)
			{
				if (item.Target is Subscription<T> { Handler: var handler })
					handlers.Add(handler);
				else
					isDiedRefsExist = true;
			}
		}
		finally
		{
			_locker.ExitReadLock();
		}

		if (!isDiedRefsExist) return handlers;

		// clean up the died references
		_locker.EnterWriteLock();
		try
		{
			if (!_subscriptions.TryGetValue(messageType, out var refs))
				if (refs is not null && refs.Where(r => r.IsAlive).ToArray() is { Length: > 0 } newRefs)
					_subscriptions[messageType] = newRefs;
				else
					_subscriptions.Remove(messageType);
		}
		finally
		{
			_locker.ExitWriteLock();
		}

		return handlers;
	}
}
