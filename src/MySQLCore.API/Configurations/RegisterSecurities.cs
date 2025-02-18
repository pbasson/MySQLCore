using System.Security.Cryptography.X509Certificates;
using MySQLCore.Core.CoreHelpers;

namespace MySQLCore.API.Configurations {
    public static class RegisterSecurities
    {
        public static void RegisterSecurity(this WebApplicationBuilder builder)
        {
            var certPath = Environment.GetEnvironmentVariable(AppSettings.CERTIFICATE_FILE) ?? builder.Configuration[AppSettings.CERTIFICATE_FILE];
            var certPassword = Environment.GetEnvironmentVariable(AppSettings.CERTIFICATE_PASSWORD) ?? builder.Configuration[AppSettings.CERTIFICATE_PASSWORD];

            if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(certPassword))
            {
                var certificate = new X509Certificate2(certPath, certPassword);

                builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(7840, listenOptions => { listenOptions.UseHttps(certificate); }); });
                Console.WriteLine(API_Variables.CertificateLoaded);
            }
            else { Console.WriteLine(API_Variables.CertificateMissing); }
        }
    }
}