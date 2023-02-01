using SuperSocket.ProtoBase;
using System.Buffers;

namespace SuperSocketFileServer;

internal sealed class FilePacketDecode : IPackageDecoder<FilePackageInfo>
{
    public FilePacketDecode()
    {
        _packetFactories = new IPacketFactory[4];
        RegisterPacketType<DataPackageInfo>(CommandKey.Data);
        RegisterPacketType<EndPackageInfo>(CommandKey.End);
        RegisterPacketType<UpLoadPackageInfo>(CommandKey.Upload);
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

    public void RegisterPacketType<TPacket>(CommandKey command)
        where TPacket : FilePackageInfo, new()
    {
        _packetFactories[(byte)command] = new DefaultPacketFactory<TPacket>();
    }

    public FilePackageInfo Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        var reader = new SequenceReader<byte>(buffer);

        reader.TryRead(out byte command);

        var package = _packetFactories[command].Create();

        package.Key = (CommandKey)command;
        package.DecodeBody(ref reader, context);

        return package;
    }
}
