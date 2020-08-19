using System;

namespace Funcky
{
    public class LinearBackoffRetryPolicy : IRetryPolicy
    {
        private readonly TimeSpan _firstDelay;

        public LinearBackoffRetryPolicy(int maxRetry, TimeSpan firstDelay)
        {
            MaxRetry = maxRetry;
            _firstDelay = firstDelay;
        }

        public int MaxRetry { get; }

#if NETSTANDARD2_1
        public TimeSpan Duration(int onRetryCount)
            => _firstDelay.Multiply(onRetryCount);
#else
        public TimeSpan Duration(int onRetryCount)
        => TimeSpan.FromTicks(_firstDelay.Ticks * onRetryCount);
#endif
    }
}
