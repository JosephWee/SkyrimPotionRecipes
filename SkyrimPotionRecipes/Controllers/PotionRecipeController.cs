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

        [HttpGet("List")]
        public IEnumerable<string> GetPotionList()
        {
            var retVal = new string[] { "Potion of Restore Health", "Potion of Restore Magicka" };
            return retVal.AsEnumerable<string>();
        }
    }
}