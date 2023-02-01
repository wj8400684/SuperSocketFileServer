using SuperSocket.ProtoBase;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace SuperSocketFileServer;

public abstract class FilePackageInfo : IKeyedPackageInfo<FileCommandKey>
{
    public FileCommandKey Key { get; set; }

    internal protected abstract void DecodeBody(ref SequenceReader<byte> reader, object context);

    public abstract int EncodeBody(IBufferWriter<byte> writer);
}

public abstract class FileAckPackageInfo : FilePackageInfo
{
    public bool SuccessFul { get; set; }

    public string? ErrorMessage { get; set; }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        if (!reader.TryRead(out byte successFul))
            return;

        SuccessFul = successFul == 1;

        if (SuccessFul)
            return;

        if (!reader.TryRead(out byte errorMessageLength) || errorMessageLength == 0)
            return;

        ErrorMessage = reader.ReadString(errorMessageLength);
    }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        var length = writer.WriterBit(SuccessFul ? (byte)1 : (byte)0);

        //如果没有错误或者错误为空不写入
        if (SuccessFul || string.IsNullOrWhiteSpace(ErrorMessage))
            return length;

        length += writer.WriterStringWithLength(ErrorMessage);

        return length;
    }
}
