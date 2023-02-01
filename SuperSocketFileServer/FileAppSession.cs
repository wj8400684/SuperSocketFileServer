using SuperSocket.Channel;
using SuperSocket.ProtoBase;
using SuperSocket.Server;
using SuperSocketFileServer;

namespace supersocketIocpServer;

public sealed class FileAppSession : AppSession
{
    private static readonly string FileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Files");

    private readonly IPackageEncoder<FilePackageInfo> _packageEncoder;

    private readonly AsyncLock _lock = new();

    private FileStream? _fileStream;

    public FileAppSession(IPackageEncoder<FilePackageInfo> encoder)
    {
        _packageEncoder = encoder;
    }

    internal bool IsClosed()
    {
        return Channel.IsClosed;
    }

    internal ValueTask SendPackageAsync(FilePackageInfo packageInfo)
    {
        if (IsClosed())
            return ValueTask.CompletedTask;

        return Channel.SendAsync(_packageEncoder, packageInfo);
    }

    protected override ValueTask OnSessionClosedAsync(CloseEventArgs e)
    {
        var fileStream = _fileStream;

        if (fileStream == null)
            return base.OnSessionClosedAsync(e);

        fileStream.Close();
        fileStream.Dispose();
        _lock.Dispose();
        _fileStream = null;

        Logger.LogInformation("销毁文件流");
        return base.OnSessionClosedAsync(e);
    }

    internal async ValueTask CreateFileAsync(string fileName, string? relativePath)
    {
        using var _ = await _lock.EnterAsync();

        var fileStream = _fileStream;

        if (fileStream != null)
            throw new Exception();

        //拼接完整目录
        var fullDirector = Path.Combine(FileDirectory, relativePath ?? string.Empty);

        //不存在目录则创建
        if (!Directory.Exists(fullDirector))
            Directory.CreateDirectory(fullDirector);

        //拼接完整路径
        var filePath = Path.Combine(fullDirector, fileName);

        if (File.Exists(filePath))
            _fileStream = new(filePath, FileMode.Open, FileAccess.ReadWrite);
        else
            _fileStream = new(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

        _fileStream.Position = _fileStream.Length;
    }

    internal async ValueTask SaveAsync(List<ArraySegment<byte>> body)
    {
        if (Channel.IsClosed)
            return;

        var fileStream = _fileStream;

        ArgumentNullException.ThrowIfNull(fileStream);

        foreach (var item in body)
            await fileStream.WriteAsync(item);
    }

    internal async ValueTask CloseFileAsync()
    {
        using var _ = await _lock.EnterAsync();

        var fileStream = _fileStream;

        ArgumentNullException.ThrowIfNull(fileStream);

        await fileStream.FlushAsync();

        fileStream.Close();
        fileStream.Dispose();
        _fileStream = null;
    }
}