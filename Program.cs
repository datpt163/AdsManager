using FBAdsManager.Common.CallApi;
using FBAdsManager.Common.ConfigureService;
using FBAdsManager.Common.Database.DbContexts;
using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Helper;
using FBAdsManager.Common.Jwt;
using FBAdsManager.Module.Ads.Services;
using FBAdsManager.Module.AdsAccount.Services;
using FBAdsManager.Module.Adsets.Services;
using FBAdsManager.Module.Auths.Services;
using FBAdsManager.Module.BM.Services;
using FBAdsManager.Module.Branches.Services;
using FBAdsManager.Module.Campaigns.Services;
using FBAdsManager.Module.Dashboard.Services;
using FBAdsManager.Module.DataFacebook.Services;
using FBAdsManager.Module.Employees.Services;
using FBAdsManager.Module.Groups.Services;
using FBAdsManager.Module.Organizations.Services;
using FBAdsManager.Module.Users.Services;
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
builder.Services.AddScoped<CallApiService, CallApiService>();
builder.Services.AddAuthSerivce(builder.Configuration);
builder.Services.AddScoped<TelegramHelper, TelegramHelper>();
builder.Services.AddScoped<IExcelHelper, ExcelHelper>();
#region AddServiceForController
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAdsAccountService, AdsAccountService>();
builder.Services.AddScoped<IDataFacebookService, DataFacebookService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddScoped<IAdsetService, AdsetService>();
builder.Services.AddScoped<IAdsService, AdsService>();
builder.Services.AddScoped<IBmService, BmService>();
builder.Services.AddScoped<IDashBoardService, DashboardService>();
#endregion

#region Configure Database
var connect = builder.Configuration.GetConnectionString("Value");
    builder.Services.AddDbContext<DbAdsmanagerContext>(options =>
    {
        options.UseMySql(connect, ServerVersion.AutoDetect(connect));
    });
#endregion

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

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
