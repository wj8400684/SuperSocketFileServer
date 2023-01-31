
using System.Buffers;
using System.Runtime.InteropServices;

namespace SuperSocketFileServer;

public sealed class DataPackageInfo : FilePackageInfo
{
    private const int DefaultCapacity = 10;

    public List<ArraySegment<byte>>? Body { get; set; }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        throw new NotImplementedException();
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object context)
    {
        var bufferList = new List<ArraySegment<byte>>(DefaultCapacity);

        foreach (var memory in reader.UnreadSequence)
        {
            if (!MemoryMarshal.TryGetArray(memory, out var result))
                throw new InvalidOperationException("Buffer backed by array was expected");

            bufferList.Add(result);
        }

        Body = bufferList;
    }
}

public sealed class DataAckPackageInfo : FileAckPackageInfo
{
}
