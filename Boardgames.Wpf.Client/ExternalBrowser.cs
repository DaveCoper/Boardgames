using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.OidcClient.Browser;

namespace Boardgames.Wpf.Client
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

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(options.EndUrl);
            listener.Start();

            var psi = new ProcessStartInfo
            {
                FileName = options.StartUrl,
                UseShellExecute = true
            };
            Process.Start(psi);

            var context = await listener.GetContextAsync();

            var result = new BrowserResult
            {
                ResultType = BrowserResultType.Success,
                Response = context.Request.Url.AbsoluteUri,
            };

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Return to application"));
            
            context.Response.Close();
            listener.Stop();


            return result;
        }
    }
}