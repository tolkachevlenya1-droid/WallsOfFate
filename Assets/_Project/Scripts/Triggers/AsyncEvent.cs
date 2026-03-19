using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Core
{
    public delegate Task AsyncEventHandler<T>(T args);

    public class AsyncEvent<T>
    {
        private readonly List<AsyncEventHandler<T>> _subscribers = new();

        public void Subscribe(AsyncEventHandler<T> handler) => _subscribers.Add(handler);
        public void Unsubscribe(AsyncEventHandler<T> handler) => _subscribers.Remove(handler);

        public async Task InvokeAsync(T args)
        {
            foreach (var handler in _subscribers)
            {
                await handler(args);
            }
        }
    }
}