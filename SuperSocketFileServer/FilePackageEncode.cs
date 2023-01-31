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
        var cmd = (byte)pack.Key;
        int bodyLength = CmdSize;

        #region ��ȡͷ���ֽڻ����� 2 byte

        var headBuffer = writer.GetSpan(HeadSize);
        writer.Advance(HeadSize);

        #endregion

        #region д������ 1 byte

        var cmdBuffer = writer.GetSpan(CmdSize);
        MemoryMarshal.Write(cmdBuffer, ref cmd);
        writer.Advance(CmdSize);

        #endregion

        #region д��body

        bodyLength += pack.EncodeBody(writer);

        #endregion

        #region д��body�ֽڳ��� 2 byte

        BinaryPrimitives.WriteInt16LittleEndian(headBuffer, (short)bodyLength);

        #endregion

        return bodyLength + HeadSize;
    }
}