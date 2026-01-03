using BenchLibrary.SixSigma.Models;

namespace BenchLibrary.SixSigma.Calculations;

/// <summary>
/// Provides Pareto analysis calculations.
/// </summary>
public static class ParetoCalculator
{
    /// <summary>
    /// Performs a Pareto analysis from category-count pairs.
    /// </summary>
    /// <param name="data">Dictionary of category names to counts.</param>
    /// <param name="title">Optional title for the analysis.</param>
    /// <returns>A <see cref="ParetoResult"/> with items sorted by frequency.</returns>
    public static ParetoResult Analyze(IDictionary<string, int> data, string? title = null)
    {
        if (data.Count == 0)
        {
            return new ParetoResult
            {
                Title = title,
                Items = new List<ParetoItem>()
            };
        }

        var totalCount = data.Values.Sum();
        var cumulativePercentage = 0.0;
        var rank = 0;

        var items = data
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp =>
            {
                var percentage = totalCount > 0 ? (double)kvp.Value / totalCount * 100 : 0;
                cumulativePercentage += percentage;
                rank++;

                return new ParetoItem
                {
                    Category = kvp.Key,
                    Count = kvp.Value,
                    Percentage = percentage,
                    CumulativePercentage = cumulativePercentage,
                    Rank = rank
                };
            })
            .ToList();

        return new ParetoResult
        {
            Title = title,
            Items = items
        };
    }

    /// <summary>
    /// Performs a Pareto analysis from a collection of items with categories.
    /// </summary>
    /// <typeparam name="T">The type of items.</typeparam>
    /// <param name="items">The collection of items.</param>
    /// <param name="categorySelector">Function to select the category from each item.</param>
    /// <param name="title">Optional title for the analysis.</param>
    /// <returns>A <see cref="ParetoResult"/> with items sorted by frequency.</returns>
    public static ParetoResult Analyze<T>(
        IEnumerable<T> items,
        Func<T, string> categorySelector,
        string? title = null)
    {
        var groups = items
            .GroupBy(categorySelector)
            .ToDictionary(g => g.Key, g => g.Count());

        return Analyze(groups, title);
    }

    /// <summary>
    /// Performs a Pareto analysis from a collection of items with categories and weights.
    /// </summary>
    /// <typeparam name="T">The type of items.</typeparam>
    /// <param name="items">The collection of items.</param>
    /// <param name="categorySelector">Function to select the category from each item.</param>
    /// <param name="weightSelector">Function to select the weight from each item.</param>
    /// <param name="title">Optional title for the analysis.</param>
    /// <returns>A <see cref="ParetoResult"/> with items sorted by weighted frequency.</returns>
    public static ParetoResult AnalyzeWeighted<T>(
        IEnumerable<T> items,
        Func<T, string> categorySelector,
        Func<T, int> weightSelector,
        string? title = null)
    {
        var groups = items
            .GroupBy(categorySelector)
            .ToDictionary(g => g.Key, g => g.Sum(weightSelector));

        return Analyze(groups, title);
    }

    /// <summary>
    /// Gets the vital few categories (those contributing to 80% of the total).
    /// </summary>
    /// <param name="data">Dictionary of category names to counts.</param>
    /// <returns>A list of category names in the vital few.</returns>
    public static IList<string> GetVitalFew(IDictionary<string, int> data)
    {
        var result = Analyze(data);
        return result.VitalFew.Select(i => i.Category).ToList();
    }

    /// <summary>
    /// Gets the trivial many categories (those contributing to 20% of the total).
    /// </summary>
    /// <param name="data">Dictionary of category names to counts.</param>
    /// <returns>A list of category names in the trivial many.</returns>
    public static IList<string> GetTrivialMany(IDictionary<string, int> data)
    {
        var result = Analyze(data);
        return result.TrivialMany.Select(i => i.Category).ToList();
    }
}
