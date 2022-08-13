using TalkHub.Models;

namespace TalkHub.Services
{
    public class SpeechProviderManager
    {

        private static readonly Dictionary<string, string> voiceProviderMapping = new Dictionary<string, string>
        {
            { "paul", "dectalk" }
        };

        private static Dictionary<Guid, SpeechResponse> jobs = new Dictionary<Guid, SpeechResponse>();

        private static Dictionary<string, IProviderWorker> workers = new Dictionary<string, IProviderWorker>();

        public SpeechResponse OpenRequest(SpeechRequest request)
        {
            SpeechResponse response = new SpeechResponse();
            response.RequestId = Guid.NewGuid();
            response.VoiceName = request.VoiceName;
            response.TextToSpeak = request.TextToSpeak;
            response.Provider = MapToProvider(request);
            if (response.Provider != null)
            {
                response.Status = RequestStatus.Accepted;
                jobs.Add(response.RequestId, response);
                EnqueueWithProviderWorker(ref response);
            } else
            {
                response.Status = RequestStatus.Failure;
                response.ErrorDetail = ErrorDetail.NoProvider;
                jobs.Add(response.RequestId, response);
            }
            return response;

        }

        public SpeechResponse GetRequest(Guid guid)
        {
            return jobs.GetValueOrDefault(guid, null);
        }

        private string MapToProvider(SpeechRequest request)
        {
            return voiceProviderMapping.GetValueOrDefault(request.VoiceName.ToLower(), null);
        }

        private void EnqueueWithProviderWorker(ref SpeechResponse response)
        {
            workers[response.Provider].AddJob(ref response);
        }
    }
}
