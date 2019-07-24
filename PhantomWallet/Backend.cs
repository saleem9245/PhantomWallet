using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Phantasma.RpcClient;
using Phantasma.RpcClient.Interfaces;
using Phantom.Wallet.Helpers;
using Serilog;
using Serilog.Core;

namespace Phantom.Wallet
{
    class Backend
    {
        public static IServiceProvider AppServices => _app.Services;
        private static IServiceCollection serviceCollection = new ServiceCollection();
        private static Application _app = new Application(serviceCollection);
        private static Logger Logger = new LoggerConfiguration().MinimumLevel.Debug()
                                    .WriteTo.File(Utils.LogPath).CreateLogger();

        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            _app = new Application(serviceCollection);

            var server = HostBuilder.CreateServer(args);
            var viewsRenderer = new ViewsRenderer(server, "views");

            viewsRenderer.SetupHandlers();
            viewsRenderer.SetupControllers();

            server.Run();
        }
    }

    public class Application
    {
        public IServiceProvider Services { get; set; }

        public Application(IServiceCollection serviceCollection)
        {
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IPhantasmaRpcService>(provider => new PhantasmaRpcService(new Phantasma.RpcClient.Client.RpcClient(new Uri("http://localhost:7077/rpc"), httpClientHandler: new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            })));
        }
    }
}
