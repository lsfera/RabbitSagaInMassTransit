using System;
using System.Configuration;
using System.Diagnostics;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Magnum;
using Magnum.StateMachine;
using MassTransit.Saga;
using Topshelf;
using log4net.Config;

namespace Saga
{
    public class Program
    {

        public static void Main(string[] args)
        {
            try
            {
                XmlConfigurator.Configure();
                var container = BootstrapContainer().Install(FromAssembly.This());

                HostFactory.Run(c =>
                                    {
                                        c.SetServiceName(ConfigurationManager.AppSettings["ServiceName"]);
                                        c.SetDisplayName(ConfigurationManager.AppSettings["DisplayName"]);
                                        c.SetDescription(ConfigurationManager.AppSettings["Description"]);
                                        c.RunAsLocalSystem();

                                        DisplayStateMachine();

                                        c.Service<ServiceHost>(s =>
                                                                       {
                                                                           s.ConstructUsing(builder =>container.Resolve<ServiceHost>());
                                                                           s.WhenStarted(o => o.Start());
                                                                           s.WhenStopped(o => o.Stop());
                                                                       });
                                    });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private static IWindsorContainer BootstrapContainer()
        {
            IWindsorContainer container = new WindsorContainer();
            container.Register(
                                Component.For(typeof(ISagaRepository<>))
                                    .ImplementedBy(typeof(InMemorySagaRepository<>))
                                    .LifeStyle.Singleton,
                                    Component.For<FrontEndSaga>(),
                                    Component.For<ServiceHost>());
            
            return container;
        } 

        private static void DisplayStateMachine()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            StateMachineInspector.Trace(new FrontEndSaga(CombGuid.Generate()));
        }



    }
}
