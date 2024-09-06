using FBAdsManager.Common.ConfigureService;
using FBAdsManager.Common.Database.DbContexts;
using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Jwt;
using FBAdsManager.Module.Auths.Services;
using FBAdsManager.Module.Branches.Services;
using FBAdsManager.Module.Employees.Services;
using FBAdsManager.Module.Groups.Services;
using FBAdsManager.Module.Organizations.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerService();
builder.Services.AddScoped<DbAdsmanagerContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAuthSerivce(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

#region AddServiceForController
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
#endregion

#region Configure Database
    var connect = builder.Configuration.GetConnectionString("Value");
    builder.Services.AddDbContext<DbAdsmanagerContext>(options =>
    {
        options.UseMySql(connect, ServerVersion.AutoDetect(connect));
    });
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors("AllowAll");
app.Run();
