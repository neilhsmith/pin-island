using Linkster.Api.Extensions.Application;
using PinIsland.Api.Extensions.Services;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureServices();

var app = builder.Build();

//using var scope = app.Services.CreateScope();
if (builder.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}
else
{
  app.UseExceptionHandler("/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSwaggerExtension(builder.Configuration, builder.Environment);

app.Run();
