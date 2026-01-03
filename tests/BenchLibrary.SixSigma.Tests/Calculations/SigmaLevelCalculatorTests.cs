using BenchLibrary.SixSigma.Calculations;
using BenchLibrary.SixSigma.Models;
using FluentAssertions;
using Xunit;

namespace BenchLibrary.SixSigma.Tests.Calculations;

public class SigmaLevelCalculatorTests
{
    [Fact]
    public void Calculate_ValidInputs_ReturnsCorrectDpmo()
    {
        // Arrange
        long totalUnits = 1000;
        long totalDefects = 10;
        int opportunitiesPerUnit = 10;
        // DPMO = 10 / (1000 * 10) * 1,000,000 = 1000

        // Act
        var result = SigmaLevelCalculator.Calculate(totalUnits, totalDefects, opportunitiesPerUnit);

        // Assert
        result.Dpmo.Should().Be(1000);
    }

    [Fact]
    public void Calculate_ValidInputs_ReturnsCorrectYield()
    {
        // Arrange
        long totalUnits = 1000;
        long totalDefects = 100;
        int opportunitiesPerUnit = 10;
        // Yield = (1 - 100/10000) * 100 = 99%

        // Act
        var result = SigmaLevelCalculator.Calculate(totalUnits, totalDefects, opportunitiesPerUnit);

        // Assert
        result.Yield.Should().Be(99);
    }

    [Fact]
    public void Calculate_ZeroDefects_ReturnsSixSigma()
    {
        // Arrange
        long totalUnits = 1000;
        long totalDefects = 0;
        int opportunitiesPerUnit = 10;

        // Act
        var result = SigmaLevelCalculator.Calculate(totalUnits, totalDefects, opportunitiesPerUnit);

        // Assert
        result.SigmaLevel.Should().BeApproximately(6.0, 0.1);
        result.MeetsSixSigma.Should().BeTrue();
    }

    [Fact]
    public void Calculate_InvalidTotalUnits_ThrowsArgumentException()
    {
        // Act
        var act = () => SigmaLevelCalculator.Calculate(0, 10, 5);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*positive*");
    }

    [Fact]
    public void Calculate_NegativeDefects_ThrowsArgumentException()
    {
        // Act
        var act = () => SigmaLevelCalculator.Calculate(100, -10, 5);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*negative*");
    }

    [Fact]
    public void Calculate_InvalidOpportunities_ThrowsArgumentException()
    {
        // Act
        var act = () => SigmaLevelCalculator.Calculate(100, 10, 0);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*positive*");
    }

    [Fact]
    public void DpmoToSigma_StandardSixSigma_ReturnsApproximatelySix()
    {
        // Arrange - Standard 6 sigma is 3.4 DPMO
        double dpmo = 3.4;

        // Act
        var result = SigmaLevelCalculator.DpmoToSigma(dpmo);

        // Assert
        result.Should().BeApproximately(6.0, 0.2);
    }

    [Fact]
    public void DpmoToSigma_ThreeSigma_ReturnsApproximatelyThree()
    {
        // Arrange - 3 sigma is approximately 66,807 DPMO
        double dpmo = 66807;

        // Act
        var result = SigmaLevelCalculator.DpmoToSigma(dpmo);

        // Assert
        result.Should().BeApproximately(3.0, 0.2);
    }

    [Fact]
    public void SigmaToDpmo_SixSigma_Returns3Point4()
    {
        // Act
        var result = SigmaLevelCalculator.SigmaToDpmo(6.0);

        // Assert
        result.Should().Be(3.4);
    }

    [Fact]
    public void SigmaToYield_SixSigma_ReturnsHighYield()
    {
        // Act
        var result = SigmaLevelCalculator.SigmaToYield(6.0);

        // Assert
        result.Should().BeGreaterThan(99.99);
    }

    [Fact]
    public void GetBenchmarks_ReturnsSixEntries()
    {
        // Act
        var benchmarks = SigmaLevelCalculator.GetBenchmarks();

        // Assert
        benchmarks.Should().HaveCount(6);
        benchmarks.Should().ContainKey(1.0);
        benchmarks.Should().ContainKey(6.0);
    }

    [Fact]
    public void Calculate_ReturnsCorrectTotalOpportunities()
    {
        // Arrange
        long totalUnits = 1000;
        int opportunitiesPerUnit = 10;

        // Act
        var result = SigmaLevelCalculator.Calculate(totalUnits, 5, opportunitiesPerUnit);

        // Assert
        result.TotalOpportunities.Should().Be(10000);
    }

    [Fact]
    public void Calculate_SigmaRating_ReturnsCorrectRating()
    {
        // Arrange - Create a process with about 4 sigma
        // 4 sigma ≈ 6,210 DPMO
        // DPMO = defects / opportunities * 1,000,000
        // For 6,210 DPMO with 1,000,000 opportunities, we need about 6,210 defects
        long totalUnits = 100000;
        long totalDefects = 621;
        int opportunitiesPerUnit = 10;

        // Act
        var result = SigmaLevelCalculator.Calculate(totalUnits, totalDefects, opportunitiesPerUnit);

        // Assert
        result.Rating.Should().Be(SigmaRating.FourSigma);
    }

    [Fact]
    public void Result_IndustryBenchmark_ReturnsDescription()
    {
        // Arrange
        var result = SigmaLevelCalculator.Calculate(1000000, 3, 1);

        // Act & Assert
        result.IndustryBenchmark.Should().Contain("World class");
    }
}
