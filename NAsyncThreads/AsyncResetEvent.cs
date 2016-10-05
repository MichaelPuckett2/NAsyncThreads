using System;
using System.Threading;

namespace NAsyncThreads
{
    public class AsyncResetEvent
    {
        private volatile int threadCounter = 0;
        private readonly int MaxSimultaneousThreads;
        private readonly int MaxSimultaneousThreadsIgnoreTimeout;
        private static AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        /// <summary>
        /// Constructs AsyncResetEvent with the MaxSimultaneousActions allowed.
        /// </summary>
        /// <param name="maxSimultaneousActions">The number of actions allowed to be invoked simultaneously. A value less than 1 is defaulted to 1.</param>
        public AsyncResetEvent(int maxSimultaneousActions)
        {
            if (maxSimultaneousActions < 1)
                maxSimultaneousActions = 1;

            MaxSimultaneousThreads = maxSimultaneousActions;
            MaxSimultaneousThreadsIgnoreTimeout = int.MaxValue;
        }

        /// <summary>
        /// Constructs AsyncResetEvent with the MaxSimultaneousActions allowed.
        /// </summary>
        /// <param name="maxSimultaneousActions">The number of actions allowed to be invoked simultaneously. A value less than 1 is defaulted to 1.</param>
        /// <param name="maxSimultaneousThreadsIgnoreTimeout">The max numer of actions allowed to run ignoring timeout. A value less than maxSimultaneousActions is defaulted to maxSimultaneousActions</param>
        public AsyncResetEvent(int maxSimultaneousActions, int maxSimultaneousThreadsIgnoreTimeout)
        {
            if (maxSimultaneousActions < 1)
                maxSimultaneousActions = 1;

            MaxSimultaneousThreads = maxSimultaneousActions;

            if (maxSimultaneousThreadsIgnoreTimeout < maxSimultaneousActions)
                maxSimultaneousThreadsIgnoreTimeout = maxSimultaneousActions;

            MaxSimultaneousThreadsIgnoreTimeout = maxSimultaneousThreadsIgnoreTimeout;
        }

        /// <summary>
        /// Forces the current thread to stop until an action has completed.
        /// If the number of threads reaching WaitAction is less than MaxSimultaneousActions then they will be immediately Invoked.
        /// </summary>
        /// <param name="action">The action to Invoke once waiting is complete.</param>
        /// <param name="timeout">The time in milliseconds the action will wait before it is forced to Invoke.</param>
        public void WaitAction(Action action, int? timeout = null)
        {
            threadCounter++;
            autoResetEvent.Reset();

            if (threadCounter > MaxSimultaneousThreads)
                autoResetEvent.WaitOne(threadCounter <= MaxSimultaneousThreadsIgnoreTimeout ? timeout ?? int.MaxValue : int.MaxValue);

            new Thread(() =>
            {
                action?.Invoke();
                threadCounter--;
                autoResetEvent.Set();
            }).Start();
        }
    }
}
