namespace SkyrimPotionRecipes
{
    using System.IO;
    using System.Web;
    using System.Text.Json;
    using System.Collections.Generic;

    public class PotionCreator
    {
        protected List<Ingredient> _Ingredients = new List<Ingredient>();
        public IReadOnlyList<Ingredient> Ingredients
        {
            get
            {
                return _Ingredients.AsReadOnly();
            }
        }

        public void AddIngredientFile(string path)
        {
            if (File.Exists(path))
                using (var file = File.OpenText(path))
                {
                    string? line = file.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        var headers = line.Split(",");
                        headers = headers.Select(x => x.Trim().TrimStart('"').TrimEnd('"').ToLower()).ToArray();
                        if (headers.Length > 5
                            && headers.Count(x => x == "id") == 1
                            && headers.Count(x => x == "name") == 1
                            && headers.Count(x => x == "effect1") == 1
                            && headers.Count(x => x == "effect2") == 1
                            && headers.Count(x => x == "effect3") == 1
                            && headers.Count(x => x == "effect4") == 1)
                        {
                            int ordinal_id = Array.IndexOf(headers, "id");
                            int ordinal_name = Array.IndexOf(headers, "name");
                            int ordinal_effect1 = Array.IndexOf(headers, "effect1");
                            int ordinal_effect2 = Array.IndexOf(headers, "effect2");
                            int ordinal_effect3 = Array.IndexOf(headers, "effect3");
                            int ordinal_effect4 = Array.IndexOf(headers, "effect4");

                            line = file.ReadLine();
                            while (!string.IsNullOrEmpty(line))
                            {
                                var fields = line.Split(",").Select(x => x.Trim().TrimStart('"').TrimEnd('"').ToLower()).ToArray();
                                _Ingredients.Add(
                                    new Ingredient()
                                    {
                                        ID = fields[ordinal_id],
                                        Name = fields[ordinal_name],
                                        Effect1 = fields[ordinal_effect1],
                                        Effect2 = fields[ordinal_effect2],
                                        Effect3 = fields[ordinal_effect3],
                                        Effect4 = fields[ordinal_effect4]
                                    }
                                );

                                line = file.ReadLine();
                            }
                        }
                    }
                }
        }

        public void AddEffectFile(string path)
        {
            //if (File.Exists(path))
            //    using (var file = File.OpenText(path))
            //    {
            //        string content = file.ReadToEnd();
            //        JsonSerializer.Deserialize(content, typeof(Ingredient));

            //    }
        }
    }
}
