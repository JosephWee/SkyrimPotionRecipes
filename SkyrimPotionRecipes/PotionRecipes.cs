namespace SkyrimPotionRecipes
{
    public class PotionRecipes
    {
        public List<PotionEffect> Effects
        {
            get
            {
                if (Recipes.Any())
                    return Recipes[0].Effects;

                return new List<PotionEffect>();
            }
        }
        public List<PotionRecipe> Recipes { get; set; }

        public PotionRecipes()
        {
            Recipes = new List<PotionRecipe>();
        }
    }
}
