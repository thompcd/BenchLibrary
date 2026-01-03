using BenchLibrary.Core.Interfaces;
using BenchLibrary.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BenchLibrary.Data.Repositories;

/// <summary>
/// Repository implementation for test results using Entity Framework Core.
/// </summary>
public class TestRepository : ITestRepository
{
    private readonly BenchLibraryDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public TestRepository(BenchLibraryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<TestResult> AddAsync(TestResult testResult, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(testResult);

        _context.TestResults.Add(testResult);
        await _context.SaveChangesAsync(cancellationToken);
        return testResult;
    }

    /// <inheritdoc />
    public async Task<TestResult?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.TestResults
            .Include(t => t.Measurements)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TestResult>> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(serialNumber);

        return await _context.TestResults
            .Include(t => t.Measurements)
            .Where(t => t.SerialNumber == serialNumber)
            .OrderByDescending(t => t.StartTime)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TestResult>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.TestResults
            .Include(t => t.Measurements)
            .Where(t => t.StartTime >= startDate && t.StartTime <= endDate)
            .OrderByDescending(t => t.StartTime)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TestResult> UpdateAsync(TestResult testResult, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(testResult);

        _context.TestResults.Update(testResult);
        await _context.SaveChangesAsync(cancellationToken);
        return testResult;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var testResult = await _context.TestResults.FindAsync(new object[] { id }, cancellationToken);
        if (testResult == null)
        {
            return false;
        }

        _context.TestResults.Remove(testResult);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
