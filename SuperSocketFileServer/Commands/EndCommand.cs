using Microsoft.Extensions.Logging;
using SuperSocket.Command;
using supersocketIocpServer;

namespace SuperSocketFileServer;

[Command(Key = (byte)FileCommandKey.End)]
public sealed class EndCommand : FileAsyncCommand<EndPackageInfo, EndAckPackageInfo>
{
    protected override async ValueTask<EndAckPackageInfo> ExecuteAsync(FileAppSession session, EndPackageInfo package)
    {
        session.LogInformation($"文件传输结束");

        await session.CloseFileAsync();

        return new EndAckPackageInfo { SuccessFul = true };
    }
}
