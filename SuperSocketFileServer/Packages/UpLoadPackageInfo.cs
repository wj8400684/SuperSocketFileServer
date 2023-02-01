using SuperSocket.ProtoBase;
using System.Buffers;
using System.Text;

namespace SuperSocketFileServer;

public sealed class UpLoadPackageInfo : FilePackageInfo
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public string FileName { get; set; } = null!;

    /// <summary>
    /// 相对路径
    /// </summary>
    public string? RelativeDirectory { get; set; }

    /// <summary>
    /// 文件长度
    /// 比如 D:\.net\git\SuperSocketFileServer\SuperSocketFileServer\bin\Release\net7.0\runtimes 则或者 .net\git\SuperSocketFileServer\SuperSocketFileServer\bin\Release\net7.0\runtimes
    /// </summary>
    public long FileLength { get; set; }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        throw new NotImplementedException();
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        reader.TryReadLittleEndian(out long fileLength);
        FileLength = fileLength;

        reader.TryRead(out byte fileNameLength);
        FileName = reader.ReadString(Encoding.UTF8, fileNameLength);

        if (reader.TryRead(out byte directoryNameLength))
            RelativeDirectory = reader.ReadString(Encoding.UTF8, directoryNameLength);
    }
}

public sealed class UpLoadAckPackageInfo : FileAckPackageInfo
{

}
