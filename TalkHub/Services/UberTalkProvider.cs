using Newtonsoft.Json;
using System.Timers;
using TalkHub.Models;
using Uberduck.NET;
using Uberduck.NET.Keys;
using Timer = System.Timers.Timer;

namespace TalkHub.Services
{
    public class UberTalkProvider : IProviderWorker
    {

        private UberduckClient client;

        private Timer workTrigger = new Timer(1000);

        private Queue<SpeechResponse> workQueue = new Queue<SpeechResponse>();

        public UberTalkProvider()
        {
            UberduckKeys keys = JsonConvert.DeserializeObject<UberduckKeys>(File.ReadAllText("uberduck.json"));
            client = new UberduckClient(keys);
            workTrigger.Elapsed += RunWorker;
            workTrigger.Enabled = true;
        }

        public void AddJob(ref SpeechResponse response)
        {
            workQueue.Enqueue(response);
        }

        public void RunWorker(object source, ElapsedEventArgs e)
        {
            // New job flow
            if (workQueue.Count > 0)
            {
                var job = workQueue.Dequeue();
                try
                {
                    job.Status = RequestStatus.QueuedAtProvider;
                    var result = client.GenerateVoiceAsync(job.TextToSpeak, job.VoiceName).Result;
                    result.SaveAudioFileAsync($"{job.RequestId}");
                    job.ProviderRequestId = result.UUID;
                    job.Status = RequestStatus.Done;

                } catch
                {
                    job.Status = RequestStatus.Failure;
                    job.ErrorDetail = ErrorDetail.Unknown;
                }
            }           
            
        }
    }
}
