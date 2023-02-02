using SuperSocket.Command;
using supersocketIocpServer;

namespace SuperSocketFileServer;

[Command(Key = (byte)FileCommandKey.Upload)]
public sealed class UploadCommand : FileAsyncCommand<UpLoadPackageInfo, UpLoadAckPackageInfo>
{
    protected override async ValueTask<UpLoadAckPackageInfo> ExecuteAsync(FileAppSession session, UpLoadPackageInfo package)
    {
        session.LogInformation($"开始传输文件：[{package.FileName}] 大小：{package.FileLength} b");

        await session.CreateFileAsync(package.FileName, package.RelativeDirectory);

        return new UpLoadAckPackageInfo { SuccessFul = true };
    }
}
