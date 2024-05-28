using blazor_demo.Components;
using DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var dbConnectionString = builder.Configuration.GetConnectionString("Blog");
builder.Services.AddDbContext<BloggingContext>(optionsBuilder =>
    optionsBuilder.UseNpgsql(dbConnectionString)
        .UseSnakeCaseNamingConvention()
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


// Migrate latest database changes during startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<BloggingContext>();
    dbContext.Database.Migrate();


    if (!dbContext.Blogs.Any())
    {
        dbContext.Blogs.AddRange([
            new Blog { Url = "https://example.com/cats", Posts = [
                new Post { Title = "Cat Post 1", Content = "Cat Content 1" },
                new Post { Title = "Cat Post 2", Content = "Cat Content 2" }
            ]},
            new Blog { Url = "https://example.com/dogs", Posts = [
                new Post { Title = "Dog Post 1", Content = "Dog Content 1" },
                new Post { Title = "Dog Post 2", Content = "Dog Content 2" }
            ]}
        ]);
        dbContext.SaveChanges();
    }
}

app.Run();