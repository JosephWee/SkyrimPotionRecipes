using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace SkyrimPotionRecipes.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PotionController : ControllerBase
    {
        private readonly ILogger<PotionController> _logger;

        public PotionController(ILogger<PotionController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Ingredients")]
        public ActionResult GetPotionIngredients()
        {
            var cache =
                HttpContext.RequestServices.GetRequiredService(typeof(IMemoryCache))
                as IMemoryCache;
            if (cache != null)
            {
                PotionCreator? potionCreator = null;
                if (cache.TryGetValue<PotionCreator>("PotionCreator", out potionCreator) && potionCreator != null)
                {
                    return new OkObjectResult(potionCreator.Ingredients.AsEnumerable<Ingredient>());
                }
            }

            return new NotFoundResult();
        }

        [HttpGet("Recipes")]
        public ActionResult GetPotionRecipes()
        {
            var cache =
                HttpContext.RequestServices.GetRequiredService(typeof(IMemoryCache))
                as IMemoryCache;
            if (cache != null)
            {
                PotionCreator? potionCreator = null;
                if (cache.TryGetValue<PotionCreator>("PotionCreator", out potionCreator) && potionCreator != null)
                {
                    return new OkObjectResult(potionCreator.GetPotionRecipeBook());
                }
            }

            return new NotFoundResult();
        }

        [HttpPost("Recipe")]
        public ActionResult GetPotionRecipe(string [] effects)
        {
            var cache =
                HttpContext.RequestServices.GetRequiredService(typeof(IMemoryCache))
                as IMemoryCache;

            if (cache != null)
            {
                PotionCreator? potionCreator = null;
                if (cache.TryGetValue<PotionCreator>("PotionCreator", out potionCreator) && potionCreator != null)
                {
                    try
                    {
                        var recipes = potionCreator.GetPotionRecipes(effects);
                        return new OkObjectResult(recipes);
                    }
                    catch
                    {
                    }
                }
            }

            return new NotFoundResult();
        }
    }
}