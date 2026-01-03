using BenchLibrary.SixSigma.Calculations;
using BenchLibrary.SixSigma.Models;
using FluentAssertions;
using Xunit;

namespace BenchLibrary.SixSigma.Tests.Calculations;

public class ControlChartCalculatorTests
{
    [Fact]
    public void CalculateXBarChart_ValidSubgroups_ReturnsCorrectChartType()
    {
        // Arrange
        var subgroups = new List<double[]>
        {
            new[] { 10.0, 11.0, 10.5, 10.2, 10.3 },
            new[] { 10.1, 10.4, 10.6, 10.2, 10.5 },
            new[] { 10.3, 10.2, 10.4, 10.5, 10.1 }
        };

        // Act
        var result = ControlChartCalculator.CalculateXBarChart(subgroups);

        // Assert
        result.ChartType.Should().Be(ControlChartType.XBar);
    }

    [Fact]
    public void CalculateXBarChart_ValidSubgroups_ReturnsCorrectPointCount()
    {
        // Arrange
        var subgroups = new List<double[]>
        {
            new[] { 10.0, 11.0, 10.5 },
            new[] { 10.1, 10.4, 10.6 },
            new[] { 10.3, 10.2, 10.4 },
            new[] { 10.2, 10.3, 10.5 }
        };

        // Act
        var result = ControlChartCalculator.CalculateXBarChart(subgroups);

        // Assert
        result.Points.Should().HaveCount(4);
        result.SubgroupCount.Should().Be(4);
    }

    [Fact]
    public void CalculateXBarChart_ValidSubgroups_CalculatesCorrectCenterLine()
    {
        // Arrange
        var subgroups = new List<double[]>
        {
            new[] { 10.0, 10.0, 10.0 },
            new[] { 12.0, 12.0, 12.0 },
            new[] { 14.0, 14.0, 14.0 }
        };

        // Act
        var result = ControlChartCalculator.CalculateXBarChart(subgroups);

        // Assert
        result.CenterLine.Should().BeApproximately(12.0, 0.01);
        result.ProcessMean.Should().BeApproximately(12.0, 0.01);
    }

    [Fact]
    public void CalculateXBarChart_EmptySubgroups_ThrowsArgumentException()
    {
        // Arrange
        var subgroups = new List<double[]>();

        // Act
        var act = () => ControlChartCalculator.CalculateXBarChart(subgroups);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*at least one*");
    }

    [Fact]
    public void CalculateXBarChart_InconsistentSubgroupSizes_ThrowsArgumentException()
    {
        // Arrange
        var subgroups = new List<double[]>
        {
            new[] { 10.0, 11.0, 10.5 },
            new[] { 10.1, 10.4 } // Different size
        };

        // Act
        var act = () => ControlChartCalculator.CalculateXBarChart(subgroups);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*same size*");
    }

    [Fact]
    public void CalculateXBarChart_InControlProcess_HasNoOutOfControlPoints()
    {
        // Arrange - Stable process with low variation
        var subgroups = Enumerable.Range(0, 20)
            .Select(i => new[] { 10.0, 10.1, 10.0, 10.1, 10.0 })
            .ToList();

        // Act
        var result = ControlChartCalculator.CalculateXBarChart(subgroups);

        // Assert
        result.IsInControl.Should().BeTrue();
        result.OutOfControlCount.Should().Be(0);
    }

    [Fact]
    public void CalculateRChart_ValidSubgroups_ReturnsRangeChartType()
    {
        // Arrange
        var subgroups = new List<double[]>
        {
            new[] { 10.0, 11.0, 10.5 },
            new[] { 10.1, 10.4, 10.6 },
            new[] { 10.3, 10.2, 10.4 }
        };

        // Act
        var result = ControlChartCalculator.CalculateRChart(subgroups);

        // Assert
        result.ChartType.Should().Be(ControlChartType.Range);
    }

    [Fact]
    public void CalculateRChart_ValidSubgroups_CalculatesCorrectAverageRange()
    {
        // Arrange
        var subgroups = new List<double[]>
        {
            new[] { 10.0, 11.0, 10.0 }, // Range = 1
            new[] { 10.0, 12.0, 10.0 }, // Range = 2
            new[] { 10.0, 13.0, 10.0 }  // Range = 3
        };

        // Act
        var result = ControlChartCalculator.CalculateRChart(subgroups);

        // Assert
        result.AverageRange.Should().BeApproximately(2.0, 0.01);
        result.CenterLine.Should().BeApproximately(2.0, 0.01);
    }

    [Fact]
    public void CalculateIndividualsChart_ValidValues_ReturnsIndividualsChartType()
    {
        // Arrange
        var values = new[] { 10.0, 10.5, 10.2, 10.8, 10.1 };

        // Act
        var result = ControlChartCalculator.CalculateIndividualsChart(values);

        // Assert
        result.ChartType.Should().Be(ControlChartType.Individuals);
    }

    [Fact]
    public void CalculateIndividualsChart_ValidValues_HasCorrectPointCount()
    {
        // Arrange
        var values = new[] { 10.0, 10.5, 10.2, 10.8, 10.1 };

        // Act
        var result = ControlChartCalculator.CalculateIndividualsChart(values);

        // Assert
        result.Points.Should().HaveCount(5);
    }

    [Fact]
    public void CalculateMovingRangeChart_ValidValues_ReturnsMovingRangeChartType()
    {
        // Arrange
        var values = new[] { 10.0, 10.5, 10.2, 10.8, 10.1 };

        // Act
        var result = ControlChartCalculator.CalculateMovingRangeChart(values);

        // Assert
        result.ChartType.Should().Be(ControlChartType.MovingRange);
    }

    [Fact]
    public void CalculateMovingRangeChart_ValidValues_HasOneLessPointThanValues()
    {
        // Arrange
        var values = new[] { 10.0, 10.5, 10.2, 10.8, 10.1 };

        // Act
        var result = ControlChartCalculator.CalculateMovingRangeChart(values);

        // Assert
        result.Points.Should().HaveCount(4); // n-1 moving ranges
    }

    [Fact]
    public void CalculateMovingRangeChart_LowerControlLimit_IsZero()
    {
        // Arrange
        var values = new[] { 10.0, 10.5, 10.2, 10.8, 10.1 };

        // Act
        var result = ControlChartCalculator.CalculateMovingRangeChart(values);

        // Assert
        result.LowerControlLimit.Should().Be(0);
    }
}
