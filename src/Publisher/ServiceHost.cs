using System;
using System.Linq;
using System.Threading;
using Castle.Core.Internal;
using DataContract;
using MassTransit;
using Timer = System.Timers.Timer;

namespace Publisher
{

    public class ServiceHost : IServiceInterface
    {

        private readonly IServiceBus bus;

        private readonly Timer timer;
        public ServiceHost(IServiceBus bus)
        {
            this.bus = bus;

            timer =  new Timer(1000 * 5) { AutoReset = false, Enabled = true};
            timer.Elapsed += (sender, eventArgs) => ProcessCarts();

        }

        public void Start()
        {
            timer.Start();
            Console.WriteLine("Starting....");
        }

        private void ProcessCarts()
        {
            Enumerable.Range(1, 100).ForEach(x =>
                              {
                                 Thread.Sleep(1000);
                                 bus.Publish(new CartClosedEvent{Data = x.ToString()});
                              });
        }

        public void Stop()
        {
            timer.Stop();
            //unsubscribeAction();
            bus.Dispose();
            //container.Dispose();
            Console.WriteLine("Stopping....");
        }


       
    }
}
