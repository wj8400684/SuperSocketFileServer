namespace SuperSocketFileServer;

public sealed class AsyncLock : IDisposable
{
    private readonly Task<IDisposable> _completedTask;
    private readonly IDisposable _releaser;
    private readonly object _syncRoot = new();
    private readonly Queue<AsyncLockWaiter> _waiters = new(64);

    private volatile bool _isDisposed;
    private bool _isLocked;

    public AsyncLock()
    {
        _releaser = new Releaser(this);
        _completedTask = Task.FromResult(_releaser);
    }

    public void Dispose()
    {
        lock (_syncRoot)
        {
            _isDisposed = true;

            while (_waiters.Any())
            {
                _waiters.Dequeue().Dispose();
            }
        }
    }

    public Task<IDisposable> EnterAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_isDisposed)
            throw new ObjectDisposedException(nameof(AsyncLock));

        lock (_syncRoot)
        {
            if (!_isLocked)
            {
                _isLocked = true;
                return _completedTask;
            }

            var waiter = new AsyncLockWaiter(cancellationToken);
            _waiters.Enqueue(waiter);

            return waiter.Task;
        }
    }

    void Release()
    {
        lock (_syncRoot)
        {
            if (_isDisposed)
            {
                // All waiters have been canceled with a ObjectDisposedException.
                // So there is nothing left to do.
                return;
            }

            // Assume that there is no waiter left first.
            _isLocked = false;

            // Try to find the next waiter which can be approved.
            // Some of them might be canceled already so it is not
            // guaranteed that the very next waiter is the correct one.
            while (_waiters.Any())
            {
                var waiter = _waiters.Dequeue();
                var isApproved = waiter.Approve(_releaser);
                waiter.Dispose();

                if (isApproved)
                {
                    _isLocked = true;
                    return;
                }
            }
        }
    }

    sealed class AsyncLockWaiter : IDisposable
    {
        private readonly CancellationTokenRegistration _cancellationRegistration;
        private readonly bool _hasCancellationRegistration;
        private readonly AsyncTaskCompletionSource<IDisposable> _promise = new ();

        public AsyncLockWaiter(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (cancellationToken.CanBeCanceled)
            {
                _cancellationRegistration = cancellationToken.Register(Cancel);
                _hasCancellationRegistration = true;
            }
        }

        public Task<IDisposable> Task => _promise.Task;

        public bool Approve(IDisposable scope)
        {
            ArgumentNullException.ThrowIfNull(scope);

            if (_promise.Task.IsCompleted)
                return false;

            return _promise.TrySetResult(scope);
        }

        public void Dispose()
        {
            if (_hasCancellationRegistration)
                _cancellationRegistration.Dispose();

            _promise.TrySetException(new ObjectDisposedException(nameof(AsyncLockWaiter)));
        }

        void Cancel()
        {
            _promise.TrySetCanceled();
        }
    }

    readonly struct Releaser : IDisposable
    {
        private readonly AsyncLock _asyncLock;

        public Releaser(AsyncLock asyncLock)
        {
            _asyncLock = asyncLock ?? throw new ArgumentNullException(nameof(asyncLock));
        }

        public void Dispose()
        {
            _asyncLock.Release();
        }
    }
}
