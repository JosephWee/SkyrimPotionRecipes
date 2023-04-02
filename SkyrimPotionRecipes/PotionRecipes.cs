namespace SkyrimPotionRecipes
{
    public class PotionRecipes
    {
        public List<PotionEffect> Effects { get; set; }
        public List<PotionRecipe> Recipes { get; set; }

        public PotionRecipes()
        {
            Effects = new List<PotionEffect>();
            Recipes = new List<PotionRecipe>();
        }
    }
}
