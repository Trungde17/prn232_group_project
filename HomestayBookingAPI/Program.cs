
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
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

using Microsoft.AspNetCore.OData.Edm;
using Repositories;
using Repositories.BookingRepository;
using Repositories.HomeStayRepository;
using Repositories.RoomRepository;
using Services;
using Services.BookingServices;
using Services.HomestayServices;
using Services.RoomServices;
using System.Text;

using System.Text.Json;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers(options =>
{
    // Có thể thêm các cấu hình MVC khác ở đây nếu cần
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
})
.AddOData(opt => // Thêm .AddOData vào cùng chuỗi
{
    opt.AddRouteComponents("odata", GetEdmModel());
    opt.Select().Filter().Expand().Count().OrderBy().SetMaxTop(100);
});

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




//dki OData
IEdmModel GetEdmModel()
{

    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

    builder.EntitySet<Room>("Rooms");
    builder.EntitySet<Amenity>("Amenity");
    builder.EntitySet<PriceType>("PriceType");
    builder.EntitySet<BedType>("BedType");
    builder.EntitySet<Booking>("OdataBooking");

   
    // 1. Lấy EntityType của đối tượng (không phải EntitySet)
    var homestayEntityType = builder.EntityType<Homestay>();

    homestayEntityType.Collection.Function("MyHomestays")
        .ReturnsCollectionFromEntitySet<Homestay>("Homestays");
    // --- KẾT THÚC PHẦN CẦN THAY ĐỔI ---

    return builder.GetEdmModel();
}



builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IGenericRepository<Room>, RoomRepository>();
builder.Services.AddScoped<IGenericRepository<Homestay>, HomeStayRepository>();
builder.Services.AddScoped<IGenericRepository<Amenity>, AmenityRepository>();


builder.Services.AddScoped<IGenericRepository<PriceType>, PriceTypeRepository>();

builder.Services.AddScoped<IGenericRepository<BedType>, BedTypeRepository>();


builder.Services.AddScoped<IBedTypeService, BedTypeService>();

builder.Services.AddScoped<IPriceTypeService, PriceTypeService>();
builder.Services.AddScoped<IAmenityService, AmenityService>();


builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IHomestayService, HomestayService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
    options.ListenAnyIP(7220, listenOptions =>
    {
        listenOptions.UseHttps(); // nếu dùng HTTPS
    });
});


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
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
