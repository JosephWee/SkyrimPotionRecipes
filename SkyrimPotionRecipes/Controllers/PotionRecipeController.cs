using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("List")]
        public IEnumerable<string> GetPotionList()
        {
            var retVal = new string[] { "Potion of Restore Health", "Potion of Restore Magicka" };
            return retVal.AsEnumerable<string>();
        }
    }
}