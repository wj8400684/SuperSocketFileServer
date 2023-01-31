using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using Microsoft.Extensions.Logging;
using SuperSocket;
using SuperSocket.Channel;
using SuperSocket.IOCPTcpChannel;
using SuperSocket.ProtoBase;
using SuperSocket.Server;

namespace supersocketIocpServer;

internal sealed class TcpIocpChannelCreatorFactory : TcpChannelCreatorFactory, IChannelCreatorFactory
{
    public TcpIocpChannelCreatorFactory(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    public new IChannelCreator CreateChannelCreator<TPackageInfo>(ListenOptions options, ChannelOptions channelOptions,
        ILoggerFactory loggerFactory, object pipelineFilterFactory)
    {
        var filterFactory = pipelineFilterFactory as IPipelineFilterFactory<TPackageInfo>;

        ArgumentNullException.ThrowIfNull(filterFactory);

        channelOptions.Logger = loggerFactory.CreateLogger(nameof(IChannel));

        var channelFactoryLogger = loggerFactory.CreateLogger(nameof(TcpChannelCreator));

        if (options.Security == SslProtocols.None)
        {
            return new TcpChannelCreator(options, (s) =>
            {
                ApplySocketOptions(s, options, channelOptions, channelFactoryLogger);
                return new ValueTask<IChannel>(
                    (new IOCPTcpPipeChannel<TPackageInfo>(s, filterFactory.Create(s), channelOptions)));
            }, channelFactoryLogger);
        }
        else
        {
            var channelFactory = new Func<Socket, ValueTask<IChannel>>(async (s) =>
            {
                ApplySocketOptions(s, options, channelOptions, channelFactoryLogger);

                var authOptions = new SslServerAuthenticationOptions
                {
                    EnabledSslProtocols = options.Security,
                    ServerCertificate = options.CertificateOptions.Certificate,
                    ClientCertificateRequired = options.CertificateOptions.ClientCertificateRequired
                };

                if (options.CertificateOptions.RemoteCertificateValidationCallback != null)
                    authOptions.RemoteCertificateValidationCallback =
                        options.CertificateOptions.RemoteCertificateValidationCallback;

                var stream = new SslStream(new NetworkStream(s, true), false);
                await stream.AuthenticateAsServerAsync(authOptions, CancellationToken.None).ConfigureAwait(false);
                return new SslStreamPipeChannel<TPackageInfo>(stream, s.RemoteEndPoint, s.LocalEndPoint,
                    filterFactory.Create(s), channelOptions);
            });

            return new TcpChannelCreator(options, channelFactory, channelFactoryLogger);
        }
    }
}