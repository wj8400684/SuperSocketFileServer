using SuperSocket.Command;
using supersocketIocpServer;

namespace SuperSocketFileServer;

[Command(Key = (byte)FileCommandKey.Upload)]
public sealed class UploadCommand : FileAsyncCommand<UpLoadPackageInfo, UpLoadAckPackageInfo>
{
    protected override async ValueTask<UpLoadAckPackageInfo> ExecuteAsync(FileAppSession session, UpLoadPackageInfo package)
    {
        session.LogInformation($"开始传输文件：[{package.FileName}] 长度：{package.FileLength}");

        await session.CreateFileAsync(package.FileName, package.RelativeDirectory);

        return new UpLoadAckPackageInfo { SuccessFul = true };
    }
}
