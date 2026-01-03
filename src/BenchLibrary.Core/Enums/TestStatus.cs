namespace BenchLibrary.Core.Enums;

/// <summary>
/// Represents the status of a test execution.
/// </summary>
public enum TestStatus
{
    /// <summary>
    /// Test has not been started yet.
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// Test is currently running.
    /// </summary>
    Running = 1,

    /// <summary>
    /// Test completed and all measurements passed specifications.
    /// </summary>
    Passed = 2,

    /// <summary>
    /// Test completed but one or more measurements failed specifications.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Test encountered an error during execution.
    /// </summary>
    Error = 4,

    /// <summary>
    /// Test was cancelled before completion.
    /// </summary>
    Cancelled = 5
}
