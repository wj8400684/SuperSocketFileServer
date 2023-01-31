using SuperSocket.ProtoBase;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace SuperSocketFileServer;

public abstract class FilePackageInfo : IKeyedPackageInfo<CommandKey>
{
    public CommandKey Key { get; set; }

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
        const int v1 = sizeof(byte);

        var successFul = SuccessFul;
        var successFulBuffer = writer.GetSpan(v1);
        MemoryMarshal.Write(successFulBuffer, ref successFul);
        writer.Advance(v1);

        //如果没有错误或者错误为空不写入
        if (successFul || string.IsNullOrWhiteSpace(ErrorMessage))
            return v1;

        var errorMessageLength = ErrorMessage.Length;
        var errorMessageBuffer = writer.GetSpan(v1);

        MemoryMarshal.Write(errorMessageBuffer, ref errorMessageLength);
        writer.Advance(v1);

        var length = writer.Write(ErrorMessage, Encoding.UTF8);

        return v1 + v1 + length;
    }
}
