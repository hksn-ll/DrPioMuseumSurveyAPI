using Microsoft.EntityFrameworkCore;
using DrPioMuseumSurveyAPI.Models;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.MapControllers();

app.MapPost("/api/submit", async (AppDbContext db, SurveyDto input) =>
{
    var newResponse = new SurveyResponse
    {
        FirstName = input.FirstName,
        LastName = input.LastName,
        Email = input.Email,
        StreetAddress = input.StreetAddress,
        District = input.District,
        Barangay = input.Barangay,
        OtherLocation = input.OtherLocation,
        AgeRange = input.AgeRange,
        Gender = input.Gender,
        RatingsRaw = string.Join(",", input.Ratings),
        LikedMost = input.LikedMost,
        Improvements = input.Improvements,
        Recommend = input.Recommend,
        Comments = input.Comments,
        SubmittedAt = DateTime.Now
    };

    db.Responses.Add(newResponse);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Survey saved successfully!" });
});

app.MapGet("/api/questions", async (AppDbContext db) =>
{
    return await db.Questions
        .Where(q => q.IsActive)
        .OrderBy(q => q.OrderIndex)
        .ToListAsync();
});

app.MapPost("/api/questions", async (AppDbContext db, Question newQ) =>
{
    int nextOrder = await db.Questions.CountAsync() + 1;
    newQ.OrderIndex = nextOrder;
    db.Questions.Add(newQ);
    await db.SaveChangesAsync();
    return Results.Ok(newQ);
});

app.MapDelete("/api/questions/{id}", async (AppDbContext db, int id) =>
{
    var q = await db.Questions.FindAsync(id);
    if (q == null) return Results.NotFound();
    q.IsActive = false;
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("/api/stats", async (AppDbContext db) =>
{
    var allData = await db.Responses.ToListAsync();
    var total = allData.Count;
    var district1 = allData.Count(r => r.District == "1");
    var district2 = allData.Count(r => r.District == "2");
    var outside = allData.Count(r => !string.IsNullOrEmpty(r.OtherLocation) || (r.District != "1" && r.District != "2"));

    double avgRating = 0;
    if (total > 0)
    {
        var allRatings = allData
            .SelectMany(r => r.RatingsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse))
            .ToList();

        if (allRatings.Any())
            avgRating = allRatings.Average();
    }

    return Results.Ok(new
    {
        TotalResponses = total,
        District1 = district1,
        District2 = district2,
        Outside = outside,
        AverageSatisfaction = Math.Round(avgRating, 1)
    });
});

app.MapGet("/api/responses", async (AppDbContext db) =>
{
    var responses = await db.Responses.OrderByDescending(r => r.SubmittedAt).ToListAsync();
    return Results.Ok(responses);
});

app.MapDelete("/api/submit/{id}", async (AppDbContext db, int id) =>
{
    var response = await db.Responses.FindAsync(id);
    if (response == null) return Results.NotFound();
    db.Responses.Remove(response);
    await db.SaveChangesAsync();
    return Results.Ok();
});
// --- START OF AUTO-FIX CODE ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try 
    {
        var context = services.GetRequiredService<AppDbContext>();
        // This command forces the database to create all missing tables immediately
        context.Database.Migrate();

        // OPTIONAL: Create a default Admin user so you can actually log in
        if (!context.Admins.Any())
        {
            // Note: Ensure you have "using DrPioMuseumSurveyAPI.Models;" at the top
            // and that BCrypt is installed. If this part errors, remove the seeding logic.
            context.Admins.Add(new Admin 
            { 
                Username = "admin", 
                // This creates a password "admin123"
                Password = BCrypt.Net.BCrypt.HashPassword("admin123") 
            });
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}
// --- END OF AUTO-FIX CODE ---


app.Run();

public class SurveyDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? StreetAddress { get; set; }
    public string? District { get; set; }
    public string? Barangay { get; set; }
    public string? OtherLocation { get; set; }
    public string AgeRange { get; set; }
    public string? Gender { get; set; }
    public List<int> Ratings { get; set; } = new List<int>();
    public string? LikedMost { get; set; }
    public string? Improvements { get; set; }
    public string? Recommend { get; set; }
    public string? Comments { get; set; }
}