using SuperSocket.Command;
using SuperSocketFileServer;

namespace supersocketIocpServer;

[Command(Key = (byte)CommandKey.Data)]
public sealed class Data : FileAsyncCommand<DataPackageInfo, DataAckPackageInfo>
{
    protected override async ValueTask<DataAckPackageInfo> ExecuteAsync(MyAppSession session, DataPackageInfo package)
    {
        ArgumentNullException.ThrowIfNull(package.Body);

        await session.SaveAsync(package.Body);

        return new DataAckPackageInfo { SuccessFul = true };
    }
}