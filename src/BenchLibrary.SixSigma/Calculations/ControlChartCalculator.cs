using BenchLibrary.SixSigma.Models;

namespace BenchLibrary.SixSigma.Calculations;

/// <summary>
/// Provides control chart calculations for X-bar and R charts.
/// </summary>
public static class ControlChartCalculator
{
    /// <summary>
    /// Calculates X-bar (mean) chart from subgrouped data.
    /// </summary>
    /// <param name="subgroups">The subgroups of measurements.</param>
    /// <returns>A <see cref="ControlChartResult"/> for the X-bar chart.</returns>
    /// <exception cref="ArgumentException">Thrown when subgroups is empty or has inconsistent sizes.</exception>
    public static ControlChartResult CalculateXBarChart(IEnumerable<IEnumerable<double>> subgroups)
    {
        var subgroupList = subgroups.Select(s => s.ToList()).ToList();

        if (subgroupList.Count == 0)
        {
            throw new ArgumentException("Must provide at least one subgroup.", nameof(subgroups));
        }

        var subgroupSize = subgroupList[0].Count;
        if (subgroupList.Any(s => s.Count != subgroupSize))
        {
            throw new ArgumentException("All subgroups must have the same size.", nameof(subgroups));
        }

        if (subgroupSize < 2 || subgroupSize > 10)
        {
            throw new ArgumentException("Subgroup size must be between 2 and 10.", nameof(subgroups));
        }

        // Calculate subgroup means and ranges
        var means = subgroupList.Select(s => s.Average()).ToList();
        var ranges = subgroupList.Select(s => s.Max() - s.Min()).ToList();

        // Calculate overall statistics
        var xBarBar = means.Average();
        var rBar = ranges.Average();

        // Get constants
        var a2 = StatisticsCalculator.GetA2Constant(subgroupSize);

        // Calculate control limits
        var ucl = xBarBar + a2 * rBar;
        var lcl = xBarBar - a2 * rBar;

        // Estimate sigma
        var d2 = StatisticsCalculator.GetD2Constant(subgroupSize);
        var estimatedSigma = rBar / d2;

        // Build points
        var points = new List<ControlChartPoint>();
        for (var i = 0; i < means.Count; i++)
        {
            var point = new ControlChartPoint
            {
                SampleNumber = i + 1,
                Value = means[i],
                CenterLine = xBarBar,
                UpperControlLimit = ucl,
                LowerControlLimit = lcl,
                Timestamp = DateTime.UtcNow.AddMinutes(-means.Count + i)
            };

            // Check for violations
            CheckViolations(point, means, i);
            points.Add(point);
        }

        return new ControlChartResult
        {
            ChartType = ControlChartType.XBar,
            CenterLine = xBarBar,
            UpperControlLimit = ucl,
            LowerControlLimit = lcl,
            Points = points,
            SubgroupSize = subgroupSize,
            SubgroupCount = subgroupList.Count,
            EstimatedSigma = estimatedSigma,
            ProcessMean = xBarBar,
            AverageRange = rBar
        };
    }

    /// <summary>
    /// Calculates R (range) chart from subgrouped data.
    /// </summary>
    /// <param name="subgroups">The subgroups of measurements.</param>
    /// <returns>A <see cref="ControlChartResult"/> for the R chart.</returns>
    public static ControlChartResult CalculateRChart(IEnumerable<IEnumerable<double>> subgroups)
    {
        var subgroupList = subgroups.Select(s => s.ToList()).ToList();

        if (subgroupList.Count == 0)
        {
            throw new ArgumentException("Must provide at least one subgroup.", nameof(subgroups));
        }

        var subgroupSize = subgroupList[0].Count;
        if (subgroupList.Any(s => s.Count != subgroupSize))
        {
            throw new ArgumentException("All subgroups must have the same size.", nameof(subgroups));
        }

        // Calculate subgroup ranges
        var ranges = subgroupList.Select(s => s.Max() - s.Min()).ToList();
        var rBar = ranges.Average();

        // Get constants
        var d3 = StatisticsCalculator.GetD3LclConstant(subgroupSize);
        var d4 = StatisticsCalculator.GetD4Constant(subgroupSize);

        // Calculate control limits
        var ucl = d4 * rBar;
        var lcl = d3 * rBar;

        // Build points
        var points = new List<ControlChartPoint>();
        for (var i = 0; i < ranges.Count; i++)
        {
            var point = new ControlChartPoint
            {
                SampleNumber = i + 1,
                Value = ranges[i],
                CenterLine = rBar,
                UpperControlLimit = ucl,
                LowerControlLimit = lcl,
                Timestamp = DateTime.UtcNow.AddMinutes(-ranges.Count + i)
            };
            points.Add(point);
        }

        return new ControlChartResult
        {
            ChartType = ControlChartType.Range,
            CenterLine = rBar,
            UpperControlLimit = ucl,
            LowerControlLimit = lcl,
            Points = points,
            SubgroupSize = subgroupSize,
            SubgroupCount = subgroupList.Count,
            AverageRange = rBar
        };
    }

