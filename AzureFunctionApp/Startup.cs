//using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using AzureFunctionApp;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[assembly : FunctionsStartup(typeof(Startup))]
namespace AzureFunctionApp
{ 
    public class Startup : FunctionsStartup
    {
        
        public override void Configure(IFunctionsHostBuilder host)
        {
            var env = Environment.GetEnvironmentVariable("env");
            //DI
            //host.services.addscoped<interface,class>();
            host.Services.AddAzureAppConfiguration();

            AzureAppSettings _azureappsettings = new();

            IConfiguration configuration = host.Services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            //bringing app configurationa azure into iconfig code
            host.Services.Configure<AzureAppConfiguration>(configuration.GetSection("appconfiguration:settings:functionapp"));

            //azureappsettings
            var appsettings = _azureappsettings.getazureconfig(host.Services);

            //app insights configuraiotn if needed

            
        }
        //this one gets first
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            // I also have json files with loglevel and appinsights instrumentation key
            KeyvaultAccess _service = new();
            string key = $"connectionstrings--functionapp-azureappconfig";

            //for this one you will need the resource to be open to connect and your azure account to have access to the keyvaul if you want to retrieve stuff fomr there
            var azureappcs = _service.GetSecretValue(key).Result;

            //add keyvault to config

            //azureservicetokenprovider azuretoken = null;
            //builder.configurationbuilder.addazurekeyvault(new uri(keyivaultendpoint),new defaultazurecredential());

            //add azureappconfiguratoin to config

            //builder.configurationbuilder.addazureappconfiguration(x => 
            //{ x.connect(azureappcs)
            //      load all keys that start with appconfiguraiton and  label
            //  .select("appconfiguraiton,{env}functionapp
            //  .configurerefresh
            //}


        }
    }

    public class AzureAppSettings
    { 
        //Notes:
        //IOptions is designed where the config is read once durin ghte applicaiton startup
        //IOptionsSnapshot is designed where the config may change during the lifetime of application and we want to get the latest value
        public AzureAppConfiguration getazureconfig(IServiceCollection builder)
        {
            var serviceprovider = builder.BuildServiceProvider();
            var azureappconfig = serviceprovider.GetService<IOptionsSnapshot<AzureAppConfiguration>>();

            return azureappconfig.Value;
        }
    }
    public class AzureAppConfiguration
    {
        public string? LogLevel { get; set; }
        public string? AppInsights {  get; set; }
    }
    public class KeyvaultAccess
    {
        //to get into keyvalut and get azure app connection string
        public async Task<string> GetSecretValue(string keyname)
        {
            string secret = "";

            //AzureServiceTokenProvider azuretoken = new AzureServiceTokenProvider();
            //var secretbundle = await keyvaultclient.getsecretasync("http:keyvault/secrets/{keyname}).configureawit(false);
            // var secret  = secretbundle.value

            return secret;

        }
    }

}
