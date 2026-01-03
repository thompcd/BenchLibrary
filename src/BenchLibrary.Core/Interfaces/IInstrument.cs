using BenchLibrary.Core.Enums;

namespace BenchLibrary.Core.Interfaces;

/// <summary>
/// Base interface for all test instruments (multimeters, oscilloscopes, etc.).
/// </summary>
public interface IInstrument
{
    /// <summary>
    /// Gets the unique identifier for this instrument instance.
    /// Typically a combination of manufacturer, model, and serial number.
    /// </summary>
    string InstrumentId { get; }

    /// <summary>
    /// Gets the type of instrument.
    /// </summary>
    InstrumentType Type { get; }

    /// <summary>
    /// Gets a value indicating whether the instrument is currently connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Connects to the instrument asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if connection was successful, false otherwise.</returns>
    Task<bool> ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects from the instrument asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task DisconnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the instrument to its default state.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task ResetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the instrument identification string (typically *IDN? response for SCPI instruments).
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The instrument identification string.</returns>
    Task<string> GetIdentificationAsync(CancellationToken cancellationToken = default);
}
