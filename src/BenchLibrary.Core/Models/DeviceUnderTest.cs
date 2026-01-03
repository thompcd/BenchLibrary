namespace BenchLibrary.Core.Models;

/// <summary>
/// Represents a device or product being tested in the manufacturing process.
/// </summary>
public class DeviceUnderTest
{
    /// <summary>
    /// Gets or sets the unique identifier for this device record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique serial number of the device.
    /// </summary>
    public required string SerialNumber { get; set; }

    /// <summary>
    /// Gets or sets the product model or part number.
    /// </summary>
    public required string ModelNumber { get; set; }

    /// <summary>
    /// Gets or sets the product revision or version.
    /// </summary>
    public string? Revision { get; set; }

    /// <summary>
    /// Gets or sets the manufacturing lot or batch number.
    /// </summary>
    public string? LotNumber { get; set; }

    /// <summary>
    /// Gets or sets the date the device was manufactured.
    /// </summary>
    public DateTime? ManufactureDate { get; set; }

    /// <summary>
    /// Gets or sets the current location or station of the device.
    /// </summary>
    public string? CurrentLocation { get; set; }

    /// <summary>
    /// Gets or sets additional notes or comments about the device.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the collection of test results for this device.
    /// </summary>
    public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();

    /// <summary>
    /// Gets or sets the timestamp when this record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the timestamp when this record was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the overall test status based on all test results.
    /// Returns true if all tests passed, false if any failed, null if no tests exist.
    /// </summary>
    public bool? OverallTestStatus
    {
        get
        {
            if (!TestResults.Any())
            {
                return null;
            }

            return TestResults.All(t => t.Status == Enums.TestStatus.Passed);
        }
    }

    /// <summary>
    /// Gets the count of tests that have been executed on this device.
    /// </summary>
    public int TestCount => TestResults.Count;

    /// <summary>
    /// Gets the count of tests that passed.
    /// </summary>
    public int PassedTestCount => TestResults.Count(t => t.Status == Enums.TestStatus.Passed);

    /// <summary>
    /// Gets the count of tests that failed.
    /// </summary>
    public int FailedTestCount => TestResults.Count(t => t.Status == Enums.TestStatus.Failed);
}
