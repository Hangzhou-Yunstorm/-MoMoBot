using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Core
{
    public class TurnContextStateCollection : Dictionary<string, object>, IDisposable
    {
        public TurnContextStateCollection()
        {

        }

        public T Get<T>(string key) where T : class
        {
            MoMoBotAssert.KeyNotNullOrEmpty(key);

            if (TryGetValue(key, out var service))
            {
                if (service is T result)
                {
                    return result;
                }
            }
            return null;
        }

        public void Add<T>(string key, T value) where T : class
        {
            MoMoBotAssert.KeyNotNullOrEmpty(key);
            MoMoBotAssert.ValueNotNull(value);

            base.Add(key, value);
        }

        public T Get<T>() where T : class
        {
            return Get<T>(typeof(T).FullName);
        }

        public void Add<T>(T value) where T : class
        {
            Add<T>(typeof(T).FullName, value);
        }

        public void Dispose()
        {
            foreach (var entry in Values)
            {
                if (entry is IDisposable disposableEntry)
                {
                    disposableEntry.Dispose();
                }
            }
        }
    }
}
