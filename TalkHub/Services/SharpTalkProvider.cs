using System.Diagnostics;
using System.Timers;
using TalkHub.Models;
using Timer = System.Timers.Timer;

namespace TalkHub.Services
{
    public class SharpTalkProvider : IProviderWorker
    {
        private Timer workTrigger = new Timer(1000);

        Queue<SpeechResponse> workQueue = new Queue<SpeechResponse>();

        public SharpTalkProvider()
        {
            workTrigger.Elapsed += RunWorker;
            workTrigger.Enabled = true;
        }

        public void AddJob(ref SpeechResponse response)
        {
            workQueue.Enqueue(response);
        }

        public void RunWorker(object source, ElapsedEventArgs e)
        {
            if (workQueue.Count > 0) 
            {
                var job = workQueue.Dequeue();
                job.ProviderRequestId = job.RequestId.ToString();
                var process = CreateTTS(job.TextToSpeak, job.RequestId.ToString());
                process.WaitForExit();
                File.Move($"{Environment.CurrentDirectory}\\{job.RequestId.ToString()}.wav", $"{Environment.CurrentDirectory}\\data\\{job.RequestId.ToString()}.wav");
            } 
        }

        private Process CreateTTS(string text, string guid)
        {
            if (!File.Exists("SharpTalkGenerator.exe"))
            {
                throw new FileNotFoundException("SharpTalk is not installed.");
            }
            return Process.Start(new ProcessStartInfo
            {
                FileName = "SharpTalkGenerator",
                Arguments = $"--text {text} --output {guid}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }
    }
}
