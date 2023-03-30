namespace SkyrimPotionRecipes
{
    public class Ingredient
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public List<PotionEffect> Effects { get; set; }

        public Ingredient()
        {
            this.ID = string.Empty;
            this.Name = string.Empty;
            this.Effects = new List<PotionEffect>();
        }
    }
}
