namespace BenchLibrary.SixSigma.Models;

/// <summary>
/// Represents the results of a process capability analysis.
/// </summary>
public class CapabilityResult
{
    /// <summary>
    /// Gets or sets the process capability index (Cp).
    /// Measures potential capability assuming the process is centered.
    /// Cp = (USL - LSL) / (6 * sigma)
    /// </summary>
    public double Cp { get; set; }

    /// <summary>
    /// Gets or sets the process capability index adjusted for centering (Cpk).
    /// Measures actual capability considering how centered the process is.
    /// Cpk = min((USL - mean) / (3 * sigma), (mean - LSL) / (3 * sigma))
    /// </summary>
    public double Cpk { get; set; }

    /// <summary>
    /// Gets or sets the process performance index (Pp).
    /// Similar to Cp but uses overall standard deviation instead of within-group.
    /// </summary>
    public double Pp { get; set; }

    /// <summary>
    /// Gets or sets the process performance index adjusted for centering (Ppk).
    /// Similar to Cpk but uses overall standard deviation.
    /// </summary>
    public double Ppk { get; set; }

    /// <summary>
    /// Gets or sets the upper specification limit (USL).
    /// </summary>
    public double UpperSpecLimit { get; set; }

    /// <summary>
    /// Gets or sets the lower specification limit (LSL).
    /// </summary>
    public double LowerSpecLimit { get; set; }

    /// <summary>
    /// Gets or sets the target or nominal value.
    /// </summary>
    public double? Target { get; set; }

    /// <summary>
    /// Gets or sets the process mean.
    /// </summary>
    public double Mean { get; set; }

    /// <summary>
    /// Gets or sets the process standard deviation.
    /// </summary>
    public double StandardDeviation { get; set; }

    /// <summary>
    /// Gets or sets the sample size used in the analysis.
    /// </summary>
    public int SampleSize { get; set; }

    /// <summary>
    /// Gets or sets the minimum value in the dataset.
    /// </summary>
    public double Minimum { get; set; }

    /// <summary>
    /// Gets or sets the maximum value in the dataset.
    /// </summary>
    public double Maximum { get; set; }

    /// <summary>
    /// Gets or sets the estimated percentage of values below LSL.
    /// </summary>
    public double PercentBelowLsl { get; set; }

    /// <summary>
    /// Gets or sets the estimated percentage of values above USL.
    /// </summary>
    public double PercentAboveUsl { get; set; }

    /// <summary>
    /// Gets the total estimated percentage of out-of-specification values.
    /// </summary>
    public double TotalOutOfSpec => PercentBelowLsl + PercentAboveUsl;

    /// <summary>
    /// Gets a value indicating whether the process is capable (Cpk >= 1.33).
    /// </summary>
    public bool IsCapable => Cpk >= 1.33;

    /// <summary>
    /// Gets a value indicating whether the process is centered (Cp ≈ Cpk).
    /// </summary>
    public bool IsCentered => Math.Abs(Cp - Cpk) < 0.1;

    /// <summary>
    /// Gets the capability rating based on Cpk value.
    /// </summary>
    public CapabilityRating Rating
    {
        get
        {
            return Cpk switch
            {
                >= 2.0 => CapabilityRating.WorldClass,
                >= 1.67 => CapabilityRating.Excellent,
                >= 1.33 => CapabilityRating.Good,
                >= 1.0 => CapabilityRating.Marginal,
                >= 0.67 => CapabilityRating.Poor,
                _ => CapabilityRating.Inadequate
            };
        }
    }
}

/// <summary>
/// Rating categories for process capability.
/// </summary>
public enum CapabilityRating
{
    /// <summary>Cpk &lt; 0.67 - Process is inadequate.</summary>
    Inadequate,

    /// <summary>Cpk 0.67-1.0 - Process is poor.</summary>
    Poor,

    /// <summary>Cpk 1.0-1.33 - Process is marginal.</summary>
    Marginal,

    /// <summary>Cpk 1.33-1.67 - Process is good.</summary>
    Good,

    /// <summary>Cpk 1.67-2.0 - Process is excellent.</summary>
    Excellent,

    /// <summary>Cpk >= 2.0 - World class process.</summary>
    WorldClass
}
