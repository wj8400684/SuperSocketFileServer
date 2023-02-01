using SuperSocket.ProtoBase;
using System.Buffers;

namespace SuperSocketFileServer;

internal sealed class FilePipeLineFilter : FixedHeaderPipelineFilter<FilePackageInfo>
{
    private const int HeadSize = 2;

    public FilePipeLineFilter()
        : base(HeadSize)
    {
    }

    protected override int GetBodyLengthFromHeader(ref ReadOnlySequence<byte> buffer)
    {
        var reader = new SequenceReader<byte>(buffer);

        reader.TryReadLittleEndian(out short length);

        return length;
    }
}
