using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoMoBot.Core
{
    public interface IStatePropertyAccessor<T> : IStatePropertyInfo
    {
        /// <summary>
        /// Get the property value from the source.
        /// If the property is not set, and no default value was defined, a <see cref="MissingMemberException"/> is thrown.
        /// </summary>
        /// <param name="turnContext">Turn Context.</param>
        /// <param name="defaultValueFactory">Function which defines the property value to be returned if no value has been set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<T> GetAsync(TurnContext turnContext, Func<T> defaultValueFactory = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete the property from the source.
        /// </summary>
        /// <param name="turnContext">Turn Context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeleteAsync(TurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Set the property value on the source.
        /// </summary>
        /// <param name="turnContext">Turn Context.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetAsync(TurnContext turnContext, T value, CancellationToken cancellationToken = default(CancellationToken));
    }
}
