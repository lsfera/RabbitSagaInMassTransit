using System;
using MassTransit;

namespace Saga
{

    public class ServiceHost : IServiceInterface
    {

        private readonly IServiceBus bus;


        public ServiceHost(IServiceBus bus)
        {
            this.bus = bus;
        }

        public void Start()
        {
            Console.WriteLine("Starting....");
        }
        
        public void Stop()
        {
            bus.Dispose();
            Console.WriteLine("Stopping....");
        }


       
    }
}
