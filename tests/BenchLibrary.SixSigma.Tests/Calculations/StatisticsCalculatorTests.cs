using BenchLibrary.SixSigma.Calculations;
using FluentAssertions;
using Xunit;

namespace BenchLibrary.SixSigma.Tests.Calculations;

public class StatisticsCalculatorTests
{
    [Fact]
    public void Mean_ValidValues_ReturnsCorrectMean()
    {
        // Arrange
        var values = new[] { 1.0, 2.0, 3.0, 4.0, 5.0 };

        // Act
        var result = StatisticsCalculator.Mean(values);

        // Assert
        result.Should().Be(3.0);
    }

    [Fact]
    public void Mean_EmptyCollection_ThrowsArgumentException()
    {
        // Arrange
        var values = Array.Empty<double>();

        // Act
        var act = () => StatisticsCalculator.Mean(values);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*empty*");
    }

    [Fact]
    public void StandardDeviation_ValidValues_ReturnsCorrectStdDev()
    {
        // Arrange
        var values = new[] { 2.0, 4.0, 4.0, 4.0, 5.0, 5.0, 7.0, 9.0 };

        // Act
        var result = StatisticsCalculator.StandardDeviation(values);

        // Assert
        result.Should().BeApproximately(2.138, 0.001);
    }

    [Fact]
    public void StandardDeviation_SingleValue_ThrowsArgumentException()
    {
        // Arrange
        var values = new[] { 5.0 };

        // Act
        var act = () => StatisticsCalculator.StandardDeviation(values);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*at least 2*");
    }

    [Fact]
    public void Range_ValidValues_ReturnsCorrectRange()
    {
        // Arrange
        var values = new[] { 3.0, 7.0, 1.0, 9.0, 4.0 };

        // Act
        var result = StatisticsCalculator.Range(values);

        // Assert
        result.Should().Be(8.0);
    }

    [Fact]
    public void Median_OddNumberOfValues_ReturnsMiddleValue()
    {
        // Arrange
        var values = new[] { 3.0, 1.0, 2.0 };

        // Act
        var result = StatisticsCalculator.Median(values);

        // Assert
        result.Should().Be(2.0);
    }

    [Fact]
    public void Median_EvenNumberOfValues_ReturnsAverageOfMiddleTwo()
    {
        // Arrange
        var values = new[] { 1.0, 2.0, 3.0, 4.0 };

        // Act
        var result = StatisticsCalculator.Median(values);

        // Assert
        result.Should().Be(2.5);
    }

    [Fact]
    public void MovingRange_ValidValues_ReturnsCorrectRanges()
    {
        // Arrange
        var values = new[] { 10.0, 12.0, 11.0, 15.0 };

        // Act
        var result = StatisticsCalculator.MovingRange(values).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Should().Be(2.0);  // |12-10|
        result[1].Should().Be(1.0);  // |11-12|
        result[2].Should().Be(4.0);  // |15-11|
    }

    [Fact]
    public void GetD2Constant_SubgroupSize5_Returns2326()
    {
        // Act
        var result = StatisticsCalculator.GetD2Constant(5);

        // Assert
        result.Should().Be(2.326);
    }

    [Fact]
    public void GetA2Constant_SubgroupSize5_Returns0577()
    {
        // Act
        var result = StatisticsCalculator.GetA2Constant(5);

        // Assert
        result.Should().Be(0.577);
    }

    [Fact]
    public void GetD4Constant_SubgroupSize5_Returns2114()
    {
        // Act
        var result = StatisticsCalculator.GetD4Constant(5);

        // Assert
        result.Should().Be(2.114);
    }

    [Fact]
    public void GetD2Constant_InvalidSubgroupSize_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var act = () => StatisticsCalculator.GetD2Constant(15);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void EstimateSigmaFromRange_ValidInputs_ReturnsCorrectSigma()
    {
        // Arrange
        var averageRange = 4.652;
        var subgroupSize = 5;

        // Act
        var result = StatisticsCalculator.EstimateSigmaFromRange(averageRange, subgroupSize);

        // Assert (sigma = R-bar / d2 = 4.652 / 2.326 ≈ 2.0)
        result.Should().BeApproximately(2.0, 0.01);
    }
}
