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

        public PotionRecipes Copy()
        {
            var copy = new PotionRecipes()
            {
                Effects = this.Effects.Select(e => e.Copy()).ToList(),
                Recipes = this.Recipes.Select(r => r.Copy()).ToList()
            };
            return copy;
        }
    }
}
