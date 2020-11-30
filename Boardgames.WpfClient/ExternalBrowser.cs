using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.OidcClient.Browser;

namespace Boardgames.WpfClient
{
    public class ExternalBrowser : IBrowser
    {
        public ExternalBrowser()
        {
        }

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            if (!HttpListener.IsSupported)
            {
                throw new Exception("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            
            using HttpListener listener = new HttpListener();
            listener.Prefixes.Add(options.EndUrl);
            listener.Start();

            var psi = new ProcessStartInfo
            {
                FileName = options.StartUrl,
                UseShellExecute = true
            };

            Process.Start(psi);

            cancellationToken.ThrowIfCancellationRequested();
            var context = await listener.GetContextAsync();

            var result = new BrowserResult
            {
                ResultType = BrowserResultType.Success,
                Response = context.Request.Url.AbsoluteUri,
            };

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "text/html";
            context.Response.ContentEncoding = Encoding.UTF8;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Boardgames.WpfClient.Resources.LoginSuccessful.html";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                await stream.CopyToAsync(context.Response.OutputStream, cancellationToken);
                await context.Response.OutputStream.FlushAsync(cancellationToken);
            }

            context.Response.Close();
            listener.Stop();

            cancellationToken.ThrowIfCancellationRequested();
            return result;
        }
    }
}