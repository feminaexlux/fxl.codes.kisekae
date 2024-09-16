using fxl.codes.kisekae.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContextFactory<KisekaeContext>(options => { options.UseNpgsql(builder.Configuration["kisekae:ConnectionString"]); });

var app = builder.Build();
app.Run();