namespace BenchLibrary.SixSigma.Models;

/// <summary>
/// Represents the result of a sigma level calculation.
/// </summary>
public class SigmaLevelResult
{
    /// <summary>
    /// Gets or sets the sigma level (typically 1-6).
    /// </summary>
    public double SigmaLevel { get; set; }

    /// <summary>
    /// Gets or sets the defects per million opportunities (DPMO).
    /// </summary>
    public double Dpmo { get; set; }

    /// <summary>
    /// Gets or sets the yield percentage.
    /// </summary>
    public double Yield { get; set; }

    /// <summary>
    /// Gets or sets the total number of opportunities.
    /// </summary>
    public long TotalOpportunities { get; set; }

    /// <summary>
    /// Gets or sets the total number of defects.
    /// </summary>
    public long TotalDefects { get; set; }

    /// <summary>
    /// Gets or sets the total number of units.
    /// </summary>
    public long TotalUnits { get; set; }

    /// <summary>
    /// Gets or sets the number of defect opportunities per unit.
    /// </summary>
    public int OpportunitiesPerUnit { get; set; }

    /// <summary>
    /// Gets the defect rate (defects / opportunities).
    /// </summary>
    public double DefectRate => TotalOpportunities > 0
        ? (double)TotalDefects / TotalOpportunities
        : 0;

    /// <summary>
    /// Gets a value indicating whether the process meets Six Sigma standards.
    /// </summary>
    public bool MeetsSixSigma => SigmaLevel >= 6.0;

    /// <summary>
    /// Gets the sigma level rating.
    /// </summary>
    public SigmaRating Rating
    {
        get
        {
            return SigmaLevel switch
            {
                >= 6.0 => SigmaRating.SixSigma,
                >= 5.0 => SigmaRating.FiveSigma,
                >= 4.0 => SigmaRating.FourSigma,
                >= 3.0 => SigmaRating.ThreeSigma,
                >= 2.0 => SigmaRating.TwoSigma,
                >= 1.0 => SigmaRating.OneSigma,
                _ => SigmaRating.BelowOneSigma
            };
        }
    }

    /// <summary>
    /// Gets the typical industry benchmark description for this sigma level.
    /// </summary>
    public string IndustryBenchmark
    {
        get
        {
            return SigmaLevel switch
            {
                >= 6.0 => "World class - 3.4 DPMO",
                >= 5.0 => "Excellent - 233 DPMO",
                >= 4.0 => "Good - 6,210 DPMO",
                >= 3.0 => "Average - 66,807 DPMO",
                >= 2.0 => "Below average - 308,538 DPMO",
                >= 1.0 => "Poor - 691,462 DPMO",
                _ => "Inadequate - Very high defect rate"
            };
        }
    }
}

/// <summary>
/// Rating categories for sigma levels.
/// </summary>
public enum SigmaRating
{
    /// <summary>Below 1 sigma - inadequate quality.</summary>
    BelowOneSigma,

    /// <summary>1 sigma - 69% yield.</summary>
    OneSigma,

    /// <summary>2 sigma - 93% yield.</summary>
    TwoSigma,

    /// <summary>3 sigma - 99.73% yield.</summary>
    ThreeSigma,

    /// <summary>4 sigma - 99.9937% yield.</summary>
    FourSigma,

    /// <summary>5 sigma - 99.99977% yield.</summary>
    FiveSigma,

    /// <summary>6 sigma - 99.9999998% yield (3.4 DPMO).</summary>
    SixSigma
}
