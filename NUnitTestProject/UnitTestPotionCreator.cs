namespace NUnitTestProject
{
    using SkyrimPotionRecipes;
    using SkyrimPotionRecipes.Extensions;
    using System.Diagnostics;

    public class Tests
    {
        protected PotionCreator potionCreator = new PotionCreator();
        protected string dataFilePath = @"..\..\..\..\SkyrimPotionRecipes\Data";
        protected string[] ingredientFileNames =
            new string[]
            {
                "Alchemy Ingredients - Creation Club.csv",
                "Alchemy Ingredients - Standard.csv"
            };
        protected string effectsFileName = "effects.json";

        [SetUp]
        public void Setup()
        {
            Assert.IsNotNull(potionCreator);
            
            if (potionCreator.Ingredients == null || potionCreator.Ingredients.Count == 0)
            {
                if (Directory.Exists(dataFilePath))
                {
                    foreach (string ingredientFileName in ingredientFileNames)
                    {
                        string filePath = Path.Combine(dataFilePath, ingredientFileName);
                        if (File.Exists(filePath))
                            potionCreator.AddIngredientFile(filePath);
                    }
                }
            }

            Assert.True(
                potionCreator.Ingredients != null
                && potionCreator.Ingredients.Count > 0
            );

            if (potionCreator.Effects == null || potionCreator.Effects.Count == 0)
            {
                string filePath = Path.Combine(dataFilePath, effectsFileName);
                if (File.Exists(filePath))
                    potionCreator.AddEffectFile(filePath);
            }

            Assert.True(
                potionCreator.Effects != null
                && potionCreator.Effects.Count(x => x.Value.All(y => !string.IsNullOrWhiteSpace(y.Name))) > 0
            );
        }

        [Test]
        public void TestGetPotionRecipeBook()
        {
            var potionRecipeBook = potionCreator.GetPotionRecipeBook();
            Assert.IsNotNull(potionRecipeBook);
            Assert.Greater(potionRecipeBook.Keys.Count, 0);
            var recipes = potionRecipeBook.SelectMany(x => x.Value.Recipes);
            Assert.IsTrue(recipes.Count(x => x.Effects.Count > 0) == recipes.Count());
            Assert.IsTrue(recipes.Count(x => x.Effects.Count > 0) > 0);
            Assert.IsTrue(recipes.Count(x => x.Ingredients.Count > 0) > 0);
            Assert.IsTrue(recipes.Count(x => x.Ingredients.Count > 0) <= recipes.Count(x => x.Effects.Count > 0));
        }
    }
}