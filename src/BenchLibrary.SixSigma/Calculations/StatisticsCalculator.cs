namespace BenchLibrary.SixSigma.Calculations;

/// <summary>
/// Provides basic statistical calculations.
/// </summary>
public static class StatisticsCalculator
{
    /// <summary>
    /// Calculates the arithmetic mean of a collection of values.
    /// </summary>
    /// <param name="values">The values to average.</param>
    /// <returns>The arithmetic mean.</returns>
    /// <exception cref="ArgumentException">Thrown when values is empty.</exception>
    public static double Mean(IEnumerable<double> values)
    {
        var list = values.ToList();
        if (list.Count == 0)
        {
            throw new ArgumentException("Cannot calculate mean of empty collection.", nameof(values));
        }

        return list.Average();
    }

    /// <summary>
    /// Calculates the sample standard deviation.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns>The sample standard deviation.</returns>
    /// <exception cref="ArgumentException">Thrown when values has fewer than 2 elements.</exception>
    public static double StandardDeviation(IEnumerable<double> values)
    {
        var list = values.ToList();
        if (list.Count < 2)
        {
            throw new ArgumentException("Need at least 2 values to calculate standard deviation.", nameof(values));
        }

        var mean = list.Average();
        var sumOfSquares = list.Sum(v => (v - mean) * (v - mean));
        return Math.Sqrt(sumOfSquares / (list.Count - 1));
    }

    /// <summary>
    /// Calculates the population standard deviation.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns>The population standard deviation.</returns>
    /// <exception cref="ArgumentException">Thrown when values is empty.</exception>
    public static double PopulationStandardDeviation(IEnumerable<double> values)
    {
        var list = values.ToList();
        if (list.Count == 0)
        {
            throw new ArgumentException("Cannot calculate standard deviation of empty collection.", nameof(values));
        }

        var mean = list.Average();
        var sumOfSquares = list.Sum(v => (v - mean) * (v - mean));
        return Math.Sqrt(sumOfSquares / list.Count);
    }

    /// <summary>
    /// Calculates the variance of a sample.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns>The sample variance.</returns>
    public static double Variance(IEnumerable<double> values)
    {
        var stdDev = StandardDeviation(values);
        return stdDev * stdDev;
    }

    /// <summary>
    /// Calculates the range (max - min) of a collection.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns>The range.</returns>
    /// <exception cref="ArgumentException">Thrown when values is empty.</exception>
    public static double Range(IEnumerable<double> values)
    {
        var list = values.ToList();
        if (list.Count == 0)
        {
            throw new ArgumentException("Cannot calculate range of empty collection.", nameof(values));
        }

        return list.Max() - list.Min();
    }

    /// <summary>
    /// Calculates the median of a collection.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns>The median value.</returns>
    /// <exception cref="ArgumentException">Thrown when values is empty.</exception>
    public static double Median(IEnumerable<double> values)
    {
        var sorted = values.OrderBy(v => v).ToList();
        if (sorted.Count == 0)
        {
            throw new ArgumentException("Cannot calculate median of empty collection.", nameof(values));
        }

        var mid = sorted.Count / 2;
        return sorted.Count % 2 == 0
            ? (sorted[mid - 1] + sorted[mid]) / 2.0
            : sorted[mid];
    }

    /// <summary>
    /// Calculates the moving range for individual measurements.
    /// </summary>
    /// <param name="values">The sequential values.</param>
    /// <returns>A collection of moving range values.</returns>
    public static IEnumerable<double> MovingRange(IEnumerable<double> values)
    {
        var list = values.ToList();
        for (var i = 1; i < list.Count; i++)
        {
            yield return Math.Abs(list[i] - list[i - 1]);
        }
    }

    /// <summary>
    /// Calculates the average moving range.
    /// </summary>
    /// <param name="values">The sequential values.</param>
    /// <returns>The average moving range.</returns>
    public static double AverageMovingRange(IEnumerable<double> values)
    {
        var movingRanges = MovingRange(values).ToList();
        return movingRanges.Count > 0 ? movingRanges.Average() : 0;
    }

