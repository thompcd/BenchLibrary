namespace BenchLibrary.SixSigma.Models;

/// <summary>
/// Represents an item in a Pareto analysis.
/// </summary>
public class ParetoItem
{
    /// <summary>
    /// Gets or sets the category or defect type name.
    /// </summary>
    public required string Category { get; set; }

    /// <summary>
    /// Gets or sets the count or frequency of occurrences.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the percentage of total occurrences.
    /// </summary>
    public double Percentage { get; set; }

    /// <summary>
    /// Gets or sets the cumulative percentage up to and including this item.
    /// </summary>
    public double CumulativePercentage { get; set; }

    /// <summary>
    /// Gets or sets the rank of this item (1 = most frequent).
    /// </summary>
    public int Rank { get; set; }

    /// <summary>
    /// Gets a value indicating whether this item is in the vital few (cumulative &lt;= 80%).
    /// </summary>
    public bool IsVitalFew => CumulativePercentage <= 80;
}

/// <summary>
/// Represents the complete result of a Pareto analysis.
/// </summary>
public class ParetoResult
{
    /// <summary>
    /// Gets or sets the title or name of the analysis.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the items sorted by frequency (highest first).
    /// </summary>
    public IList<ParetoItem> Items { get; set; } = new List<ParetoItem>();

    /// <summary>
    /// Gets the total count across all items.
    /// </summary>
    public int TotalCount => Items.Sum(i => i.Count);

    /// <summary>
    /// Gets the vital few items (cumulative percentage &lt;= 80%).
    /// </summary>
    public IEnumerable<ParetoItem> VitalFew => Items.Where(i => i.IsVitalFew);

    /// <summary>
    /// Gets the trivial many items (cumulative percentage > 80%).
    /// </summary>
    public IEnumerable<ParetoItem> TrivialMany => Items.Where(i => !i.IsVitalFew);

    /// <summary>
    /// Gets the count of vital few categories.
    /// </summary>
    public int VitalFewCount => VitalFew.Count();

    /// <summary>
    /// Gets the percentage of categories that are vital few.
    /// </summary>
    public double VitalFewCategoryPercentage => Items.Count > 0
        ? (double)VitalFewCount / Items.Count * 100
        : 0;

    /// <summary>
    /// Gets the percentage of total count represented by vital few.
    /// </summary>
    public double VitalFewCountPercentage => TotalCount > 0
        ? (double)VitalFew.Sum(i => i.Count) / TotalCount * 100
        : 0;
}
