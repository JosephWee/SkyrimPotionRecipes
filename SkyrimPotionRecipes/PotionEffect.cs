namespace SkyrimPotionRecipes
{
    public class PotionEffect
    {
        protected string _Name = string.Empty;
        public string Name { get { return _Name; } }

        public PotionEffect(string name)
        {
            this._Name = name;
        }

        public override string ToString()
        {
            return this._Name;
        }
    }
}
