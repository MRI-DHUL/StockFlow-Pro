using Moq;
using System.Linq.Expressions;

namespace StockFlow.UnitTests.Helpers;

/// <summary>
/// Helper methods for working with Moq in tests
/// </summary>
public static class MockExtensions
{
    /// <summary>
    /// Sets up a mock to return a queryable collection
    /// </summary>
    public static Mock<T> SetupQueryable<T, TEntity>(this Mock<T> mock, Expression<Func<T, IQueryable<TEntity>>> expression, IEnumerable<TEntity> data)
        where T : class
    {
        var queryable = data.AsQueryable();
        mock.Setup(expression).Returns(queryable);
        return mock;
    }

    /// <summary>
    /// Verifies that a method was called with any valid cancellation token
    /// </summary>
    public static void VerifyWithAnyCancellationToken<T>(this Mock<T> mock, Expression<Action<T>> expression, Times times)
        where T : class
    {
        mock.Verify(expression, times);
    }
}
