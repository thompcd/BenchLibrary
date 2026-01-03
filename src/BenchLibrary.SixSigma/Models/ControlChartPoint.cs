namespace BenchLibrary.SixSigma.Models;

/// <summary>
/// Represents a single data point on a control chart.
/// </summary>
public class ControlChartPoint
{
    /// <summary>
    /// Gets or sets the sample or subgroup number.
    /// </summary>
    public int SampleNumber { get; set; }

    /// <summary>
    /// Gets or sets the plotted value (e.g., X-bar, R, etc.).
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the sample.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the center line value at this point.
    /// </summary>
    public double CenterLine { get; set; }

    /// <summary>
    /// Gets or sets the upper control limit (UCL) at this point.
    /// </summary>
    public double UpperControlLimit { get; set; }

    /// <summary>
    /// Gets or sets the lower control limit (LCL) at this point.
    /// </summary>
    public double LowerControlLimit { get; set; }

    /// <summary>
    /// Gets a value indicating whether the point is out of control (above UCL or below LCL).
    /// </summary>
    public bool IsOutOfControl => Value > UpperControlLimit || Value < LowerControlLimit;

    /// <summary>
    /// Gets a value indicating whether the point is above the upper control limit.
    /// </summary>
    public bool IsAboveUcl => Value > UpperControlLimit;

    /// <summary>
    /// Gets a value indicating whether the point is below the lower control limit.
    /// </summary>
    public bool IsBelowLcl => Value < LowerControlLimit;

    /// <summary>
    /// Gets or sets any violation rules triggered by this point.
    /// </summary>
    public IList<ControlChartViolation> Violations { get; set; } = new List<ControlChartViolation>();
}

/// <summary>
/// Represents a Western Electric rule violation on a control chart.
/// </summary>
public class ControlChartViolation
{
    /// <summary>
    /// Gets or sets the rule number that was violated.
    /// </summary>
    public int RuleNumber { get; set; }

    /// <summary>
    /// Gets or sets the description of the violation.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Gets or sets the severity of the violation.
    /// </summary>
    public ViolationSeverity Severity { get; set; }
}

/// <summary>
/// Severity levels for control chart violations.
/// </summary>
public enum ViolationSeverity
{
    /// <summary>Warning level - process may be drifting.</summary>
    Warning,

    /// <summary>Major level - immediate attention needed.</summary>
    Major,

    /// <summary>Critical level - process is out of control.</summary>
    Critical
}
