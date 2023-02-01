using SuperSocket.Command;
using SuperSocketFileServer;

namespace supersocketIocpServer;

[Command(Key = (byte)FileCommandKey.Data)]
public sealed class DataCommand : FileAsyncCommand<DataPackageInfo>
{
    protected override async ValueTask ExecuteAsync(FileAppSession session, DataPackageInfo package)
    {
        ArgumentNullException.ThrowIfNull(package.Body);

        await session.SaveAsync(package.Body);
    }
}