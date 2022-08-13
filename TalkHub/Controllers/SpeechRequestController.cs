using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.Swagger.Annotations;
using TalkHub.Models;
using TalkHub.Services;

namespace TalkHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeechRequestController : ControllerBase
    {
        private static SpeechProviderManager manager = new SpeechProviderManager();

        [HttpGet(Name = "SpeechRequest")]
        [Produces(typeof(SpeechResponse))]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, Description = "Get status of a submitted speech request.", Type = typeof(SpeechResponse))]
        public async Task<IActionResult> GetSpeechRequest(Guid guid)
        {
            var response = manager.GetRequest(guid);

            if (response == null)
            {
                return NotFound();
            } else
            {
                return Ok(response);
            }
        }

        [HttpPost(Name = "SpeechRequest")]
        [Produces(typeof(SpeechResponse))]
        [SwaggerResponse(System.Net.HttpStatusCode.Accepted, Description = "Create a speech request.", Type = typeof(SpeechResponse))]
        public async Task<IActionResult> CreateSpeechRequest(SpeechRequest request)
        {
            var response = manager.OpenRequest(request);

            if (response.Status.Equals(RequestStatus.Failure) && response.ErrorDetail.Equals(ErrorDetail.NoProvider))
            {
                return BadRequest(response);
            }
            else
            {
                return Accepted(response);
            }
        }
    }
}
