using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using AppCollection.Models;
using AppCollection.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options => { options.Filters.Add(new AuthorizeFilter()); })
    .AddViewLocalization().AddDataAnnotationsLocalization();
// Add localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
//Auth
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = false;
    });
// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<CleanupService>();
builder.Services.AddSingleton<WeatherService>();
builder.Services.AddScoped<SearchService>(p =>
{
    var context = p.GetRequiredService<ApplicationDbContext>();
    var httpClientFactory = p.GetRequiredService<IHttpClientFactory>();
    return new SearchService(httpClientFactory, context);
});

builder.Services.AddScoped<PdfSignatureService>();
builder.Services.AddScoped<DocumentService>(provider =>
{
    var context = provider.GetRequiredService<ApplicationDbContext>();
    var configuration = provider.GetRequiredService<IConfiguration>();
    var storageRoot = configuration.GetValue<string>("Storage:Root") ?? "UserDocs";
    return new DocumentService(context, storageRoot);
});
//CONNECTION STRING STORED IN CONFIG - appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
// Set supported cultures
var supportedCultures = new[] { "en", "cs" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("cs")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

localizationOptions.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
app.UseRequestLocalization(localizationOptions);
// Configure the HTTP request pipeline.

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = errorFeature?.Error;
        //exception handling for db err
        if (exception is SqlException || exception is InvalidOperationException)
        {
            context.Response.Redirect("/Home/DatabaseError");
        }
        else
        {
            context.Response.Redirect("/Home/Error");
        }
    });
});

// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();