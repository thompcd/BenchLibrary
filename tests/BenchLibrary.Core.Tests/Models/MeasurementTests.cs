using BenchLibrary.Core.Enums;
using BenchLibrary.Core.Models;
using FluentAssertions;
using Xunit;

namespace BenchLibrary.Core.Tests.Models;

public class MeasurementTests
{
    [Fact]
    public void IsWithinLimits_ValueBetweenLimits_ReturnsTrue()
    {
        // Arrange
        var measurement = new Measurement
        {
            Name = "Voltage",
            Value = 5.0,
            Unit = MeasurementUnit.Volts,
            LowerLimit = 4.5,
            UpperLimit = 5.5
        };

        // Act & Assert
        measurement.IsWithinLimits.Should().BeTrue();
    }

    [Fact]
    public void IsWithinLimits_ValueBelowLowerLimit_ReturnsFalse()
    {
        // Arrange
        var measurement = new Measurement
        {
            Name = "Voltage",
            Value = 4.0,
            Unit = MeasurementUnit.Volts,
            LowerLimit = 4.5,
            UpperLimit = 5.5
        };

        // Act & Assert
        measurement.IsWithinLimits.Should().BeFalse();
    }

    [Fact]
    public void IsWithinLimits_ValueAboveUpperLimit_ReturnsFalse()
    {
        // Arrange
        var measurement = new Measurement
        {
            Name = "Voltage",
            Value = 6.0,
            Unit = MeasurementUnit.Volts,
            LowerLimit = 4.5,
            UpperLimit = 5.5
        };

        // Act & Assert
        measurement.IsWithinLimits.Should().BeFalse();
    }

    [Fact]
    public void IsWithinLimits_NoLowerLimit_ChecksOnlyUpperLimit()
    {
        // Arrange
        var measurement = new Measurement
        {
            Name = "Voltage",
            Value = 5.0,
            Unit = MeasurementUnit.Volts,
            LowerLimit = null,
            UpperLimit = 5.5
        };

        // Act & Assert
        measurement.IsWithinLimits.Should().BeTrue();
    }

    [Fact]
    public void IsWithinLimits_NoUpperLimit_ChecksOnlyLowerLimit()
    {
        // Arrange
        var measurement = new Measurement
        {
            Name = "Voltage",
            Value = 5.0,
            Unit = MeasurementUnit.Volts,
            LowerLimit = 4.5,
            UpperLimit = null
        };

        // Act & Assert
        measurement.IsWithinLimits.Should().BeTrue();
    }

    [Fact]
    public void IsWithinLimits_NoLimits_ReturnsTrue()
    {
        // Arrange
        var measurement = new Measurement
        {
            Name = "Voltage",
            Value = 100.0,
            Unit = MeasurementUnit.Volts,
            LowerLimit = null,
            UpperLimit = null
        };

        // Act & Assert
        measurement.IsWithinLimits.Should().BeTrue();
    }

    [Fact]
    public void Deviation_ValueWithinLimits_ReturnsZero()
    {
        // Arrange
        var measurement = new Measurement
        {
            Name = "Voltage",
            Value = 5.0,
            Unit = MeasurementUnit.Volts,
            LowerLimit = 4.5,
            UpperLimit = 5.5
        };

        // Act & Assert
        measurement.Deviation.Should().Be(0);
    }

    [Fact]
    public void Deviation_ValueBelowLowerLimit_ReturnsNegativeDeviation()
    {
        // Arrange
        var measurement = new Measurement
        {
            Name = "Voltage",
            Value = 4.0,
            Unit = MeasurementUnit.Volts,
            LowerLimit = 4.5,
            UpperLimit = 5.5
        };

        // Act & Assert
        measurement.Deviation.Should().Be(-0.5);
    }

    [Fact]
    public void Deviation_ValueAboveUpperLimit_ReturnsPositiveDeviation()
    {
        // Arrange
        var measurement = new Measurement
        {
            Name = "Voltage",
            Value = 6.0,
            Unit = MeasurementUnit.Volts,
            LowerLimit = 4.5,
            UpperLimit = 5.5
        };

        // Act & Assert
        measurement.Deviation.Should().Be(0.5);
    }

    [Fact]
    public void Timestamp_DefaultsToUtcNow()
    {
        // Arrange & Act
        var measurement = new Measurement
        {
            Name = "Test",
            Value = 1.0
        };

        // Assert
        measurement.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
