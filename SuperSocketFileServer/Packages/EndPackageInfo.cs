using System.Buffers;

namespace SuperSocketFileServer;

public sealed class EndPackageInfo : FilePackageInfo
{
    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        throw new NotImplementedException();
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
    }
}

public sealed class EndAckPackageInfo : FileAckPackageInfo
{
}
