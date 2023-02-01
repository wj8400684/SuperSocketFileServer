using SuperSocket.Command;
using SuperSocketFileServer;

namespace supersocketIocpServer;

[Command(Key = (byte)FileCommandKey.Data)]
public sealed class DataCommand : FileAsyncCommand<DataPackageInfo, DataAckPackageInfo>
{
    protected override async ValueTask<DataAckPackageInfo> ExecuteAsync(FileAppSession session, DataPackageInfo package)
    {
        ArgumentNullException.ThrowIfNull(package.Body);

        await session.SaveAsync(package.Body);

        return new DataAckPackageInfo { SuccessFul = true };
    }
}