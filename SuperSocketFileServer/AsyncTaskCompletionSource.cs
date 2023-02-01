namespace SuperSocketFileServer;

public sealed class AsyncTaskCompletionSource<TResult>
{
    readonly TaskCompletionSource<TResult> _taskCompletionSource;

    public AsyncTaskCompletionSource()
    {
        _taskCompletionSource = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    public Task<TResult> Task => _taskCompletionSource.Task;

    public void TrySetCanceled()
    {
        _taskCompletionSource.TrySetCanceled();
    }

    public void TrySetException(Exception exception)
    {
        _taskCompletionSource.TrySetException(exception);
    }

    public bool TrySetResult(TResult result)
    {
        return _taskCompletionSource.TrySetResult(result);
    }
}
