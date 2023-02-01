using SuperSocket.ProtoBase;
using System.Buffers;

namespace SuperSocketFileServer;

internal sealed class FilePackageDecode : IPackageDecoder<FilePackageInfo>
{
    private const int HeadSize = 2;

    public FilePackageDecode()
    {
        _packetFactories = new IPacketFactory[4];
        RegisterPacketType<DataPackageInfo>(FileCommandKey.Data);
        RegisterPacketType<EndPackageInfo>(FileCommandKey.End);
        RegisterPacketType<UpLoadPackageInfo>(FileCommandKey.Upload);
    }

    private IPacketFactory[] _packetFactories;

    interface IPacketFactory
    {
        FilePackageInfo Create();
    }

    class DefaultPacketFactory<TPacket> : IPacketFactory
        where TPacket : FilePackageInfo, new()
    {
        public FilePackageInfo Create()
        {
            return new TPacket();
        }
    }

    public void RegisterPacketType<TPacket>(FileCommandKey command)
        where TPacket : FilePackageInfo, new()
    {
        _packetFactories[(byte)command] = new DefaultPacketFactory<TPacket>();
    }

    public FilePackageInfo Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        var reader = new SequenceReader<byte>(buffer.Slice(HeadSize));

        reader.TryRead(out byte command);

        var package = _packetFactories[command].Create();

        package.Key = (FileCommandKey)command;
        package.DecodeBody(ref reader, context);

        return package;
    }
}
