using SuperSocket.ProtoBase;
using System.Buffers;
using System.Text;

namespace SuperSocketFileServer;

public sealed class UpLoadPackageInfo : FilePackageInfo
{
    public string? FileName { get; set; }

    public int FileSize { get; set; }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        throw new NotImplementedException();
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        reader.TryRead(out byte fileNameLength);
        FileName = reader.ReadString(Encoding.UTF8, fileNameLength);

        reader.TryReadLittleEndian(out int fileLength);
        FileSize = fileLength;
    }
}
