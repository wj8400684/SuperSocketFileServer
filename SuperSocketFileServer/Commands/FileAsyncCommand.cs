using SuperSocket.Command;
using supersocketIocpServer;

namespace SuperSocketFileServer;

public abstract class FileAsyncCommand<TPackageInfo> : IAsyncCommand<MyAppSession, FilePackageInfo>
    where TPackageInfo : FilePackageInfo
{
    async ValueTask IAsyncCommand<MyAppSession, FilePackageInfo>.ExecuteAsync(MyAppSession session, FilePackageInfo package)
    {
        try
        {
            await ExecuteAsync(session, (TPackageInfo)package);
        }
        catch (Exception)
        {
            throw;
        }
    }

    protected abstract ValueTask ExecuteAsync(MyAppSession session, TPackageInfo package);
}

public abstract class FileAsyncCommand<TPackageInfo, TAckPackageInfo> : IAsyncCommand<MyAppSession, FilePackageInfo>
    where TPackageInfo : FilePackageInfo
    where TAckPackageInfo : FileAckPackageInfo
{
    async ValueTask IAsyncCommand<MyAppSession, FilePackageInfo>.ExecuteAsync(MyAppSession session, FilePackageInfo package)
    {
        TAckPackageInfo ackPackage;

        try
        {
            ackPackage = await ExecuteAsync(session, (TPackageInfo)package);
        }
        catch (Exception)
        {
            throw;
        }

        await session.SendPackageAsync(ackPackage);
    }

    protected abstract ValueTask<TAckPackageInfo> ExecuteAsync(MyAppSession session, TPackageInfo package);
}
