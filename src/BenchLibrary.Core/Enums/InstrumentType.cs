namespace BenchLibrary.Core.Enums;

/// <summary>
/// Defines the type of test instrument used in manufacturing and quality processes.
/// </summary>
public enum InstrumentType
{
    /// <summary>
    /// Unknown or unspecified instrument type.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Digital or analog multimeter for voltage, current, and resistance measurements.
    /// </summary>
    Multimeter = 1,

    /// <summary>
    /// Oscilloscope for waveform analysis and signal measurement.
    /// </summary>
    Oscilloscope = 2,

    /// <summary>
    /// Power supply for providing regulated voltage and current.
    /// </summary>
    PowerSupply = 3,

    /// <summary>
    /// Function or signal generator for producing test signals.
    /// </summary>
    FunctionGenerator = 4,

    /// <summary>
    /// Spectrum analyzer for frequency domain analysis.
    /// </summary>
    SpectrumAnalyzer = 5,

    /// <summary>
    /// Logic analyzer for digital signal analysis.
    /// </summary>
    LogicAnalyzer = 6,

    /// <summary>
    /// Electronic load for testing power sources under load.
    /// </summary>
    ElectronicLoad = 7,

    /// <summary>
    /// LCR meter for inductance, capacitance, and resistance measurements.
    /// </summary>
    LcrMeter = 8,

    /// <summary>
    /// Network analyzer for RF and microwave measurements.
    /// </summary>
    NetworkAnalyzer = 9,

    /// <summary>
    /// Temperature controller or chamber for environmental testing.
    /// </summary>
    TemperatureController = 10,

    /// <summary>
    /// Custom or specialized instrument not covered by other types.
    /// </summary>
    Custom = 99
}
