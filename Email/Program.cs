using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using SmtpServer;
using SmtpServer.Mail;
using SmtpServer.Protocol;
using SmtpServer.Storage;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Email
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.Name = "Email Tester";
            app.HelpOption("-?|-h|--help");
            var configPathOption = app.Option("-c|--config <configfile>", "Specify file with configuration", CommandOptionType.SingleValue);
            
            app.OnExecute(() =>
            {
                var configPath = "appsettings.json";
                if (configPathOption.HasValue())
                {
                    configPath = configPathOption.Value();
                }

                IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configPath, false, true)
                .AddJsonFile("secrets.json", true, true)
                .Build();

                Console.WriteLine("Starting");

                Run(config).Wait();

                return 0;
            });


            app.Execute(args);


            
            
        }

        static async Task Run(IConfiguration config)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var optionsBuilder = new DbContextOptionsBuilder<MainDbContext>();            
            optionsBuilder.UseNpgsql(config.GetConnectionString("MainDbContext"));
            var db = new MainDbContext(optionsBuilder.Options);

            var ports = config["Ports"].Split(",").Select(x=>int.Parse(x)).ToArray();

            var options = new OptionsBuilder()
                .ServerName(config["ServerName"])
                .Port(ports)                
                .UserAuthenticator(new MainUserAuthenticator(db))
                .MessageStore(new MainMessageStore(db))
                .AllowUnsecureAuthentication(true)
                .AuthenticationRequired(true)
                .Build();

            var smtpServer = new SmtpServer.SmtpServer(options);

            Console.WriteLine("Started");

            var serverTask =  smtpServer.StartAsync(cancellationTokenSource.Token);

            await serverTask;
        }
    }
    
}
