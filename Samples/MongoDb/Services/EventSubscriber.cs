using System;
using MongoDB.Driver.Core.Events;

namespace Joker.AspNetCore.MongoDb.Services
{
  public class EventSubscriber : IEventSubscriber
  {
    private readonly ReflectionEventSubscriber subscriber;

    public EventSubscriber()
    {
      subscriber = new ReflectionEventSubscriber(this);
    }

    public bool TryGetEventHandler<TEvent>(out Action<TEvent> handler)
    {
      return subscriber.TryGetEventHandler(out handler);
    }

    public void Handle(CommandStartedEvent commandStartedEvent)
    {

    }

    public void Handle(CommandSucceededEvent commandSucceededEvent)
    {

    }

    public void Handle(CommandFailedEvent commandFailedEvent)
    {

    }
  }
}