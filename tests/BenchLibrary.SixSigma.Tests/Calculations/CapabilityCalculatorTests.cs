using BenchLibrary.SixSigma.Calculations;
using BenchLibrary.SixSigma.Models;
using FluentAssertions;
using Xunit;

namespace BenchLibrary.SixSigma.Tests.Calculations;

public class CapabilityCalculatorTests
{
    [Fact]
    public void Calculate_CenteredProcess_CpEqualsCpk()
    {
        // Arrange - A perfectly centered process with mean at midpoint of spec limits
        // LSL = 4.0, USL = 6.0, Target = 5.0
        // Generate values centered at 5.0
        var random = new Random(42);
        var values = Enumerable.Range(0, 100)
            .Select(_ => 5.0 + (random.NextDouble() - 0.5) * 0.4)
            .ToList();

        // Act
        var result = CapabilityCalculator.Calculate(values, 4.0, 6.0);

        // Assert
        result.Cp.Should().BeApproximately(result.Cpk, 0.2);
        result.IsCentered.Should().BeTrue();
    }

    [Fact]
    public void Calculate_ShiftedProcess_CpkLessThanCp()
    {
        // Arrange - A process shifted toward USL
        var values = Enumerable.Range(0, 100)
            .Select(i => 5.5 + (i % 10 - 5) * 0.05) // Mean around 5.5, shifted toward USL
            .ToList();

        // Act
        var result = CapabilityCalculator.Calculate(values, 4.0, 6.0);

        // Assert
        result.Cpk.Should().BeLessThan(result.Cp);
    }

    [Fact]
    public void Calculate_CapableProcess_ReturnsCapableRating()
    {
        // Arrange - Low variation process
        var values = Enumerable.Range(0, 100)
            .Select(i => 5.0 + (i % 5 - 2) * 0.02)
            .ToList();

        // Act
        var result = CapabilityCalculator.Calculate(values, 4.0, 6.0);

        // Assert
        result.IsCapable.Should().BeTrue();
        result.Rating.Should().BeOneOf(
            CapabilityRating.Good,
            CapabilityRating.Excellent,
            CapabilityRating.WorldClass);
    }

    [Fact]
    public void Calculate_IncapableProcess_ReturnsIncapableRating()
    {
        // Arrange - High variation process
        var values = Enumerable.Range(0, 100)
            .Select(i => 5.0 + (i % 20 - 10) * 0.2) // High variation
            .ToList();

        // Act
        var result = CapabilityCalculator.Calculate(values, 4.0, 6.0);

        // Assert
        result.IsCapable.Should().BeFalse();
    }

    [Fact]
    public void Calculate_InvalidSpecLimits_ThrowsArgumentException()
    {
        // Arrange
        var values = new[] { 1.0, 2.0, 3.0 };

        // Act
        var act = () => CapabilityCalculator.Calculate(values, 6.0, 4.0); // USL < LSL

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*greater than*");
    }

    [Fact]
    public void Calculate_TooFewValues_ThrowsArgumentException()
    {
        // Arrange
        var values = new[] { 5.0 }; // Only 1 value

        // Act
        var act = () => CapabilityCalculator.Calculate(values, 4.0, 6.0);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*at least 2*");
    }

    [Fact]
    public void Calculate_ReturnsCorrectSampleSize()
    {
        // Arrange
        var values = Enumerable.Range(0, 50).Select(i => 5.0 + i * 0.01).ToList();

        // Act
        var result = CapabilityCalculator.Calculate(values, 4.0, 6.0);

        // Assert
        result.SampleSize.Should().Be(50);
    }

    [Fact]
    public void Calculate_SetsCorrectSpecLimits()
    {
        // Arrange
        var values = new[] { 4.5, 5.0, 5.5 };

        // Act
        var result = CapabilityCalculator.Calculate(values, 4.0, 6.0);

        // Assert
        result.LowerSpecLimit.Should().Be(4.0);
        result.UpperSpecLimit.Should().Be(6.0);
    }

    [Fact]
    public void Calculate_SetsDefaultTarget()
    {
        // Arrange
        var values = new[] { 4.5, 5.0, 5.5 };

        // Act
        var result = CapabilityCalculator.Calculate(values, 4.0, 6.0);

        // Assert
        result.Target.Should().Be(5.0); // Midpoint of 4.0 and 6.0
    }

    [Fact]
    public void Calculate_UsesProvidedTarget()
    {
        // Arrange
        var values = new[] { 4.5, 5.0, 5.5 };

        // Act
        var result = CapabilityCalculator.Calculate(values, 4.0, 6.0, target: 5.2);

        // Assert
        result.Target.Should().Be(5.2);
    }

    [Fact]
    public void CalculateCpk_FromKnownValues_ReturnsCorrectCpk()
    {
        // Arrange
        double mean = 5.0;
        double stdDev = 0.5;
        double lsl = 4.0;
        double usl = 6.0;

        // Expected: min((6-5)/(3*0.5), (5-4)/(3*0.5)) = min(0.667, 0.667) = 0.667

        // Act
        var result = CapabilityCalculator.CalculateCpk(mean, stdDev, lsl, usl);

        // Assert
        result.Should().BeApproximately(0.667, 0.01);
    }

    [Fact]
    public void CalculateCp_FromKnownValues_ReturnsCorrectCp()
    {
        // Arrange
        double stdDev = 0.5;
        double lsl = 4.0;
        double usl = 6.0;

        // Expected: (6-4)/(6*0.5) = 2/3 = 0.667

        // Act
        var result = CapabilityCalculator.CalculateCp(stdDev, lsl, usl);

        // Assert
        result.Should().BeApproximately(0.667, 0.01);
    }
}
