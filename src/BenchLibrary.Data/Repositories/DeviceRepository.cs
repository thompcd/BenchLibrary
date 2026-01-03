using BenchLibrary.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BenchLibrary.Data.Repositories;

/// <summary>
/// Repository for device under test operations.
/// </summary>
public class DeviceRepository
{
    private readonly BenchLibraryDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public DeviceRepository(BenchLibraryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Adds a new device to the repository.
    /// </summary>
    /// <param name="device">The device to add.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The added device with its assigned ID.</returns>
    public async Task<DeviceUnderTest> AddAsync(DeviceUnderTest device, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);

        _context.Devices.Add(device);
        await _context.SaveChangesAsync(cancellationToken);
        return device;
    }

    /// <summary>
    /// Gets a device by its unique identifier.
    /// </summary>
    /// <param name="id">The device ID.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The device if found, null otherwise.</returns>
    public async Task<DeviceUnderTest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Devices
            .Include(d => d.TestResults)
                .ThenInclude(t => t.Measurements)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    /// <summary>
    /// Gets a device by its serial number.
    /// </summary>
    /// <param name="serialNumber">The serial number.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The device if found, null otherwise.</returns>
    public async Task<DeviceUnderTest?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(serialNumber);

        return await _context.Devices
            .Include(d => d.TestResults)
                .ThenInclude(t => t.Measurements)
            .FirstOrDefaultAsync(d => d.SerialNumber == serialNumber, cancellationToken);
    }

    /// <summary>
    /// Gets devices by model number.
    /// </summary>
    /// <param name="modelNumber">The model number.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of devices with the specified model number.</returns>
    public async Task<IEnumerable<DeviceUnderTest>> GetByModelNumberAsync(string modelNumber, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(modelNumber);

        return await _context.Devices
            .Include(d => d.TestResults)
            .Where(d => d.ModelNumber == modelNumber)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets devices by lot number.
    /// </summary>
    /// <param name="lotNumber">The lot number.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of devices from the specified lot.</returns>
    public async Task<IEnumerable<DeviceUnderTest>> GetByLotNumberAsync(string lotNumber, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(lotNumber);

        return await _context.Devices
            .Include(d => d.TestResults)
            .Where(d => d.LotNumber == lotNumber)
            .OrderBy(d => d.SerialNumber)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Updates an existing device.
    /// </summary>
    /// <param name="device">The device to update.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated device.</returns>
    public async Task<DeviceUnderTest> UpdateAsync(DeviceUnderTest device, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);

        _context.Devices.Update(device);
        await _context.SaveChangesAsync(cancellationToken);
        return device;
    }

    /// <summary>
    /// Deletes a device by its unique identifier.
    /// </summary>
    /// <param name="id">The device ID to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if the device was deleted, false if not found.</returns>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var device = await _context.Devices.FindAsync(new object[] { id }, cancellationToken);
        if (device == null)
        {
            return false;
        }

        _context.Devices.Remove(device);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Gets all devices with pagination.
    /// </summary>
    /// <param name="skip">Number of records to skip.</param>
    /// <param name="take">Number of records to take.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of devices.</returns>
    public async Task<IEnumerable<DeviceUnderTest>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Devices
            .OrderByDescending(d => d.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the total count of devices.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The total number of devices.</returns>
    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Devices.CountAsync(cancellationToken);
    }
}
