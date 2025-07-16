using BusinessObjects;
using BusinessObjects.Bookings;
using BusinessObjects.Homestays;
using BusinessObjects.Rooms;
using DataAccess;
using HomestayBookingAPI;
using HomestayBookingAPI.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Repositories;
using Repositories.HomeStayRepository;
using Repositories.RoomRepository;
using Services;
using Services.HomestayServices;
using Services.RoomServices;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddOData(opt =>
    opt.AddRouteComponents("odata", GetEdmModel())
        .Select().Filter().Expand().Count().OrderBy().SetMaxTop(100));// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Database
builder.Services.AddDbContext<HomestayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// dki jwt
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<HomestayDbContext>()
    .AddDefaultTokenProviders();
// dki dich vu mail
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
//dki OData
IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<Room>("Rooms");
    builder.EntitySet<Homestay>("Homestays");
   
    return builder.GetEdmModel();
}




builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IGenericRepository<Room>, RoomRepository>();
builder.Services.AddScoped<IGenericRepository<Homestay>, HomeStayRepository>();
builder.Services.AddScoped<HomeStayRepository>();



builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IHomestayService, HomestayService>();

//dki AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
var app = builder.Build();
// Tạo scope để gọi dịch vụ DI
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await DbSeeder.SeedRolesAsync(roleManager);  //  Gọi hàm seed
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
