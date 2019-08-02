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

        private static IServiceCollection serviceCollection; //= new ServiceCollection();
        private static Application _app; //= new Application(serviceCollection);
        private static Logger Logger = new LoggerConfiguration().MinimumLevel.Debug()
                                    .WriteTo.File(Utils.LogPath).CreateLogger();

        static void Main(string[] args)
        {
            Init();
            var server = HostBuilder.CreateServer(args);
            var viewsRenderer = new ViewsRenderer(server, "views");
            Console.WriteLine("UTILS LOGPATH: " + Utils.LogPath);

            viewsRenderer.SetupHandlers();
            viewsRenderer.SetupControllers();

            server.Run();
        }

        public static void Init(bool reInit = false)
        {
            if (_app == null || reInit)
            {
                serviceCollection = new ServiceCollection();
                _app = new Application(serviceCollection);
            }
        }
    }

    public class Application
    {
        public IServiceProvider Services { get; set; }
        private static Logger Logger = new LoggerConfiguration().MinimumLevel.Debug()
                                    .WriteTo.File(Utils.LogPath).CreateLogger();

        public Application(IServiceCollection serviceCollection)
        {
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            Logger.Information("TYPE COLLECTION: " + serviceCollection.GetType());
            Logger.Information("Settings.RpcServerUrl" + Settings.RpcServerUrl);
            serviceCollection.AddScoped<IPhantasmaRpcService>(provider => new PhantasmaRpcService(
                new Phantasma.RpcClient.Client.RpcClient(new Uri(Settings.RpcServerUrl), httpClientHandler: new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            })));
        }
    }
}
