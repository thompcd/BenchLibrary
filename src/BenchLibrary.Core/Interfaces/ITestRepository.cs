using BenchLibrary.Core.Models;

namespace BenchLibrary.Core.Interfaces;

/// <summary>
/// Repository interface for persisting and retrieving test results.
/// </summary>
public interface ITestRepository
{
    /// <summary>
    /// Adds a new test result to the repository.
    /// </summary>
    /// <param name="testResult">The test result to add.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The added test result with its assigned ID.</returns>
    Task<TestResult> AddAsync(TestResult testResult, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a test result by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the test result.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The test result if found, null otherwise.</returns>
    Task<TestResult?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all test results for a specific serial number.
    /// </summary>
    /// <param name="serialNumber">The device serial number.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of test results for the serial number.</returns>
    Task<IEnumerable<TestResult>> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets test results within a date range.
    /// </summary>
    /// <param name="startDate">The start of the date range (inclusive).</param>
    /// <param name="endDate">The end of the date range (inclusive).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of test results within the date range.</returns>
    Task<IEnumerable<TestResult>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing test result.
    /// </summary>
    /// <param name="testResult">The test result to update.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated test result.</returns>
    Task<TestResult> UpdateAsync(TestResult testResult, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a test result by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the test result to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if the test result was deleted, false if not found.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
