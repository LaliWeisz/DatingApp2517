using System;
using System.Linq;
using System.Text;
using API;
using API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
var app = builder.Build();
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod()
.WithOrigins("https://localhost:4200"));
app.UseAuthentication();//בדיקת תקינות הטוקן
app.UseAuthorization();//בדיקת הרשאה לפי הטוקן
app.UseHttpsRedirection();
app.UseAuthentication();
app.MapControllers();
app.Run();