    /// <summary>
    /// Calculates Individuals (I) chart for single measurements.
    /// </summary>
    /// <param name="values">The individual measurements.</param>
    /// <returns>A <see cref="ControlChartResult"/> for the Individuals chart.</returns>
    public static ControlChartResult CalculateIndividualsChart(IEnumerable<double> values)
    {
        var list = values.ToList();

        if (list.Count < 2)
        {
            throw new ArgumentException("Need at least 2 values for Individuals chart.", nameof(values));
        }

        var mean = list.Average();
        var movingRanges = StatisticsCalculator.MovingRange(list).ToList();
        var mrBar = movingRanges.Average();

        // For individuals chart, use d2 = 1.128 (moving range of 2)
        var estimatedSigma = mrBar / 1.128;

        var ucl = mean + 3 * estimatedSigma;
        var lcl = mean - 3 * estimatedSigma;

        var points = new List<ControlChartPoint>();
        for (var i = 0; i < list.Count; i++)
        {
            var point = new ControlChartPoint
            {
                SampleNumber = i + 1,
                Value = list[i],
                CenterLine = mean,
                UpperControlLimit = ucl,
                LowerControlLimit = lcl,
                Timestamp = DateTime.UtcNow.AddMinutes(-list.Count + i)
            };
            points.Add(point);
        }

        return new ControlChartResult
        {
            ChartType = ControlChartType.Individuals,
            CenterLine = mean,
            UpperControlLimit = ucl,
            LowerControlLimit = lcl,
            Points = points,
            SubgroupSize = 1,
            SubgroupCount = list.Count,
            EstimatedSigma = estimatedSigma,
            ProcessMean = mean,
            AverageRange = mrBar
        };
    }

    /// <summary>
    /// Calculates Moving Range (MR) chart for single measurements.
    /// </summary>
    /// <param name="values">The individual measurements.</param>
    /// <returns>A <see cref="ControlChartResult"/> for the Moving Range chart.</returns>
    public static ControlChartResult CalculateMovingRangeChart(IEnumerable<double> values)
    {
        var list = values.ToList();

        if (list.Count < 2)
        {
            throw new ArgumentException("Need at least 2 values for Moving Range chart.", nameof(values));
        }

        var movingRanges = StatisticsCalculator.MovingRange(list).ToList();
        var mrBar = movingRanges.Average();

        // D3 = 0 and D4 = 3.267 for moving range of 2
        var ucl = 3.267 * mrBar;
        var lcl = 0;

        var points = new List<ControlChartPoint>();
        for (var i = 0; i < movingRanges.Count; i++)
        {
            var point = new ControlChartPoint
            {
                SampleNumber = i + 2, // MR starts at point 2
                Value = movingRanges[i],
                CenterLine = mrBar,
                UpperControlLimit = ucl,
                LowerControlLimit = lcl,
                Timestamp = DateTime.UtcNow.AddMinutes(-movingRanges.Count + i)
            };
            points.Add(point);
        }

        return new ControlChartResult
        {
            ChartType = ControlChartType.MovingRange,
            CenterLine = mrBar,
            UpperControlLimit = ucl,
            LowerControlLimit = lcl,
            Points = points,
            SubgroupSize = 2,
            SubgroupCount = movingRanges.Count,
            AverageRange = mrBar
        };
    }

    /// <summary>
    /// Checks for Western Electric rule violations.
    /// </summary>
    private static void CheckViolations(ControlChartPoint point, List<double> allValues, int currentIndex)
    {
        // Rule 1: Point beyond 3 sigma
        if (point.IsOutOfControl)
        {
            point.Violations.Add(new ControlChartViolation
            {
                RuleNumber = 1,
                Description = "Point beyond 3 sigma control limit",
                Severity = ViolationSeverity.Critical
            });
        }

        // Additional rules can be implemented here (runs, trends, etc.)
    }
}
