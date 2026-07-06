using inventory_v2.Data;
using inventory_v2.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var conString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(conString, ServerVersion.AutoDetect(conString)));

builder.Services.AddControllers();
builder.Services.AddScoped<ITypesService, TypesService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserLogService, UserLogService>();
builder.Services.AddScoped<IMovementService, MovementService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IAssetsService, AssetsService>();
builder.Services.AddScoped<IChildAssetsService, ChildAssetsService>();
builder.Services.AddScoped<IMovementCartService, MovementCartService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors(x => x
            .SetIsOriginAllowed(origin => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());

app.MapControllers();

app.Run();
