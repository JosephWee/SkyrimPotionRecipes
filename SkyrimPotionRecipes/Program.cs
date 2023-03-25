using NUnit.Framework;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

string rootPath = app.Environment.ContentRootPath;
if (!string.IsNullOrEmpty(rootPath))
{
    string pathAlchemyIngredientsCreationClub = Path.Combine(rootPath, "Data", "Alchemy Ingredients - Creation Club.csv");
    string pathAlchemyIngredientsStandard = Path.Combine(rootPath, "Data", "Alchemy Ingredients - Standard.csv");
    string pathEffectsJson = Path.Combine(rootPath, "Data", "effects.json");

    SkyrimPotionRecipes.PotionCreator potionCreator = new SkyrimPotionRecipes.PotionCreator();
    potionCreator.AddIngredientFile(pathAlchemyIngredientsCreationClub);
    potionCreator.AddIngredientFile(pathAlchemyIngredientsStandard);
    potionCreator.AddEffectFile(pathEffectsJson);

    IMemoryCache cache = app.Services.GetRequiredService<IMemoryCache>();
    cache.Set("PotionCreator", potionCreator);

    SkyrimPotionRecipes.PotionCreator? potionCreatorOut = null;
    cache.TryGetValue("PotionCreator", out potionCreatorOut);

    Assert.IsNotNull(potionCreatorOut);
    Assert.True(potionCreatorOut.Ingredients.Count > 0);
    Assert.True(potionCreatorOut.Effects.Keys.Count() > 0);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
