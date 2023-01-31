using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperSocket;
using SuperSocket.Command;
using SuperSocket.ProtoBase;
using SuperSocketFileServer;
using supersocketIocpServer;

await SuperSocketHostBuilder.Create<FilePackageInfo, FilePipeLineFilter>()
    .UseSession<MyAppSession>()
    .UseCommand(options => options.AddCommandAssembly(typeof(Data).Assembly))
    .ConfigureServices(
        (context, service) => service.AddSingleton<IPackageEncoder<FilePackageInfo>, FilePackageEncode>())
    .UseChannelCreatorFactory<TcpIocpChannelCreatorFactory>()
    .Build()
    .RunAsync();