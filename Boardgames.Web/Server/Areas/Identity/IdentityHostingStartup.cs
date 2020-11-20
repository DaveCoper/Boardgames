using System;
using System.Security.Claims;
using Boardgames.Web.Server.Data;
using Boardgames.Web.Server.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Boardgames.Web.Server.Areas.Identity.IdentityHostingStartup))]
namespace Boardgames.Web.Server.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(
                (context, services) =>
                {
                });
        }
    }
}