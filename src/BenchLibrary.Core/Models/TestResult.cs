using BenchLibrary.Core.Enums;

namespace BenchLibrary.Core.Models;

/// <summary>
/// Represents the result of a complete test execution on a device.
/// </summary>
public class TestResult
{
    /// <summary>
    /// Gets or sets the unique identifier for this test result.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the serial number of the device under test.
    /// </summary>
    public required string SerialNumber { get; set; }

    /// <summary>
    /// Gets or sets the name of the test that was executed.
    /// </summary>
    public required string TestName { get; set; }

    /// <summary>
    /// Gets or sets the current status of the test.
    /// </summary>
    public TestStatus Status { get; set; } = TestStatus.NotStarted;

    /// <summary>
    /// Gets or sets the timestamp when the test started.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the test completed.
    /// Null if the test has not completed.
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Gets or sets the operator or user who ran the test.
    /// </summary>
    public string? Operator { get; set; }

    /// <summary>
    /// Gets or sets the workstation or test station identifier.
    /// </summary>
    public string? Workstation { get; set; }

    /// <summary>
    /// Gets or sets any error message if the test failed or errored.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the collection of measurements taken during the test.
    /// </summary>
    public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();

    /// <summary>
    /// Gets or sets the timestamp when this record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the timestamp when this record was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the duration of the test, or null if the test has not completed.
    /// </summary>
    public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value - StartTime : null;

    /// <summary>
    /// Gets a value indicating whether all measurements passed their specification limits.
    /// </summary>
    public bool AllMeasurementsPassed => Measurements.All(m => m.IsWithinLimits);

    /// <summary>
    /// Gets the count of measurements that failed their specification limits.
    /// </summary>
    public int FailedMeasurementCount => Measurements.Count(m => !m.IsWithinLimits);

    /// <summary>
    /// Gets the total number of measurements in this test result.
    /// </summary>
    public int TotalMeasurementCount => Measurements.Count;
}
