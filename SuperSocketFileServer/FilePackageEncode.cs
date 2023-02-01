using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using SuperSocket.ProtoBase;
using SuperSocketFileServer;

namespace supersocketIocpServer;

internal sealed class FilePackageEncode : IPackageEncoder<FilePackageInfo>
{
    private const int HeadSize = sizeof(short);
    private const int CmdSize = sizeof(byte);

    public int Encode(IBufferWriter<byte> writer, FilePackageInfo pack)
    {
        int bodyLength = CmdSize;

        #region 获取头部字节缓冲区 2 byte

        var headBuffer = writer.GetSpan(HeadSize);
        writer.Advance(HeadSize);

        #endregion

        #region 写入命令 1 byte

        writer.WriterBit((byte)pack.Key);

        #endregion

        #region 写入body

        bodyLength += pack.EncodeBody(writer);

        #endregion

        #region 写入body字节长度 2 byte

        BinaryPrimitives.WriteInt16LittleEndian(headBuffer, (short)bodyLength);

        #endregion

        return bodyLength + HeadSize;
    }
}