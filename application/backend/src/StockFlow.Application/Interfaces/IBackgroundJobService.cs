namespace StockFlow.Application.Interfaces;

public interface IBackgroundJobService
{
    void EnqueueJob(Action job);
    void EnqueueJob<T>(Action<T> job);
    void ScheduleJob(Action job, TimeSpan delay);
    void RecurringJob(string jobId, Action job, string cronExpression);
}
