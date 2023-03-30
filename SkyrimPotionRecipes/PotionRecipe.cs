namespace SkyrimPotionRecipes
{
    public class PotionRecipe
    {
        public List<PotionEffect> Effects { get; set; }
        public List<Ingredient> Ingredients { get; set; }

        public PotionRecipe()
        {
            Effects = new List<PotionEffect>();
            Ingredients = new List<Ingredient>();
        }
    }
}
