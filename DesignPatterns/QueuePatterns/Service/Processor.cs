using Receiver;

namespace Service
{
    public abstract class Processor
    {
        private readonly MessageReceiver _receiver;
        public Processor(MessageReceiver receiver)
        {
            _receiver = receiver;
        }

        public void Start()
        {
            _receiver.Start();
            _receiver.MessageReceived += Process;
        }

        public void Stop()
        {
            _receiver.Stop();
        }

        public abstract void Process(object sender, MessageReceivedArgs argument);
    }
}