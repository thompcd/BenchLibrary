using BenchLibrary.Core.Enums;
using BenchLibrary.Core.Models;
using FluentAssertions;
using Xunit;

namespace BenchLibrary.Core.Tests.Models;

public class TestResultTests
{
    [Fact]
    public void AllMeasurementsPassed_AllWithinLimits_ReturnsTrue()
    {
        // Arrange
        var testResult = new TestResult
        {
            SerialNumber = "SN001",
            TestName = "Voltage Test",
            Measurements = new List<Measurement>
            {
                new() { Name = "V1", Value = 5.0, LowerLimit = 4.5, UpperLimit = 5.5 },
                new() { Name = "V2", Value = 5.1, LowerLimit = 4.5, UpperLimit = 5.5 }
            }
        };

        // Act & Assert
        testResult.AllMeasurementsPassed.Should().BeTrue();
    }

    [Fact]
    public void AllMeasurementsPassed_SomeOutOfLimits_ReturnsFalse()
    {
        // Arrange
        var testResult = new TestResult
        {
            SerialNumber = "SN001",
            TestName = "Voltage Test",
            Measurements = new List<Measurement>
            {
                new() { Name = "V1", Value = 5.0, LowerLimit = 4.5, UpperLimit = 5.5 },
                new() { Name = "V2", Value = 6.0, LowerLimit = 4.5, UpperLimit = 5.5 }
            }
        };

        // Act & Assert
        testResult.AllMeasurementsPassed.Should().BeFalse();
    }

    [Fact]
    public void FailedMeasurementCount_ReturnsCorrectCount()
    {
        // Arrange
        var testResult = new TestResult
        {
            SerialNumber = "SN001",
            TestName = "Voltage Test",
            Measurements = new List<Measurement>
            {
                new() { Name = "V1", Value = 5.0, LowerLimit = 4.5, UpperLimit = 5.5 },
                new() { Name = "V2", Value = 6.0, LowerLimit = 4.5, UpperLimit = 5.5 },
                new() { Name = "V3", Value = 4.0, LowerLimit = 4.5, UpperLimit = 5.5 }
            }
        };

        // Act & Assert
        testResult.FailedMeasurementCount.Should().Be(2);
    }

    [Fact]
    public void TotalMeasurementCount_ReturnsCorrectCount()
    {
        // Arrange
        var testResult = new TestResult
        {
            SerialNumber = "SN001",
            TestName = "Voltage Test",
            Measurements = new List<Measurement>
            {
                new() { Name = "V1", Value = 5.0 },
                new() { Name = "V2", Value = 5.1 },
                new() { Name = "V3", Value = 5.2 }
            }
        };

        // Act & Assert
        testResult.TotalMeasurementCount.Should().Be(3);
    }

    [Fact]
    public void Duration_TestComplete_ReturnsDuration()
    {
        // Arrange
        var startTime = new DateTime(2024, 1, 1, 10, 0, 0);
        var endTime = new DateTime(2024, 1, 1, 10, 5, 30);
        var testResult = new TestResult
        {
            SerialNumber = "SN001",
            TestName = "Test",
            StartTime = startTime,
            EndTime = endTime
        };

        // Act & Assert
        testResult.Duration.Should().Be(TimeSpan.FromMinutes(5.5));
    }

    [Fact]
    public void Duration_TestNotComplete_ReturnsNull()
    {
        // Arrange
        var testResult = new TestResult
        {
            SerialNumber = "SN001",
            TestName = "Test",
            StartTime = DateTime.UtcNow,
            EndTime = null
        };

        // Act & Assert
        testResult.Duration.Should().BeNull();
    }

    [Fact]
    public void Status_DefaultsToNotStarted()
    {
        // Arrange & Act
        var testResult = new TestResult
        {
            SerialNumber = "SN001",
            TestName = "Test"
        };

        // Assert
        testResult.Status.Should().Be(TestStatus.NotStarted);
    }

    [Fact]
    public void CreatedAt_DefaultsToUtcNow()
    {
        // Arrange & Act
        var testResult = new TestResult
        {
            SerialNumber = "SN001",
            TestName = "Test"
        };

        // Assert
        testResult.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
