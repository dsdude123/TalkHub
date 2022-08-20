using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TalkHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeechDataController : ControllerBase
    {
        [HttpGet(Name = "SpeechData")]
        public async Task<IActionResult> GetSpeechData(Guid guid)
        {
            string path = Environment.CurrentDirectory + "\\" + guid.ToString() + ".wav";
            if (System.IO.File.Exists(path))
            {
                return File(System.IO.File.ReadAllBytes(path), "audio/wav");
            } else
            {
                return NotFound();
            }
        }
    }
}
