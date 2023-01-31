using SuperSocket.Channel;
using SuperSocket.ProtoBase;
using SuperSocket.Server;
using SuperSocketFileServer;
using System.IO;

namespace supersocketIocpServer;

public sealed class MyAppSession : AppSession
{
    private static readonly string FileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Files");

    private readonly IPackageEncoder<FilePackageInfo> _packageEncoder;

    internal object _fileRoot = new();

    internal FileStream? FileStream { get; set; }

    public MyAppSession(IPackageEncoder<FilePackageInfo> encoder)
    {
        _packageEncoder = encoder;
    }

    public ValueTask SendPackageAsync(FilePackageInfo packageInfo)
    {
        if (Channel.IsClosed)
            return ValueTask.CompletedTask;

        return Channel.SendAsync(_packageEncoder, packageInfo);
    }

    protected override ValueTask OnSessionClosedAsync(CloseEventArgs e)
    {
        var fileStream = FileStream;

        if (fileStream == null)
            return base.OnSessionClosedAsync(e);

        fileStream.Close();
        fileStream.Dispose();
        FileStream = null;

        return base.OnSessionClosedAsync(e);
    }

    internal void CreateFile(string name)
    {
        lock (_fileRoot)
        {
            var fileStream = FileStream;

            if (fileStream != null)
                throw new Exception();

            if (!Directory.Exists(FileDirectory))
                Directory.CreateDirectory(FileDirectory);

            var filePath = Path.Combine(FileDirectory, name);

            if (File.Exists(filePath))
            {
                FileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            }
            else
            {
                FileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            }

            FileStream.Position = FileStream.Length;
        }
    }

    internal async ValueTask SaveAsync(List<ArraySegment<byte>> body)
    {
        var fileStream = FileStream;

        ArgumentNullException.ThrowIfNull(fileStream);

        foreach (var item in body)
            await fileStream.WriteAsync(item);
    }

    internal void CloseFile()
    {
        lock (_fileRoot)
        {
            var fileStream = FileStream;

            ArgumentNullException.ThrowIfNull(fileStream);

            fileStream.Close();
            fileStream.Dispose();
            FileStream = null;
        }
    }
}