    /// <summary>
    /// Estimates sigma from the average range using the d2 constant.
    /// </summary>
    /// <param name="averageRange">The average range (R-bar).</param>
    /// <param name="subgroupSize">The subgroup size.</param>
    /// <returns>The estimated sigma.</returns>
    public static double EstimateSigmaFromRange(double averageRange, int subgroupSize)
    {
        var d2 = GetD2Constant(subgroupSize);
        return averageRange / d2;
    }

    /// <summary>
    /// Gets the d2 constant for a given subgroup size.
    /// </summary>
    /// <param name="subgroupSize">The subgroup size (2-10).</param>
    /// <returns>The d2 constant.</returns>
    public static double GetD2Constant(int subgroupSize)
    {
        // d2 constants for subgroup sizes 2-10
        return subgroupSize switch
        {
            2 => 1.128,
            3 => 1.693,
            4 => 2.059,
            5 => 2.326,
            6 => 2.534,
            7 => 2.704,
            8 => 2.847,
            9 => 2.970,
            10 => 3.078,
            _ => throw new ArgumentOutOfRangeException(nameof(subgroupSize), "Subgroup size must be between 2 and 10.")
        };
    }

    /// <summary>
    /// Gets the d3 constant for a given subgroup size.
    /// </summary>
    /// <param name="subgroupSize">The subgroup size (2-10).</param>
    /// <returns>The d3 constant.</returns>
    public static double GetD3Constant(int subgroupSize)
    {
        return subgroupSize switch
        {
            2 => 0.853,
            3 => 0.888,
            4 => 0.880,
            5 => 0.864,
            6 => 0.848,
            7 => 0.833,
            8 => 0.820,
            9 => 0.808,
            10 => 0.797,
            _ => throw new ArgumentOutOfRangeException(nameof(subgroupSize), "Subgroup size must be between 2 and 10.")
        };
    }

    /// <summary>
    /// Gets the A2 constant for X-bar chart control limits.
    /// </summary>
    /// <param name="subgroupSize">The subgroup size (2-10).</param>
    /// <returns>The A2 constant.</returns>
    public static double GetA2Constant(int subgroupSize)
    {
        return subgroupSize switch
        {
            2 => 1.880,
            3 => 1.023,
            4 => 0.729,
            5 => 0.577,
            6 => 0.483,
            7 => 0.419,
            8 => 0.373,
            9 => 0.337,
            10 => 0.308,
            _ => throw new ArgumentOutOfRangeException(nameof(subgroupSize), "Subgroup size must be between 2 and 10.")
        };
    }

    /// <summary>
    /// Gets the D3 constant for R chart lower control limit.
    /// </summary>
    /// <param name="subgroupSize">The subgroup size (2-10).</param>
    /// <returns>The D3 constant (0 for subgroups less than 7).</returns>
    public static double GetD3LclConstant(int subgroupSize)
    {
        return subgroupSize switch
        {
            2 => 0,
            3 => 0,
            4 => 0,
            5 => 0,
            6 => 0,
            7 => 0.076,
            8 => 0.136,
            9 => 0.184,
            10 => 0.223,
            _ => throw new ArgumentOutOfRangeException(nameof(subgroupSize), "Subgroup size must be between 2 and 10.")
        };
    }

    /// <summary>
    /// Gets the D4 constant for R chart upper control limit.
    /// </summary>
    /// <param name="subgroupSize">The subgroup size (2-10).</param>
    /// <returns>The D4 constant.</returns>
    public static double GetD4Constant(int subgroupSize)
    {
        return subgroupSize switch
        {
            2 => 3.267,
            3 => 2.574,
            4 => 2.282,
            5 => 2.114,
            6 => 2.004,
            7 => 1.924,
            8 => 1.864,
            9 => 1.816,
            10 => 1.777,
            _ => throw new ArgumentOutOfRangeException(nameof(subgroupSize), "Subgroup size must be between 2 and 10.")
        };
    }
}
