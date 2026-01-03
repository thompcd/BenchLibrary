namespace BenchLibrary.Core.Enums;

/// <summary>
/// Defines standard units of measurement used in electronic testing.
/// </summary>
public enum MeasurementUnit
{
    /// <summary>
    /// Dimensionless quantity or unknown unit.
    /// </summary>
    None = 0,

    // Electrical
    /// <summary>Volts (V) - electrical potential.</summary>
    Volts = 1,
    /// <summary>Millivolts (mV) - 10^-3 volts.</summary>
    Millivolts = 2,
    /// <summary>Microvolts (µV) - 10^-6 volts.</summary>
    Microvolts = 3,
    /// <summary>Amperes (A) - electrical current.</summary>
    Amperes = 4,
    /// <summary>Milliamperes (mA) - 10^-3 amperes.</summary>
    Milliamperes = 5,
    /// <summary>Microamperes (µA) - 10^-6 amperes.</summary>
    Microamperes = 6,
    /// <summary>Ohms (Ω) - electrical resistance.</summary>
    Ohms = 7,
    /// <summary>Kiloohms (kΩ) - 10^3 ohms.</summary>
    Kiloohms = 8,
    /// <summary>Megaohms (MΩ) - 10^6 ohms.</summary>
    Megaohms = 9,
    /// <summary>Watts (W) - electrical power.</summary>
    Watts = 10,
    /// <summary>Milliwatts (mW) - 10^-3 watts.</summary>
    Milliwatts = 11,

    // Frequency
    /// <summary>Hertz (Hz) - frequency.</summary>
    Hertz = 20,
    /// <summary>Kilohertz (kHz) - 10^3 hertz.</summary>
    Kilohertz = 21,
    /// <summary>Megahertz (MHz) - 10^6 hertz.</summary>
    Megahertz = 22,
    /// <summary>Gigahertz (GHz) - 10^9 hertz.</summary>
    Gigahertz = 23,

    // Time
    /// <summary>Seconds (s).</summary>
    Seconds = 30,
    /// <summary>Milliseconds (ms) - 10^-3 seconds.</summary>
    Milliseconds = 31,
    /// <summary>Microseconds (µs) - 10^-6 seconds.</summary>
    Microseconds = 32,
    /// <summary>Nanoseconds (ns) - 10^-9 seconds.</summary>
    Nanoseconds = 33,

    // Temperature
    /// <summary>Degrees Celsius (°C).</summary>
    Celsius = 40,
    /// <summary>Degrees Fahrenheit (°F).</summary>
    Fahrenheit = 41,
    /// <summary>Kelvin (K).</summary>
    Kelvin = 42,

    // Capacitance
    /// <summary>Farads (F).</summary>
    Farads = 50,
    /// <summary>Microfarads (µF) - 10^-6 farads.</summary>
    Microfarads = 51,
    /// <summary>Nanofarads (nF) - 10^-9 farads.</summary>
    Nanofarads = 52,
    /// <summary>Picofarads (pF) - 10^-12 farads.</summary>
    Picofarads = 53,

    // Inductance
    /// <summary>Henries (H).</summary>
    Henries = 60,
    /// <summary>Millihenries (mH) - 10^-3 henries.</summary>
    Millihenries = 61,
    /// <summary>Microhenries (µH) - 10^-6 henries.</summary>
    Microhenries = 62,

    // Decibels
    /// <summary>Decibels (dB) - logarithmic ratio.</summary>
    Decibels = 70,
    /// <summary>Decibel-milliwatts (dBm).</summary>
    DecibelMilliwatts = 71,

    // Percentage
    /// <summary>Percentage (%).</summary>
    Percent = 80
}
