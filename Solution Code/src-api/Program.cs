using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Contexts;
using src_api.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); 

var CentralDBConnectrionString = builder.Configuration.GetConnectionString("CentralDB");
builder.Services.AddDbContext<CentralDBContext>(
    options => options.UseSqlServer(CentralDBConnectrionString)
);

var siteDBConnectionString = builder.Configuration.GetConnectionString("SiteDB");
builder.Services.AddDbContext<SiteDBContext>(
    options => options.UseSqlServer(siteDBConnectionString)
);

builder.Services.AddLogging(config => config.AddLog4Net());
builder.Services.AddScoped<IAuthHelper, AuthHelper>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddMvc();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseWhen(context => !context.Request.Path.StartsWithSegments("/Auth/Login"),
    configuration =>
    {
        configuration.UseMiddleware<AuthorizationMiddleware>();
    });
//app.Use(async (context, next) =>
//{
//    // Check if the token is present in the request headers
//    // var token = context.Request.Cookies["token"];



//    var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImdvLmRqIiwicHBhdXNlcmlkIjoiMTY4MiIsIm1lbWJlcm9mIjoiIiwidGh1bWJuYWlscGhvdG8iOiIiLCJzZXJ2ZXJJZCI6IjEiLCJzZXJ2ZXJOYW1lIjoiTlBMLURFVi1EVEEiLCJzZXJ2ZXIiOiJOUEwtREVWLURUQSIsInNlcnZlckRCIjoiR0JEQiIsInN1cGVyYWRtaW4iOiJGYWxzZSIsImFzcGVjdGluZ3NpdGUiOiJUcnVlIiwibmJmIjoxNzIzNDQ5NTQxLCJleHAiOjE3MjM1MzU5NDEsImlhdCI6MTcyMzQ0OTU0MX0._mFhh8aB8p2E93fQK-07fJwM7Ctzosma381OPwaCpbM";



//    if (!string.IsNullOrEmpty(token))
//    {
//        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("I really really really love to code."));



//        var tokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = secretKey,
//            ValidateIssuer = false,
//            ValidateAudience = false
//        };



//        var tokenHandler = new JwtSecurityTokenHandler();



//        ClaimsPrincipal claimsPrincipal;
//        try
//        {
//            claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
//            var server = claimsPrincipal.FindFirst("server");
//            var ppaId = claimsPrincipal.FindFirst("ppauserid");
//            var serverId = claimsPrincipal.FindFirst("serverId");
//            if (server != null && ppaId != null && serverId != null)
//            {
//                context.Items["Server"] = server.Value;
//                context.Items["PPAUserId"] = ppaId.Value;
//                context.Items["ServerId"] =  serverId.Value;



//            }
//        }
//        catch (SecurityTokenException)
//        {
//            Console.WriteLine("Token Validation failed");
//        }
//    }



//    await next.Invoke();
//});

app.MapControllers();

app.Run();
