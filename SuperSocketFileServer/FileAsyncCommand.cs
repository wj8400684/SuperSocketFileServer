using SuperSocket.Command;
using supersocketIocpServer;

namespace SuperSocketFileServer;

public abstract class FileAsyncCommand<TPackageInfo> : IAsyncCommand<FileAppSession, FilePackageInfo>
    where TPackageInfo : FilePackageInfo
{
    async ValueTask IAsyncCommand<FileAppSession, FilePackageInfo>.ExecuteAsync(FileAppSession session, FilePackageInfo package)
    {
        if (session.IsClosed())
            return;

        try
        {
            await ExecuteAsync(session, (TPackageInfo)package);
        }
        catch (Exception)
        {
            throw;
        }
    }

    protected abstract ValueTask ExecuteAsync(FileAppSession session, TPackageInfo package);
}

public abstract class FileAsyncCommand<TPackageInfo, TAckPackageInfo> : IAsyncCommand<FileAppSession, FilePackageInfo>
    where TPackageInfo : FilePackageInfo, new()
    where TAckPackageInfo : FileAckPackageInfo, new()
{
    async ValueTask IAsyncCommand<FileAppSession, FilePackageInfo>.ExecuteAsync(FileAppSession session, FilePackageInfo package)
    {
        if (session.IsClosed())
            return;

        TAckPackageInfo? ackPackage = null;

        try
        {
            ackPackage = await ExecuteAsync(session, (TPackageInfo)package);
        }
        catch (ArgumentNullException)
        {
            ackPackage = new() { SuccessFul = false, ErrorMessage = "参数错误" };
            throw;
        }
        catch (Exception)
        {
            ackPackage = new() { SuccessFul = false, ErrorMessage = "未知错误" };
            throw;
        }
        finally
        {
            if (ackPackage != null)
                await session.SendPackageAsync(ackPackage);
        }
    }

    protected abstract ValueTask<TAckPackageInfo> ExecuteAsync(FileAppSession session, TPackageInfo package);
}
