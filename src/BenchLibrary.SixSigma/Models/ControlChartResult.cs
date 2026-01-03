namespace BenchLibrary.SixSigma.Models;

/// <summary>
/// Represents the complete result of a control chart analysis.
/// </summary>
public class ControlChartResult
{
    /// <summary>
    /// Gets or sets the type of control chart.
    /// </summary>
    public ControlChartType ChartType { get; set; }

    /// <summary>
    /// Gets or sets the center line value.
    /// </summary>
    public double CenterLine { get; set; }

    /// <summary>
    /// Gets or sets the upper control limit (UCL).
    /// </summary>
    public double UpperControlLimit { get; set; }

    /// <summary>
    /// Gets or sets the lower control limit (LCL).
    /// </summary>
    public double LowerControlLimit { get; set; }

    /// <summary>
    /// Gets or sets the collection of data points on the chart.
    /// </summary>
    public IList<ControlChartPoint> Points { get; set; } = new List<ControlChartPoint>();

    /// <summary>
    /// Gets or sets the subgroup size used for calculations.
    /// </summary>
    public int SubgroupSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of subgroups.
    /// </summary>
    public int SubgroupCount { get; set; }

    /// <summary>
    /// Gets the count of points that are out of control.
    /// </summary>
    public int OutOfControlCount => Points.Count(p => p.IsOutOfControl);

    /// <summary>
    /// Gets the percentage of points that are in control.
    /// </summary>
    public double InControlPercentage => Points.Count > 0
        ? (1.0 - (double)OutOfControlCount / Points.Count) * 100
        : 100;

    /// <summary>
    /// Gets a value indicating whether the process is in statistical control.
    /// </summary>
    public bool IsInControl => OutOfControlCount == 0;

    /// <summary>
    /// Gets all points that are out of control.
    /// </summary>
    public IEnumerable<ControlChartPoint> OutOfControlPoints => Points.Where(p => p.IsOutOfControl);

    /// <summary>
    /// Gets or sets the estimated process sigma.
    /// </summary>
    public double EstimatedSigma { get; set; }

    /// <summary>
    /// Gets or sets the process mean (X-bar-bar for X-bar charts).
    /// </summary>
    public double ProcessMean { get; set; }

    /// <summary>
    /// Gets or sets the average range (R-bar for R charts).
    /// </summary>
    public double AverageRange { get; set; }
}

/// <summary>
/// Types of control charts.
/// </summary>
public enum ControlChartType
{
    /// <summary>X-bar chart for subgroup means.</summary>
    XBar,

    /// <summary>R chart for subgroup ranges.</summary>
    Range,

    /// <summary>S chart for subgroup standard deviations.</summary>
    StandardDeviation,

    /// <summary>Individual measurements chart (I chart).</summary>
    Individuals,

    /// <summary>Moving range chart (MR chart).</summary>
    MovingRange,

    /// <summary>p chart for proportion defective.</summary>
    ProportionDefective,

    /// <summary>np chart for number defective.</summary>
    NumberDefective,

    /// <summary>c chart for defects per unit.</summary>
    DefectsPerUnit,

    /// <summary>u chart for defects per unit (varying sample size).</summary>
    DefectsPerUnitVariable
}
