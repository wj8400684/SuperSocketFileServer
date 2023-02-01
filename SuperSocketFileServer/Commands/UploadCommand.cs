using Microsoft.Extensions.Logging;
using SuperSocket.Command;
using supersocketIocpServer;

namespace SuperSocketFileServer;

[Command(Key = (byte)FileCommandKey.Upload)]
public sealed class UploadCommand : FileAsyncCommand<UpLoadPackageInfo>
{
    protected override ValueTask ExecuteAsync(FileAppSession session, UpLoadPackageInfo package)
    {
        ArgumentNullException.ThrowIfNull(package.FileName);

        session.LogInformation($"开始传输文件：[{package.FileName}] 长度：{package.FileSize}");

        session.CreateFile(package.FileName);

        return ValueTask.CompletedTask;
    }
}
