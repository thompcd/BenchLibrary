using BenchLibrary.SixSigma.Calculations;
using BenchLibrary.SixSigma.Models;

namespace BenchLibrary.Web.Services;

/// <summary>
/// Service for SPC calculations and data processing.
/// </summary>
public class SpcDataService
{
    private readonly SimulatedDataService _simulatedData;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpcDataService"/> class.
    /// </summary>
    /// <param name="simulatedData">The simulated data service.</param>
    public SpcDataService(SimulatedDataService simulatedData)
    {
        _simulatedData = simulatedData;
    }

    /// <summary>
    /// Gets X-bar and R chart results for simulated data.
    /// </summary>
    /// <param name="mean">Target mean.</param>
    /// <param name="stdDev">Standard deviation.</param>
    /// <param name="subgroupSize">Subgroup size.</param>
    /// <param name="subgroupCount">Number of subgroups.</param>
    /// <returns>Tuple of X-bar and R chart results.</returns>
    public (ControlChartResult XBar, ControlChartResult R) GetControlCharts(
        double mean = 5.0,
        double stdDev = 0.1,
        int subgroupSize = 5,
        int subgroupCount = 25)
    {
        var subgroups = _simulatedData.GenerateSubgroups(mean, stdDev, subgroupSize, subgroupCount);
        var xbar = ControlChartCalculator.CalculateXBarChart(subgroups);
        var r = ControlChartCalculator.CalculateRChart(subgroups);
        return (xbar, r);
    }

    /// <summary>
    /// Gets control charts with a simulated process shift.
    /// </summary>
    public (ControlChartResult XBar, ControlChartResult R) GetControlChartsWithShift(
        double mean = 5.0,
        double stdDev = 0.1,
        int subgroupSize = 5,
        int subgroupCount = 25,
        int shiftAtSubgroup = 15,
        double shiftAmount = 0.3)
    {
        var subgroups = _simulatedData.GenerateSubgroupsWithShift(
            mean, stdDev, subgroupSize, subgroupCount, shiftAtSubgroup, shiftAmount);
        var xbar = ControlChartCalculator.CalculateXBarChart(subgroups);
        var r = ControlChartCalculator.CalculateRChart(subgroups);
        return (xbar, r);
    }

    /// <summary>
    /// Gets process capability analysis for simulated data.
    /// </summary>
    /// <param name="mean">Target mean.</param>
    /// <param name="stdDev">Standard deviation.</param>
    /// <param name="lsl">Lower specification limit.</param>
    /// <param name="usl">Upper specification limit.</param>
    /// <param name="count">Number of measurements.</param>
    /// <returns>Capability analysis result.</returns>
    public CapabilityResult GetCapabilityAnalysis(
        double mean = 5.0,
        double stdDev = 0.1,
        double lsl = 4.5,
        double usl = 5.5,
        int count = 100)
    {
        var measurements = _simulatedData.GenerateMeasurements(mean, stdDev, count);
        return CapabilityCalculator.Calculate(measurements, lsl, usl);
    }

    /// <summary>
    /// Gets Pareto analysis for simulated defect data.
    /// </summary>
    /// <returns>Pareto analysis result.</returns>
    public ParetoResult GetParetoAnalysis()
    {
        var defects = _simulatedData.GenerateDefectData();
        return ParetoCalculator.Analyze(defects, "Production Defects");
    }

    /// <summary>
    /// Calculates sigma level from production metrics.
    /// </summary>
    /// <param name="units">Total units produced.</param>
    /// <param name="defects">Total defects found.</param>
    /// <param name="opportunities">Opportunities per unit.</param>
    /// <returns>Sigma level result.</returns>
    public SigmaLevelResult GetSigmaLevel(long units = 10000, long defects = 62, int opportunities = 10)
    {
        return SigmaLevelCalculator.Calculate(units, defects, opportunities);
    }
}
