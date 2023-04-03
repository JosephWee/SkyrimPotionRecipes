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

        public PotionRecipe Copy()
        {
            var copy = new PotionRecipe()
            {
                Effects = this.Effects.Select(e => e.Copy()).ToList(),
                Ingredients = this.Ingredients.Select(i => i.Copy()).ToList()
            };
            return copy;
        }
    }
}
