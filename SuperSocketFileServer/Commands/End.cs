using Microsoft.Extensions.Logging;
using SuperSocket.Command;
using supersocketIocpServer;

namespace SuperSocketFileServer;

[Command(Key = (byte)CommandKey.End)]
public sealed class End : FileAsyncCommand<EndPackageInfo, EndAckPackageInfo>
{
    protected override ValueTask<EndAckPackageInfo> ExecuteAsync(MyAppSession session, EndPackageInfo package)
    {
        session.LogInformation($"文件传输结束");

        session.CloseFile();

        return ValueTask.FromResult(new EndAckPackageInfo { SuccessFul = true });
    }
}
