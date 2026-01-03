namespace BenchLibrary.Web.Services;

/// <summary>
/// Service for generating simulated manufacturing data for demo purposes.
/// </summary>
public class SimulatedDataService
{
    private readonly Random _random = new();

    /// <summary>
    /// Generates simulated measurement data with normal distribution.
    /// </summary>
    /// <param name="mean">The target mean value.</param>
    /// <param name="standardDeviation">The standard deviation.</param>
    /// <param name="count">Number of measurements to generate.</param>
    /// <returns>A list of simulated measurements.</returns>
    public List<double> GenerateMeasurements(double mean, double standardDeviation, int count)
    {
        var measurements = new List<double>();
        for (var i = 0; i < count; i++)
        {
            measurements.Add(GenerateNormalValue(mean, standardDeviation));
        }
        return measurements;
    }

    /// <summary>
    /// Generates subgroups of simulated measurements.
    /// </summary>
    /// <param name="mean">The target mean value.</param>
    /// <param name="standardDeviation">The standard deviation.</param>
    /// <param name="subgroupSize">Size of each subgroup.</param>
    /// <param name="subgroupCount">Number of subgroups to generate.</param>
    /// <returns>A list of subgroups, each containing measurement values.</returns>
    public List<List<double>> GenerateSubgroups(double mean, double standardDeviation, int subgroupSize, int subgroupCount)
    {
        var subgroups = new List<List<double>>();
        for (var i = 0; i < subgroupCount; i++)
        {
            var subgroup = GenerateMeasurements(mean, standardDeviation, subgroupSize);
            subgroups.Add(subgroup);
        }
        return subgroups;
    }

    /// <summary>
    /// Generates subgroups with a simulated process shift.
    /// </summary>
    /// <param name="mean">The target mean value.</param>
    /// <param name="standardDeviation">The standard deviation.</param>
    /// <param name="subgroupSize">Size of each subgroup.</param>
    /// <param name="subgroupCount">Number of subgroups to generate.</param>
    /// <param name="shiftAtSubgroup">Subgroup index where shift occurs.</param>
    /// <param name="shiftAmount">Amount of mean shift.</param>
    /// <returns>A list of subgroups with a process shift.</returns>
    public List<List<double>> GenerateSubgroupsWithShift(
        double mean,
        double standardDeviation,
        int subgroupSize,
        int subgroupCount,
        int shiftAtSubgroup,
        double shiftAmount)
    {
        var subgroups = new List<List<double>>();
        for (var i = 0; i < subgroupCount; i++)
        {
            var currentMean = i >= shiftAtSubgroup ? mean + shiftAmount : mean;
            var subgroup = GenerateMeasurements(currentMean, standardDeviation, subgroupSize);
            subgroups.Add(subgroup);
        }
        return subgroups;
    }

    /// <summary>
    /// Generates simulated defect data for Pareto analysis.
    /// </summary>
    /// <returns>Dictionary of defect types and their counts.</returns>
    public Dictionary<string, int> GenerateDefectData()
    {
        return new Dictionary<string, int>
        {
            { "Solder Bridge", _random.Next(80, 120) },
            { "Missing Component", _random.Next(40, 60) },
            { "Cold Joint", _random.Next(30, 50) },
            { "Tombstone", _random.Next(20, 35) },
            { "Misalignment", _random.Next(15, 25) },
            { "Polarity Reversal", _random.Next(5, 15) },
            { "Cracked Component", _random.Next(3, 10) },
            { "Foreign Material", _random.Next(2, 8) }
        };
    }

    /// <summary>
    /// Generates a single value from a normal distribution using Box-Muller transform.
    /// </summary>
    private double GenerateNormalValue(double mean, double standardDeviation)
    {
        var u1 = 1.0 - _random.NextDouble();
        var u2 = 1.0 - _random.NextDouble();
        var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + standardDeviation * randStdNormal;
    }
}
