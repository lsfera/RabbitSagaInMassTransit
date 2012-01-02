using System;
using DataContract;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using log4net;

namespace Saga
{

    public class FrontEndSaga :
        SagaStateMachine<FrontEndSaga>,
        ISaga
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(FrontEndSaga));

        static FrontEndSaga()
        {
            Define(() =>
                       {
                           Correlate(NewClosedCart)
                                              .By((saga, message) => saga.OrderNumber == message.Data);
                           Correlate(OrderReady)
                                              .By((saga, message) => saga.OrderNumber == message.Data.OrderNumber);
                           Initially(
                               When(NewClosedCart)
                                   .Then((saga, message) => saga.ProcessNerClosedCart(message))
                                   .TransitionTo(WaitingForOrder));
                           
                           During(WaitingForOrder,
                               When(OrderReady)
                               .Then((saga, message) => saga.NotifyOrderIsCompleted(message))
                               .TransitionTo(Completed));


                       });

        }

        protected FrontEndSaga(){}

        public virtual string OrderNumber { get; set; }

        private void NotifyOrderIsCompleted(OrderReadyEvent message)
        {
            log.InfoFormat("Received OrderReadyEvent for order: {0}", message.Data);
        }


        private void ProcessNerClosedCart(CartClosedEvent message)
        {
            OrderNumber = message.Data;
            log.InfoFormat("Received CartClosedEvent for cart: {0}", message.Data);
        }


        public FrontEndSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }


        public static State Initial { get; set; }
        public static State WaitingForOrder { get; set; }
        public static State Completed { get; set; }


        public static Event<CartClosedEvent> NewClosedCart { get; set; }
        public static Event<OrderReadyEvent> OrderReady { get; set; }




        public virtual Guid CorrelationId { get; set; }

        public virtual IServiceBus Bus { get; set; }
    }
} 

