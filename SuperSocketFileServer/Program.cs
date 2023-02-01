using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperSocket;
using SuperSocket.Command;
using SuperSocket.ProtoBase;
using SuperSocketFileServer;
using supersocketIocpServer;

await SuperSocketHostBuilder.Create<FilePackageInfo, FilePipeLineFilter>()
    .UseSession<FileAppSession>()
    .UsePackageDecoder<FilePackageDecode>()
    .UseCommand(options => options.AddCommandAssembly(typeof(DataCommand).Assembly))
    .UseChannelCreatorFactory<TcpIocpChannelCreatorFactory>()
    .ConfigureServices(
        (context, service) => service.AddSingleton<IPackageEncoder<FilePackageInfo>, FilePackageEncode>())
    .Build()
    .RunAsync();