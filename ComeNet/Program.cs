using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ComeNet.Data;
using AWSWEBAPP.Services;
using ComeNet.Models;
using static ComeNet.Models.UserService;
using WebRTC.Hubs;
using ComeNet.Hubs;
using ComeNet.Services;
using Microsoft.AspNetCore.Mvc;
using System;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
builder.Services.AddDbContext<ComeNetContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IAwslogService, AwslogService>();
builder.Services.AddScoped<IUserConnectionManager, UserConnectionManager>();

builder.Services.AddSignalR();



// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddHttpClient();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseWebSockets();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapHub<DefaultHubs>("/meeting");
app.MapHub<NotificationUserHub>("/NotificationUserHub");

app.MapControllerRoute
    (
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
