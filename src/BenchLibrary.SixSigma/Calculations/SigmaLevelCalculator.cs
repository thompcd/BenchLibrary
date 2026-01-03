using BenchLibrary.SixSigma.Models;

namespace BenchLibrary.SixSigma.Calculations;

/// <summary>
/// Provides sigma level and DPMO calculations.
/// </summary>
public static class SigmaLevelCalculator
{
    /// <summary>
    /// Calculates the sigma level from defect data.
    /// </summary>
    /// <param name="totalUnits">The total number of units produced.</param>
    /// <param name="totalDefects">The total number of defects found.</param>
    /// <param name="opportunitiesPerUnit">The number of defect opportunities per unit.</param>
    /// <returns>A <see cref="SigmaLevelResult"/> with the calculated metrics.</returns>
    /// <exception cref="ArgumentException">Thrown when inputs are invalid.</exception>
    public static SigmaLevelResult Calculate(long totalUnits, long totalDefects, int opportunitiesPerUnit)
    {
        if (totalUnits <= 0)
        {
            throw new ArgumentException("Total units must be positive.", nameof(totalUnits));
        }

        if (totalDefects < 0)
        {
            throw new ArgumentException("Total defects cannot be negative.", nameof(totalDefects));
        }

        if (opportunitiesPerUnit <= 0)
        {
            throw new ArgumentException("Opportunities per unit must be positive.", nameof(opportunitiesPerUnit));
        }

        var totalOpportunities = totalUnits * opportunitiesPerUnit;
        var dpmo = (double)totalDefects / totalOpportunities * 1_000_000;
        var yield = (1 - (double)totalDefects / totalOpportunities) * 100;
        var sigmaLevel = DpmoToSigma(dpmo);

        return new SigmaLevelResult
        {
            SigmaLevel = sigmaLevel,
            Dpmo = dpmo,
            Yield = yield,
            TotalOpportunities = totalOpportunities,
            TotalDefects = totalDefects,
            TotalUnits = totalUnits,
            OpportunitiesPerUnit = opportunitiesPerUnit
        };
    }

    /// <summary>
    /// Calculates the sigma level from DPMO.
    /// </summary>
    /// <param name="dpmo">Defects per million opportunities.</param>
    /// <returns>The sigma level.</returns>
    public static double DpmoToSigma(double dpmo)
    {
        if (dpmo <= 0)
        {
            return 6.0; // Perfect process
        }

        if (dpmo >= 1_000_000)
        {
            return 0; // All defects
        }

        // Convert DPMO to defect rate
        var defectRate = dpmo / 1_000_000;

        // Use inverse normal CDF (approximation)
        var zScore = InverseNormalCdf(1 - defectRate);

        // Add 1.5 sigma shift (industry standard)
        return zScore + 1.5;
    }

    /// <summary>
    /// Calculates DPMO from sigma level.
    /// </summary>
    /// <param name="sigmaLevel">The sigma level.</param>
    /// <returns>The DPMO value.</returns>
    public static double SigmaToDpmo(double sigmaLevel)
    {
        if (sigmaLevel >= 6)
        {
            return 3.4; // Standard Six Sigma value
        }

        // Remove 1.5 sigma shift
        var zScore = sigmaLevel - 1.5;

        // Use normal CDF
        var defectRate = 1 - NormalCdf(zScore);

        return defectRate * 1_000_000;
    }

    /// <summary>
    /// Calculates yield from sigma level.
    /// </summary>
    /// <param name="sigmaLevel">The sigma level.</param>
    /// <returns>The yield percentage.</returns>
    public static double SigmaToYield(double sigmaLevel)
    {
        var dpmo = SigmaToDpmo(sigmaLevel);
        return 100 - (dpmo / 10_000);
    }

    /// <summary>
    /// Gets the standard sigma level benchmarks.
    /// </summary>
    /// <returns>Dictionary of sigma levels to their DPMO and yield values.</returns>
    public static IDictionary<double, (double Dpmo, double Yield)> GetBenchmarks()
    {
        return new Dictionary<double, (double Dpmo, double Yield)>
        {
            { 1.0, (691_462, 30.85) },
            { 2.0, (308_538, 69.15) },
            { 3.0, (66_807, 93.32) },
            { 4.0, (6_210, 99.38) },
            { 5.0, (233, 99.977) },
            { 6.0, (3.4, 99.99966) }
        };
    }

    /// <summary>
    /// Approximation of the standard normal cumulative distribution function.
    /// </summary>
    private static double NormalCdf(double z)
    {
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

    /// <summary>
    /// Approximation of the inverse standard normal CDF.
    /// </summary>
    private static double InverseNormalCdf(double p)
    {
        if (p <= 0)
        {
            return double.NegativeInfinity;
        }

        if (p >= 1)
        {
            return double.PositiveInfinity;
        }

        // Rational approximation for central region
        const double a1 = -39.6968302866538;
        const double a2 = 220.946098424521;
        const double a3 = -275.928510446969;
        const double a4 = 138.357751867269;
        const double a5 = -30.6647980661472;
        const double a6 = 2.50662823884;
        const double b1 = -54.4760987982241;
        const double b2 = 161.585836858041;
        const double b3 = -155.698979859887;
        const double b4 = 66.8013118877197;
        const double b5 = -13.2806815528857;

        var q = p - 0.5;

        if (Math.Abs(q) <= 0.425)
        {
            var r = 0.180625 - q * q;
            return q * (((((((a1 * r + a2) * r + a3) * r + a4) * r + a5) * r + a6) * r + 1) /
                       (((((((b1 * r + b2) * r + b3) * r + b4) * r + b5) * r + 1) * r + 1)));
        }

        var r2 = q < 0 ? p : 1 - p;
        r2 = Math.Sqrt(-Math.Log(r2));

        double result;
        if (r2 <= 5)
        {
            r2 -= 1.6;
            result = (((((((7.74545014278341e-4 * r2 + 2.27238449892692e-2) * r2 + 2.41780725177450e-1) * r2 +
                          1.27045825245236) * r2 + 3.64784832476320) * r2 + 5.76949722146069) * r2 +
                        4.63033784615655) * r2 + 1.42343711074968) /
                      (((((((1.05075007164442e-9 * r2 + 5.47593808499534e-4) * r2 + 1.51986665636165e-2) * r2 +
                           1.48103976427480e-1) * r2 + 6.89767334985100e-1) * r2 + 1.67638483018380) * r2 +
                         2.05319162663776) * r2 + 1);
        }
        else
        {
            r2 -= 5;
            result = (((((((2.01033439929228e-7 * r2 + 2.71155556874349e-5) * r2 + 1.24266094738808e-3) * r2 +
                          2.65321895265761e-2) * r2 + 2.96560571828504e-1) * r2 + 1.78482653991729) * r2 +
                        5.46378491116411) * r2 + 6.65790464350111) /
                      (((((((2.04426310338994e-15 * r2 + 1.42151175831645e-7) * r2 + 1.84631831751005e-5) * r2 +
                           7.86869131145613e-4) * r2 + 1.48753612908506e-2) * r2 + 1.36929880922735e-1) * r2 +
                         5.99832206555888e-1) * r2 + 1);
        }

        return q < 0 ? -result : result;
    }
}
