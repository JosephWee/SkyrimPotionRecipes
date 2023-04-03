namespace SkyrimPotionRecipes
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Web;
    using Microsoft.Extensions.Configuration.EnvironmentVariables;
    using NUnit.Framework;
    using SkyrimPotionRecipes.Extensions;

    public class PotionCreator
    {
        protected List<Ingredient> _Ingredients = new List<Ingredient>();
        public IReadOnlyCollection<Ingredient> Ingredients
        {
            get
            {
                return _Ingredients.AsReadOnly();
            }
        }

        protected ImmutableDictionary<string, IReadOnlyCollection<PotionEffect>> _Effects = new Dictionary<string,IReadOnlyCollection<PotionEffect>>().ToImmutableDictionary();
        public ImmutableDictionary<string, IReadOnlyCollection<PotionEffect>> Effects
        {
            get
            {
                return _Effects;
            }
        }

        protected string EffectNameProcessing(string EffectName)
        {
            return string.Join(
                ' ',
                Regex.Split(EffectName.Trim(), @"\s+")
                .Select(x => x.ToLower())
                .ToArray()
            );
        }

        protected int Factorial(int n)
        {
            // n x (n - 1)!
            // https://en.wikipedia.org/wiki/Factorial

            if (n == 1)
                return 1;

            return n * Factorial(n - 1);
        }

        protected List<List<PotionEffect>> Permutations(IReadOnlyCollection<PotionEffect> potionEffects)
        {
            var effects = new Queue<PotionEffect>(potionEffects.Distinct());

            // Number of Permutations of N objects is N! (N Factorial)
            // https://en.wikipedia.org/wiki/Permutation

            int numberOfPermuations = Factorial(effects.Count);

            List<List<PotionEffect>> permutationsByShifting = new List<List<PotionEffect>>();
            for (int i = 0; i < effects.Count; i++)
            {
                permutationsByShifting.Add(effects.ToList());
                effects.Enqueue(effects.Dequeue());
            }

            if (permutationsByShifting.Count >= numberOfPermuations)
                return permutationsByShifting;

            Dictionary<string,List<PotionEffect>> permDict = new Dictionary<string,List<PotionEffect>>();
            foreach (var permutation in permutationsByShifting)
            {
                string permKey = string.Join("-", permutation.Select(x => x.Name));

                if (permDict.ContainsKey(permKey))
                    continue;
                else
                    permDict.Add(permKey,permutation);

                for (int i = 1; i < permutation.Count; i++)
                {
                    var newPermutation = new List<PotionEffect>();
                    newPermutation.AddRange(permutation);
                    newPermutation[i - 1] = permutation[i];
                    newPermutation[i] = permutation[i - 1];

                    string npermKey = string.Join("-", newPermutation.Select(x => x.Name));

                    if (permDict.ContainsKey(npermKey))
                        continue;
                    else
                        permDict.Add(npermKey, newPermutation);
                }
            }

            List<List<PotionEffect>> result = new List<List<PotionEffect>>();
            foreach (var permutation in permDict.ToList())
            {
                result.Add(permutation.Value);
            }

            return result;
        }

        protected List<List<PotionEffect>> Combinations(IReadOnlyCollection<PotionEffect> potionEffects, int choose)
        {
            List<PotionEffect> effects = potionEffects.Distinct().OrderBy(x => x.Name).ToList();

            if (potionEffects.Count > effects.Count)
                throw new ArgumentException("There should not be duplicates in the items to choose from.");

            if (choose > effects.Count)
                throw new ArgumentException("The number of items to pick is larger than the number of unique items to choose from.");

            var results = new List<List<PotionEffect>>();
            for (int i = 0; (i + choose) <= effects.Count; i++)
            {
                var permutations = Permutations(potionEffects.Skip(i).Take(choose).ToList());
                var uniqueCombinations = new Dictionary<string, List<PotionEffect>>();

                foreach (var permutation in permutations)
                {
                    var oPermutation = permutation.OrderBy(x => x.Name).ToList();
                    string key = string.Join("-", oPermutation.Select(x => x.Name));
                    if (!uniqueCombinations.ContainsKey(key))
                    {
                        uniqueCombinations.Add(key, oPermutation);
                    }
                }

                results.AddRange(uniqueCombinations.Select(x => x.Value));
            }

            return results;
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
                                var newIngredient =
                                    new Ingredient()
                                    {
                                        ID = fields[ordinal_id],
                                        Name = fields[ordinal_name]
                                    };
                                newIngredient.Effects.Add(new PotionEffect(EffectNameProcessing(fields[ordinal_effect1])));
                                newIngredient.Effects.Add(new PotionEffect(EffectNameProcessing(fields[ordinal_effect2])));
                                newIngredient.Effects.Add(new PotionEffect(EffectNameProcessing(fields[ordinal_effect3])));
                                newIngredient.Effects.Add(new PotionEffect(EffectNameProcessing(fields[ordinal_effect4])));

                                _Ingredients.Add(newIngredient);
                                
                                line = file.ReadLine();
                            }
                        }
                    }

                    _Ingredients = _Ingredients.OrderBy(x => x.Name).ToList();
                }
        }

        public void AddEffectFile(string path)
        {
            if (File.Exists(path))
                using (var file = File.OpenText(path))
                {
                    string content = file.ReadToEnd();
                    Dictionary<string, string[]>? dictionary =
                        JsonSerializer.Deserialize(
                            content,
                            typeof(Dictionary<string,string[]>)
                        ) as Dictionary<string, string[]>;

                    if (dictionary != null)
                    {
                        Dictionary<string, IReadOnlyCollection<PotionEffect>> effects =
                            new Dictionary<string, IReadOnlyCollection<PotionEffect>>();

                        foreach (KeyValuePair<string, string[]> item in dictionary)
                        {
                            List<string> list = item.Value.Select(x => x.Trim().ToLower()).ToList();
                            effects.Add(item.Key, list.Select(x => new PotionEffect(EffectNameProcessing(x))).ToList().AsReadOnly());
                        }

                        _Effects = effects.ToImmutableDictionary<string, IReadOnlyCollection<PotionEffect>>();
                    }
                }
        }

        protected List<PotionRecipe> ValidateRecipes(List<PotionRecipe> recipes, IReadOnlyCollection<PotionEffect> effectsWanted)
        {
            // Remove duplicate combinations from chains and incomplete chains

            var validRecipes = new List<PotionRecipe>();

            foreach (var recipe in recipes)
            {
                var trecipe =
                    new PotionRecipe()
                    {
                        Effects = recipe.Effects,
                        Ingredients = recipe.Ingredients.Distinct().OrderBy(x => x.Name).ToList()
                    };

                if (!validRecipes.Any(x => x.Ingredients.AreListEqual<Ingredient,string>(trecipe.Ingredients, y => y.ID)))
                {
                    bool iscomplete = true;

                    foreach (var effectWanted in effectsWanted)
                    {
                        var matchingIngredients =
                            trecipe
                            .Ingredients
                            .Where(i => i.Effects.Any(e => e.Name == effectWanted.Name))
                            .ToList();

                        if (matchingIngredients.Count() < 2)
                        {
                            iscomplete = false;
                            break;
                        }
                    }

                    if (iscomplete && trecipe.Ingredients.Count > 1)
                        validRecipes.Add(trecipe);
                }
            }

            return validRecipes;
        }

        public PotionRecipes GetPotionRecipes(string[] desiredEffectNames)
        {
            IReadOnlyCollection<PotionEffect> effectsWanted =
                desiredEffectNames
                .Select(x => EffectNameProcessing(x))
                .Distinct()
                .Select(x => new PotionEffect(x))
                .ToList()
                .AsReadOnly();

            bool effectFound = false;
            foreach (PotionEffect effectWanted in effectsWanted)
            {
                effectFound = this._Effects.Any(x => x.Value.Any(y => y.Name == effectWanted.Name));
                if (effectFound)
                    break;
            }

            if (!effectFound)
                throw new ArgumentException("desiredEffects must be the list of known potion effects");

            if (effectsWanted.Count < 1)
                throw new ArgumentException("desiredEffects must have at least 1 item");

            if (effectsWanted.Count > 4)
                throw new ArgumentException("desiredEffects supports at most 4 items");

            var potionRecipes = new PotionRecipes() { Effects = effectsWanted.Select(e => e.Copy()).ToList() };

            if (effectsWanted.Count == 1)
            {
                var effect = effectsWanted.First();
                PotionRecipe recipe = null;
                for (int ingIndex = 0; ingIndex < _Ingredients.Count; ingIndex++)
                {
                    var ingredient = _Ingredients[ingIndex];
                    if (ingredient.Effects.Any(x => x.Name == effect.Name))
                    {
                        if (recipe == null)
                            recipe = new PotionRecipe() { Effects = effectsWanted.Select(e => e.Copy()).ToList() };
                        recipe.Ingredients.Add(ingredient);
                    }
                }
                if (recipe != null)
                    potionRecipes.Recipes.Add(recipe);
            }
            else
            {
                var effectPermutations = Permutations(effectsWanted);
                
                for (int permIndex = 0; permIndex < effectPermutations.Count; permIndex++)
                {
                    var potionEffects = effectPermutations[permIndex];
                    var A = new List<PotionRecipe>();

                    for (int linkIndex = 0; linkIndex < potionEffects.Count; linkIndex++)
                    {
                        var effect1 = linkIndex == 0 ? potionEffects[potionEffects.Count - 1] : potionEffects[linkIndex - 1];
                        var effect2 = potionEffects[linkIndex];

                        var B = new List<PotionRecipe>();

                        for (int ingredientIndex = 0; ingredientIndex < _Ingredients.Count; ingredientIndex++)
                        {
                            var ingredient = _Ingredients[ingredientIndex];
                            if (ingredient.Effects.Any(x => x.Name == effect1.Name)
                                && ingredient.Effects.Any(x => x.Name == effect2.Name))
                            {
                                if (linkIndex == 0)
                                {
                                    B.Add(
                                        new PotionRecipe()
                                        {
                                            Effects = potionEffects.Select(e => e.Copy()).ToList(),
                                            Ingredients = new List<Ingredient>() { ingredient.Copy() }
                                        }
                                    );
                                }
                                else
                                {
                                    foreach (var recipe in A.ToList())
                                    {
                                        if (!recipe.Ingredients.Any(i => i.Name == ingredient.Name))
                                        {
                                            var copy = recipe.Copy();
                                            copy.Ingredients.Add(ingredient.Copy());
                                            B.Add(copy);
                                        }
                                    }
                                }
                            }
                        }

                        A = B.Select(pr => pr.Copy()).ToList();
                    }

                    potionRecipes.Recipes.AddRange(A);
                }
            }

            var validRecipes = ValidateRecipes(potionRecipes.Recipes, effectsWanted);
            potionRecipes.Recipes = validRecipes;

            return potionRecipes;
        }

        public Dictionary<string, PotionRecipes> GetPotionRecipeBook()
        {
            var potions = new Dictionary<string, PotionRecipes>();

            // Single effect potions
            var effectsKeys = new string[] { "Cure", "Health", "Magicka", "Stamina", "Resist", "Others","Poison" };
            foreach (var k in effectsKeys)
            {
                string suffix = "Potion";
                if (k == "Poison")
                    suffix = "Poison";

                var effectsList = _Effects[k];
                foreach (var effect in effectsList)
                {
                    var pname = $"{effect.Name.ToTitleCase()} {suffix}";
                    var recipes = GetPotionRecipes(new string[] { effect.Name });
                    potions.Add(pname, recipes);
                }
            }

            // Multiple effect potions
            var potionGroups = new List<List<string>>();
            potionGroups.Add(new List<string>() { "Cure" });
            potionGroups.Add(new List<string>() { "Health" });
            potionGroups.Add(new List<string>() { "Magicka" });
            potionGroups.Add(new List<string>() { "Stamina" });

            foreach (var potionGroup in potionGroups)
            {
                var effectsList = new List<PotionEffect>();

                foreach (var k in potionGroup)
                {
                    effectsList.AddRange(_Effects[k].ToArray());
                }

                var combinations = Combinations(effectsList, Math.Min(4, effectsList.Count()));

                foreach (var combination in combinations)
                {
                    var pname = $"{string.Join(" | ", combination.Select(x => x.Name.ToTitleCase()))} Potion";
                    if (!potions.ContainsKey(pname))
                        potions.Add(pname, GetPotionRecipes(combination.Select(x => x.Name).ToArray()));
                }
            }

            return potions;
        }
    }
}
