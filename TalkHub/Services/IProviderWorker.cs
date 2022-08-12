using TalkHub.Models;

namespace TalkHub.Services
{
    public interface IProviderWorker
    {
        public abstract void AddJob(ref SpeechResponse response);

        public abstract void RunWorker();
    }
}
