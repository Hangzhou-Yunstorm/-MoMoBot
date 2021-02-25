using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MoMoBot.Core
{
    public abstract class BotState : IPropertyManager
    {
        private readonly IStorage _storage;
        private readonly string _contextServiceKey;
        public BotState(IStorage storage, string contextServiceKey)
        {
            MoMoBotAssert.NotNull(storage);
            MoMoBotAssert.NotNull(contextServiceKey);
            _storage = storage;
            _contextServiceKey = contextServiceKey;
        }

        public IStatePropertyAccessor<T> CreateProperty<T>(string name)
        {
            MoMoBotAssert.NameNullOrWhiteSpace(name);
            return new StatePropertyAccessor<T>(this, name);
        }

        /// <summary>
        /// Reads in  the current state object and caches it in the context object for this turm.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="force">Optional. True to bypass the cache.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task LoadAsync(TurnContext turnContext, bool force = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            var cachedState = turnContext.TurnState.Get<CachedBotState>(_contextServiceKey);
            var storageKey = GetStorageKey(turnContext);
            if (force || cachedState == null || cachedState.State == null)
            {
                var items = await _storage.ReadAsync(new[] { storageKey }, cancellationToken).ConfigureAwait(false);
                items.TryGetValue(storageKey, out object val);
                turnContext.TurnState[_contextServiceKey] = new CachedBotState((IDictionary<string, object>)val);
            }
        }

        /// <summary>
        /// If it has changed, writes to storage the state object that is cached in the current context object for this turn.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="force">Optional. True to save state to storage whether or not there are changes.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task SaveChangesAsync(TurnContext turnContext, bool force = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            var cachedState = turnContext.TurnState.Get<CachedBotState>(_contextServiceKey);
            if (force || (cachedState != null && cachedState.IsChanged()))
            {
                var key = GetStorageKey(turnContext);
                var changes = new Dictionary<string, object>
                {
                    { key, cachedState.State },
                };
                await _storage.WriteAsync(changes).ConfigureAwait(false);
                cachedState.Hash = cachedState.ComputeHash(cachedState.State);
                return;
            }
        }

        /// <summary>
        /// Clears any state currently stored in this state scope.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="cancellationToken">cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>NOTE: that SaveChangesAsync must be called in order for the cleared state to be persisted to the underlying store.</remarks>
        public Task ClearStateAsync(TurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            // Explicitly setting the hash will mean IsChanged is always true. And that will force a Save.
            turnContext.TurnState[_contextServiceKey] = new CachedBotState { Hash = string.Empty };

            return Task.CompletedTask;
        }

        /// <summary>
        /// Delete any state currently stored in this state scope.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="cancellationToken">cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DeleteAsync(TurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            var cachedState = turnContext.TurnState.Get<CachedBotState>(_contextServiceKey);
            if (cachedState != null)
            {
                turnContext.TurnState.Remove(_contextServiceKey);
            }

            var storageKey = GetStorageKey(turnContext);
            await _storage.DeleteAsync(new[] { storageKey }, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// When overridden in a derived class, gets the key to use when reading and writing state to and from storage.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <returns>The storage key.</returns>
        protected abstract string GetStorageKey(TurnContext turnContext);

        /// <summary>
        /// Gets a property from the state cache in the turn context.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <remarks>If the task is successful, the result contains the property value.</remarks>
        protected Task<T> GetPropertyValueAsync<T>(TurnContext turnContext, string propertyName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            var cachedState = turnContext.TurnState.Get<CachedBotState>(_contextServiceKey);

            // if there is no value, this will throw, to signal to IPropertyAccesor that a default value should be computed
            // This allows this to work with value types
            return Task.FromResult((T)cachedState.State[propertyName]);
        }

        /// <summary>
        /// Deletes a property from the state cache in the turn context.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="propertyName">The name of the property to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        protected Task DeletePropertyValueAsync(TurnContext turnContext, string propertyName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            var cachedState = turnContext.TurnState.Get<CachedBotState>(_contextServiceKey);
            cachedState.State.Remove(propertyName);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Set the value of a property in the state cache in the turn context.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set on the property.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        protected Task SetPropertyValueAsync(TurnContext turnContext, string propertyName, object value, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            var cachedState = turnContext.TurnState.Get<CachedBotState>(_contextServiceKey);
            cachedState.State[propertyName] = value;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Internal cached bot state.
        /// </summary>
        private class CachedBotState
        {
            public CachedBotState(IDictionary<string, object> state = null)
            {
                State = state ?? new ConcurrentDictionary<string, object>();
                Hash = ComputeHash(State);
            }

            public IDictionary<string, object> State { get; set; }

            public string Hash { get; set; }

            public bool IsChanged()
            {
                return Hash != ComputeHash(State);
            }

            internal string ComputeHash(object obj)
            {
                return JsonConvert.SerializeObject(obj);
            }
        }

        /// <summary>
        /// Implements IPropertyAccessor for an IPropertyContainer.
        /// Note the semantic of this accessor are intended to be lazy, this means teh Get, Set and Delete
        /// methods will first call LoadAsync. This will be a no-op if the data is already loaded.
        /// The implication is you can just use this accessor in the application code directly without first calling LoadAsync
        /// this approach works with the AutoSaveStateMiddleware which will save as needed at the end of a turn.
        /// </summary>
        /// <typeparam name="T">type of value the propertyAccessor accesses.</typeparam>
        private class BotStatePropertyAccessor<T> : IStatePropertyAccessor<T>
        {
            private BotState _botState;

            public BotStatePropertyAccessor(BotState botState, string name)
            {
                _botState = botState;
                Name = name;
            }

            /// <summary>
            /// Gets name of the property.
            /// </summary>
            /// <value>
            /// name of the property.
            /// </value>
            public string Name { get; private set; }

            /// <summary>
            /// Delete the property. The semantics are intended to be lazy, note the use of LoadAsync at the start.
            /// </summary>
            /// <param name="turnContext">The turn context.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
            public async Task DeleteAsync(TurnContext turnContext, CancellationToken cancellationToken)
            {
                await _botState.LoadAsync(turnContext, false, cancellationToken).ConfigureAwait(false);
                await _botState.DeletePropertyValueAsync(turnContext, Name, cancellationToken).ConfigureAwait(false);
            }

            /// <summary>
            /// Get the property value. The semantics are intended to be lazy, note the use of LoadAsync at the start.
            /// </summary>
            /// <param name="turnContext">The context object for this turn.</param>
            /// <param name="defaultValueFactory">Defines the default value. Invoked when no value been set for the requested state property.  If defaultValueFactory is defined as null, the MissingMemberException will be thrown if the underlying property is not set.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
            public async Task<T> GetAsync(TurnContext turnContext, Func<T> defaultValueFactory, CancellationToken cancellationToken)
            {
                await _botState.LoadAsync(turnContext, false, cancellationToken).ConfigureAwait(false);
                try
                {
                    return await _botState.GetPropertyValueAsync<T>(turnContext, Name, cancellationToken).ConfigureAwait(false);
                }
                catch (KeyNotFoundException)
                {
                    // ask for default value from factory
                    if (defaultValueFactory == null)
                    {
                        return default(T);
                    }

                    var result = defaultValueFactory();

                    // save default value for any further calls
                    await SetAsync(turnContext, result, cancellationToken).ConfigureAwait(false);
                    return result;
                }
            }

            /// <summary>
            /// Set the property value. The semantics are intended to be lazy, note the use of LoadAsync at the start.
            /// </summary>
            /// <param name="turnContext">turn context.</param>
            /// <param name="value">value.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
            public async Task SetAsync(TurnContext turnContext, T value, CancellationToken cancellationToken)
            {
                await _botState.LoadAsync(turnContext, false, cancellationToken).ConfigureAwait(false);
                await _botState.SetPropertyValueAsync(turnContext, Name, value, cancellationToken).ConfigureAwait(false);
            }
        }

        public class StatePropertyAccessor<T> : IStatePropertyAccessor<T>
        {
            private BotState _botState;

            public StatePropertyAccessor(BotState botState, string name)
            {
                _botState = botState;
                Name = name;
            }

            /// <summary>
            /// Gets name of the property.
            /// </summary>
            /// <value>
            /// name of the property.
            /// </value>
            public string Name { get; private set; }

            /// <summary>
            /// Delete the property. The semantics are intended to be lazy, note the use of LoadAsync at the start.
            /// </summary>
            /// <param name="turnContext">The turn context.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
            public async Task DeleteAsync(TurnContext turnContext, CancellationToken cancellationToken)
            {
                await _botState.LoadAsync(turnContext, false, cancellationToken).ConfigureAwait(false);
                await _botState.DeletePropertyValueAsync(turnContext, Name, cancellationToken).ConfigureAwait(false);
            }

            /// <summary>
            /// Get the property value. The semantics are intended to be lazy, note the use of LoadAsync at the start.
            /// </summary>
            /// <param name="turnContext">The context object for this turn.</param>
            /// <param name="defaultValueFactory">Defines the default value. Invoked when no value been set for the requested state property.  If defaultValueFactory is defined as null, the MissingMemberException will be thrown if the underlying property is not set.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
            public async Task<T> GetAsync(TurnContext turnContext, Func<T> defaultValueFactory, CancellationToken cancellationToken)
            {
                await _botState.LoadAsync(turnContext, false, cancellationToken).ConfigureAwait(false);
                try
                {
                    return await _botState.GetPropertyValueAsync<T>(turnContext, Name, cancellationToken).ConfigureAwait(false);
                }
                catch (KeyNotFoundException)
                {
                    // ask for default value from factory
                    if (defaultValueFactory == null)
                    {
                        return default(T);
                    }

                    var result = defaultValueFactory();

                    // save default value for any further calls
                    await SetAsync(turnContext, result, cancellationToken).ConfigureAwait(false);
                    return result;
                }
            }

            /// <summary>
            /// Set the property value. The semantics are intended to be lazy, note the use of LoadAsync at the start.
            /// </summary>
            /// <param name="turnContext">turn context.</param>
            /// <param name="value">value.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
            public async Task SetAsync(TurnContext turnContext, T value, CancellationToken cancellationToken)
            {
                await _botState.LoadAsync(turnContext, false, cancellationToken).ConfigureAwait(false);
                await _botState.SetPropertyValueAsync(turnContext, Name, value, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
