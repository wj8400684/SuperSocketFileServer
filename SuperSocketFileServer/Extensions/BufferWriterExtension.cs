using System.Buffers.Binary;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using SuperSocket.ProtoBase;

namespace SuperSocketFileServer;

internal static class BufferWriterExtension
{
    public static int WriterBit(this IBufferWriter<byte> writer, byte value)
    {
        const int v1 = sizeof(byte);

        var buffer = writer.GetSpan(v1);

        MemoryMarshal.Write(buffer, ref value);
        writer.Advance(v1);

        return v1;
    }

    public static int WriterInt32(this IBufferWriter<byte> writer, int value)
    {
        const int v1 = sizeof(int);

        var buffer = writer.GetSpan(v1);

        BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
        writer.Advance(v1);

        return v1;
    }

    public static int WriterShort(this IBufferWriter<byte> writer, short value)
    {
        const int v1 = sizeof(short);

        var buffer = writer.GetSpan(v1);

        BinaryPrimitives.WriteInt16LittleEndian(buffer, value);
        writer.Advance(v1);

        return v1;
    }

    public static int WriterStringWithLength(this IBufferWriter<byte> writer, string value)
    {
        const int v1 = sizeof(byte);

        var buffer = writer.GetSpan(v1);

        writer.Advance(v1);

        var length = (byte)writer.Write(value, Encoding.UTF8);

        MemoryMarshal.Write(buffer, ref length);

        return v1 + length;
    }
}
