using Microsoft.Extensions.Logging;
using SuperSocket.Command;
using supersocketIocpServer;

namespace SuperSocketFileServer;

[Command(Key = (byte)CommandKey.Upload)]
public sealed class Upload : FileAsyncCommand<UpLoadPackageInfo>
{
    protected override ValueTask ExecuteAsync(MyAppSession session, UpLoadPackageInfo package)
    {
        ArgumentNullException.ThrowIfNull(package.FileName);

        session.LogInformation($"开始传输文件：[{package.FileName}] 长度：{package.FileSize}");

        session.CreateFile(package.FileName);

        return ValueTask.CompletedTask;
    }
}
