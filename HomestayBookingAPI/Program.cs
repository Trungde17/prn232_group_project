
using BusinessObjects;
using BusinessObjects.Bookings;
using BusinessObjects.Homestays;
using BusinessObjects.Rooms;
using DataAccess;
using HomestayBookingAPI.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Repositories;
using Repositories.BookingRepository;
using Repositories.HomeStayRepository;
using Repositories.RoomRepository;
using Services;
using Services.BookingServices;
using Services.HomestayServices;
using Services.RoomServices;
using System.Text;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
    // Thêm sự kiện để debug lỗi xác thực
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<HomestayDbContext>()
    .AddDefaultTokenProviders();
// dki dich vu mail
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
//homestay service
builder.Services.AddScoped<IGenericRepository<Homestay>, HomeStayRepository>();
builder.Services.AddScoped<IGenericRepository<HomestayAmenity>, HomestayAmenityRepository>();
builder.Services.AddScoped<IGenericRepository<HomestayPolicy>, HomestayPolicyRepository>();
builder.Services.AddScoped<IGenericRepository<HomestayNeighbourhood>, HomestayNeighbourhoodRepository>();
builder.Services.AddScoped<IGenericRepository<HomestayImage>, HomestayImageRepository>();
builder.Services.AddScoped<IGenericRepository<HomestayType>, HomestayTypeRepository>();

builder.Services.AddScoped<IHomestayService, HomestayService>();
//room service
builder.Services.AddScoped<IGenericRepository<Room>, RoomRepository>();
builder.Services.AddScoped<IGenericRepository<RoomAmenity>, RoomAmenityRepository>();
builder.Services.AddScoped<IGenericRepository<RoomSchedule>, RoomScheduleRepository>();
builder.Services.AddScoped<IGenericRepository<RoomBed>, RoomBedRepository>();
builder.Services.AddScoped<IGenericRepository<RoomPrice>, RoomPriceRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();
//booking service
builder.Services.AddScoped<IGenericRepository<Booking>, BookingRepository>();
builder.Services.AddScoped<IGenericRepository<BookingDetail>, BookingDetailRepository>();

builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddScoped<IGenericRepository<Ward>, WardRepository>();

builder.Services.AddScoped<FavoriteHomestayRepository>();
builder.Services.AddScoped<IFavoriteHomestayService, FavoriteHomestayService>();

//dki OData
IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<Booking>("Bookings");
    builder.EntitySet<Homestay>("Homestays");
    builder.EntitySet<Room>("Rooms");
    builder.EntitySet<FavoriteHomestay>("FavoriteHomestays");
    return builder.GetEdmModel();
}
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    })
    .AddOData(opt =>
    opt.AddRouteComponents("odata", GetEdmModel())
        .Select().Filter().Expand().Count().OrderBy().SetMaxTop(100));

//dki AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));


var app = builder.Build();
// Tạo scope để gọi dịch vụ DI
//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//    await DbSeeder.SeedRolesAsync(roleManager);  //  Gọi hàm seed
//}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
