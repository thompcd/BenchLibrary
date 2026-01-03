using BenchLibrary.Core.Enums;

namespace BenchLibrary.Core.Models;

/// <summary>
/// Represents a single measurement taken during a test.
/// </summary>
public class Measurement
{
    /// <summary>
    /// Gets or sets the unique identifier for this measurement.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name or label of this measurement.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the measured value.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the unit of measurement.
    /// </summary>
    public MeasurementUnit Unit { get; set; }

    /// <summary>
    /// Gets or sets the lower specification limit (LSL).
    /// Null if there is no lower limit.
    /// </summary>
    public double? LowerLimit { get; set; }

    /// <summary>
    /// Gets or sets the upper specification limit (USL).
    /// Null if there is no upper limit.
    /// </summary>
    public double? UpperLimit { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the measurement was taken.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the ID of the test result this measurement belongs to.
    /// </summary>
    public int TestResultId { get; set; }

    /// <summary>
    /// Gets a value indicating whether the measurement is within specification limits.
    /// </summary>
    public bool IsWithinLimits
    {
        get
        {
            var aboveLower = !LowerLimit.HasValue || Value >= LowerLimit.Value;
            var belowUpper = !UpperLimit.HasValue || Value <= UpperLimit.Value;
            return aboveLower && belowUpper;
        }
    }

    /// <summary>
    /// Gets the deviation from the specification limits.
    /// Returns 0 if within limits, negative if below LSL, positive if above USL.
    /// </summary>
    public double Deviation
    {
        get
        {
            if (LowerLimit.HasValue && Value < LowerLimit.Value)
            {
                return Value - LowerLimit.Value;
            }

            if (UpperLimit.HasValue && Value > UpperLimit.Value)
            {
                return Value - UpperLimit.Value;
            }

            return 0;
        }
    }
}
