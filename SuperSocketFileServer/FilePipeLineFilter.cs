using SuperSocket.ProtoBase;
using System.Buffers;

namespace SuperSocketFileServer;

public sealed class FilePipeLineFilter : FixedHeaderPipelineFilter<FilePackageInfo>
{
    private const int HeadSize = 2;

    public FilePipeLineFilter()
        : base(HeadSize)
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

    protected override FilePackageInfo DecodePackage(ref ReadOnlySequence<byte> buffer)
    {
        var reader = new SequenceReader<byte>(buffer.Slice(HeadSize));

        reader.TryRead(out byte command);

        var package = _packetFactories[command].Create();

        package.Key = (CommandKey)command;
        package.DecodeBody(ref reader, Context);

        return package;
    }

    protected override int GetBodyLengthFromHeader(ref ReadOnlySequence<byte> buffer)
    {
        var reader = new SequenceReader<byte>(buffer);

        reader.TryReadLittleEndian(out short length);

        return length;
    }
}
