using System.Configuration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Magnum.Extensions;
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
                                                   sbc.ReceiveFrom(ConfigurationManager.AppSettings["serviceBus"]);//testUser:topSecret@localhost:5672
                                                   sbc.Subscribe(subs => subs.LoadFrom(container));
                                                   
                                                   sbc.Validate();

                                               })).LifeStyle.Singleton);
        }
    }
}