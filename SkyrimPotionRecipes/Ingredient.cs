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

        public Ingredient Copy()
        {
            var copy = new Ingredient()
            {
                ID = this.ID,
                Name = this.Name,
                Effects = this.Effects.Select(e => e.Copy()).ToList()
            };
            return copy;
        }
    }
}
