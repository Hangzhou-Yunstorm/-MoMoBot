﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MoMoBot.Core
{
    public class MemoryStorage : IStorage
    {
        private static readonly JsonSerializer StateJsonSerializer = new JsonSerializer() { TypeNameHandling = TypeNameHandling.All };
        private readonly Dictionary<string, JObject> _memory;
        private readonly object _synclock = new object();
        private int _eTag = 0;
        public MemoryStorage(Dictionary<string, JObject> dictionary = null)
        {
            _memory = dictionary ?? new Dictionary<string, JObject>();
        }

        public Task DeleteAsync(string[] keys, CancellationToken cancellationToken = default)
        {
            MoMoBotAssert.NotNull(keys);
            lock (_synclock)
            {
                foreach (var key in keys)
                {
                    _memory.Remove(key);
                }
            }
            return Task.CompletedTask;
        }

        public Task<IDictionary<string, object>> ReadAsync(string[] keys, CancellationToken cancellationToken = default)
        {
            MoMoBotAssert.NotNull(keys);
            var storeItems = new Dictionary<string, object>(keys.Length);
            lock (_synclock)
            {
                foreach (var key in keys)
                {
                    if (_memory.TryGetValue(key, out var state))
                    {
                        if (state != null)
                        {
                            storeItems.Add(key, state.ToObject<object>(StateJsonSerializer));
                        }
                    }
                }
            }

            return Task.FromResult<IDictionary<string, object>>(storeItems);
        }

        public Task<T> ReadAsync<T>(string key, Func<T> def)
        {
            MoMoBotAssert.KeyNotNullOrEmpty(key);

            lock (_synclock)
            {
                if (_memory.TryGetValue(key, out var state))
                {
                    if (state != null)
                    {
                        return Task.FromResult(state.ToObject<T>(StateJsonSerializer));
                    }
                }
                if (def != null)
                {
                    var result = def.Invoke();

                    if (result != null)
                    {
                        WriteAsync(new Dictionary<string, object> { { key, result } });
                        return Task.FromResult(result);
                    }

                }
                return default;
            }
        }

        public Task WriteAsync(IDictionary<string, object> changes, CancellationToken cancellationToken = default)
        {
            MoMoBotAssert.NotNull(changes);
            lock (_synclock)
            {
                foreach (var change in changes)
                {
                    var newValue = change.Value;

                    var oldStateETag = default(string);

                    if (_memory.TryGetValue(change.Key, out var oldState))
                    {
                        if (oldState.TryGetValue("eTag", out var etag))
                        {
                            oldStateETag = etag.Value<string>();
                        }
                    }

                    var newState = JObject.FromObject(newValue, StateJsonSerializer);

                    // Set ETag if applicable
                    if (newValue is IStoreItem newStoreItem)
                    {
                        if (oldStateETag != null
                                &&
                           newStoreItem.ETag != "*"
                                &&
                           newStoreItem.ETag != oldStateETag)
                        {
                            throw new Exception($"Etag conflict.\r\n\r\nOriginal: {newStoreItem.ETag}\r\nCurrent: {oldStateETag}");
                        }

                        newState["eTag"] = (_eTag++).ToString();
                    }

                    _memory[change.Key] = newState;
                }
            }

            return Task.CompletedTask;
        }
    }
}
