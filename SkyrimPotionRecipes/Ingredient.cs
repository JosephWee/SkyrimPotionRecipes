namespace SkyrimPotionRecipes
{
    public class Ingredient
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Effect1 { get; set; }
        public string Effect2 { get; set; }
        public string Effect3 { get; set; }
        public string Effect4 { get; set; }

        public Ingredient()
        {
            this.ID = string.Empty;
            this.Name = string.Empty;
            this.Effect1 = string.Empty;
            this.Effect2 = string.Empty;
            this.Effect3 = string.Empty;
            this.Effect4 = string.Empty;
        }
    }
}
