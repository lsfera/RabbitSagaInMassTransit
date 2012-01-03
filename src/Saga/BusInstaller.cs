using System;
using System.Configuration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MassTransit;
using MassTransit.Advanced;

namespace Saga
{
    public class BusInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IServiceBus>()
                                   .UsingFactoryMethod(
                                       () => ServiceBusFactory.New(
                                           sbc =>
                                               {
                                                   sbc.UseRabbitMq();
                                                   sbc.UseRabbitMqRouting();
                                                   sbc.SetReceiveTimeout(TimeSpan.FromMinutes(2));
                                                   sbc.ReceiveFrom(ConfigurationManager.AppSettings["serviceBus"]);
                                                   sbc.Subscribe(subs => subs.LoadFrom(container));
                                                   sbc.Validate();
                                               })).LifeStyle.Singleton);
        }
    }
}