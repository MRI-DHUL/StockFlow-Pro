using Hangfire;
using StockFlow.Application.Interfaces;

namespace StockFlow.Infrastructure.Services;

public class HangfireJobService : IBackgroundJobService
{
    public void EnqueueJob(Action job)
    {
        BackgroundJob.Enqueue(() => job());
    }

    public void EnqueueJob<T>(Action<T> job)
    {
        BackgroundJob.Enqueue<T>(x => job(x));
    }

    public void ScheduleJob(Action job, TimeSpan delay)
    {
        BackgroundJob.Schedule(() => job(), delay);
    }

    public void RecurringJob(string jobId, Action job, string cronExpression)
    {
        Hangfire.RecurringJob.AddOrUpdate(jobId, () => job(), cronExpression);
    }
}
