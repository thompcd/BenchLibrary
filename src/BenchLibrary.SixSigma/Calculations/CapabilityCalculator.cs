using BenchLibrary.SixSigma.Models;

namespace BenchLibrary.SixSigma.Calculations;

/// <summary>
/// Provides process capability calculations (Cp, Cpk, Pp, Ppk).
/// </summary>
public static class CapabilityCalculator
{
    /// <summary>
    /// Calculates process capability indices from a dataset.
    /// </summary>
    /// <param name="values">The measurement values.</param>
    /// <param name="lowerSpecLimit">The lower specification limit (LSL).</param>
    /// <param name="upperSpecLimit">The upper specification limit (USL).</param>
    /// <param name="target">Optional target value (defaults to midpoint of spec limits).</param>
    /// <returns>A <see cref="CapabilityResult"/> containing all capability metrics.</returns>
    /// <exception cref="ArgumentException">Thrown when values has fewer than 2 elements or USL &lt;= LSL.</exception>
    public static CapabilityResult Calculate(
        IEnumerable<double> values,
        double lowerSpecLimit,
        double upperSpecLimit,
        double? target = null)
    {
        var list = values.ToList();

        if (list.Count < 2)
        {
            throw new ArgumentException("Need at least 2 values to calculate capability.", nameof(values));
        }

        if (upperSpecLimit <= lowerSpecLimit)
        {
            throw new ArgumentException("Upper specification limit must be greater than lower specification limit.");
        }

        var mean = StatisticsCalculator.Mean(list);
        var stdDev = StatisticsCalculator.StandardDeviation(list);
        var specRange = upperSpecLimit - lowerSpecLimit;

        // Calculate Cp (potential capability)
        var cp = specRange / (6 * stdDev);

        // Calculate Cpk (actual capability considering centering)
        var cpupper = (upperSpecLimit - mean) / (3 * stdDev);
        var cplower = (mean - lowerSpecLimit) / (3 * stdDev);
        var cpk = Math.Min(cpupper, cplower);

        // Pp and Ppk use the same formulas with overall standard deviation
        // For this implementation, they are the same since we're using sample std dev
        var pp = cp;
        var ppk = cpk;

        // Estimate out-of-spec percentages using Z-scores
        var zLower = (lowerSpecLimit - mean) / stdDev;
        var zUpper = (upperSpecLimit - mean) / stdDev;
        var percentBelowLsl = NormalCdf(zLower) * 100;
        var percentAboveUsl = (1 - NormalCdf(zUpper)) * 100;

        return new CapabilityResult
        {
            Cp = cp,
            Cpk = cpk,
            Pp = pp,
            Ppk = ppk,
            LowerSpecLimit = lowerSpecLimit,
            UpperSpecLimit = upperSpecLimit,
            Target = target ?? (lowerSpecLimit + upperSpecLimit) / 2,
            Mean = mean,
            StandardDeviation = stdDev,
            SampleSize = list.Count,
            Minimum = list.Min(),
            Maximum = list.Max(),
            PercentBelowLsl = percentBelowLsl,
            PercentAboveUsl = percentAboveUsl
        };
    }

    /// <summary>
    /// Calculates Cpk from known mean and standard deviation.
    /// </summary>
    /// <param name="mean">The process mean.</param>
    /// <param name="standardDeviation">The process standard deviation.</param>
    /// <param name="lowerSpecLimit">The lower specification limit.</param>
    /// <param name="upperSpecLimit">The upper specification limit.</param>
    /// <returns>The Cpk value.</returns>
    public static double CalculateCpk(
        double mean,
        double standardDeviation,
        double lowerSpecLimit,
        double upperSpecLimit)
    {
        if (standardDeviation <= 0)
        {
            throw new ArgumentException("Standard deviation must be positive.", nameof(standardDeviation));
        }

        var cpupper = (upperSpecLimit - mean) / (3 * standardDeviation);
        var cplower = (mean - lowerSpecLimit) / (3 * standardDeviation);
        return Math.Min(cpupper, cplower);
    }

    /// <summary>
    /// Calculates Cp from known standard deviation.
    /// </summary>
    /// <param name="standardDeviation">The process standard deviation.</param>
    /// <param name="lowerSpecLimit">The lower specification limit.</param>
    /// <param name="upperSpecLimit">The upper specification limit.</param>
    /// <returns>The Cp value.</returns>
    public static double CalculateCp(
        double standardDeviation,
        double lowerSpecLimit,
        double upperSpecLimit)
    {
        if (standardDeviation <= 0)
        {
            throw new ArgumentException("Standard deviation must be positive.", nameof(standardDeviation));
        }

        var specRange = upperSpecLimit - lowerSpecLimit;
        return specRange / (6 * standardDeviation);
    }

    /// <summary>
    /// Approximation of the standard normal cumulative distribution function.
    /// </summary>
    /// <param name="z">The z-score.</param>
    /// <returns>The cumulative probability P(Z &lt;= z).</returns>
    private static double NormalCdf(double z)
    {
        // Approximation using the error function
        const double a1 = 0.254829592;
        const double a2 = -0.284496736;
        const double a3 = 1.421413741;
        const double a4 = -1.453152027;
        const double a5 = 1.061405429;
        const double p = 0.3275911;

        var sign = z < 0 ? -1 : 1;
        z = Math.Abs(z) / Math.Sqrt(2);

        var t = 1.0 / (1.0 + p * z);
        var y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-z * z);

        return 0.5 * (1.0 + sign * y);
    }
}
