using BenchLibrary.SixSigma.Calculations;
using FluentAssertions;
using Xunit;

namespace BenchLibrary.SixSigma.Tests.Calculations;

public class ParetoCalculatorTests
{
    [Fact]
    public void Analyze_ValidData_SortsItemsByCountDescending()
    {
        // Arrange
        var data = new Dictionary<string, int>
        {
            { "A", 10 },
            { "B", 50 },
            { "C", 30 },
            { "D", 10 }
        };

        // Act
        var result = ParetoCalculator.Analyze(data);

        // Assert
        result.Items[0].Category.Should().Be("B");
        result.Items[1].Category.Should().Be("C");
        result.Items[0].Count.Should().Be(50);
        result.Items[1].Count.Should().Be(30);
    }

    [Fact]
    public void Analyze_ValidData_CalculatesCorrectPercentages()
    {
        // Arrange
        var data = new Dictionary<string, int>
        {
            { "A", 50 },
            { "B", 30 },
            { "C", 20 }
        };

        // Act
        var result = ParetoCalculator.Analyze(data);

        // Assert
        result.Items[0].Percentage.Should().Be(50);
        result.Items[1].Percentage.Should().Be(30);
        result.Items[2].Percentage.Should().Be(20);
    }

    [Fact]
    public void Analyze_ValidData_CalculatesCorrectCumulativePercentages()
    {
        // Arrange
        var data = new Dictionary<string, int>
        {
            { "A", 50 },
            { "B", 30 },
            { "C", 20 }
        };

        // Act
        var result = ParetoCalculator.Analyze(data);

        // Assert
        result.Items[0].CumulativePercentage.Should().Be(50);
        result.Items[1].CumulativePercentage.Should().Be(80);
        result.Items[2].CumulativePercentage.Should().Be(100);
    }

    [Fact]
    public void Analyze_ValidData_AssignsCorrectRanks()
    {
        // Arrange
        var data = new Dictionary<string, int>
        {
            { "A", 50 },
            { "B", 30 },
            { "C", 20 }
        };

        // Act
        var result = ParetoCalculator.Analyze(data);

        // Assert
        result.Items[0].Rank.Should().Be(1);
        result.Items[1].Rank.Should().Be(2);
        result.Items[2].Rank.Should().Be(3);
    }

    [Fact]
    public void Analyze_ValidData_IdentifiesVitalFew()
    {
        // Arrange
        var data = new Dictionary<string, int>
        {
            { "A", 50 },
            { "B", 30 },
            { "C", 15 },
            { "D", 5 }
        };

        // Act
        var result = ParetoCalculator.Analyze(data);

        // Assert
        result.VitalFew.Should().HaveCount(2); // A (50%) and B (80%)
        result.TrivialMany.Should().HaveCount(2); // C and D
    }

    [Fact]
    public void Analyze_EmptyData_ReturnsEmptyResult()
    {
        // Arrange
        var data = new Dictionary<string, int>();

        // Act
        var result = ParetoCalculator.Analyze(data);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public void Analyze_WithTitle_SetsTitle()
    {
        // Arrange
        var data = new Dictionary<string, int> { { "A", 10 } };

        // Act
        var result = ParetoCalculator.Analyze(data, "Defect Analysis");

        // Assert
        result.Title.Should().Be("Defect Analysis");
    }

    [Fact]
    public void Analyze_ValidData_CalculatesCorrectTotalCount()
    {
        // Arrange
        var data = new Dictionary<string, int>
        {
            { "A", 50 },
            { "B", 30 },
            { "C", 20 }
        };

        // Act
        var result = ParetoCalculator.Analyze(data);

        // Assert
        result.TotalCount.Should().Be(100);
    }

    [Fact]
    public void AnalyzeWithItems_GroupsByCategory()
    {
        // Arrange
        var defects = new[]
        {
            new { Type = "Scratch", Product = "A" },
            new { Type = "Scratch", Product = "B" },
            new { Type = "Dent", Product = "A" },
            new { Type = "Scratch", Product = "C" }
        };

        // Act
        var result = ParetoCalculator.Analyze(defects, d => d.Type);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.First(i => i.Category == "Scratch").Count.Should().Be(3);
        result.Items.First(i => i.Category == "Dent").Count.Should().Be(1);
    }

    [Fact]
    public void GetVitalFew_ReturnsCorrectCategories()
    {
        // Arrange
        var data = new Dictionary<string, int>
        {
            { "A", 50 },
            { "B", 30 },
            { "C", 15 },
            { "D", 5 }
        };

        // Act
        var vitalFew = ParetoCalculator.GetVitalFew(data);

        // Assert
        vitalFew.Should().BeEquivalentTo(new[] { "A", "B" });
    }

    [Fact]
    public void GetTrivialMany_ReturnsCorrectCategories()
    {
        // Arrange
        var data = new Dictionary<string, int>
        {
            { "A", 50 },
            { "B", 30 },
            { "C", 15 },
            { "D", 5 }
        };

        // Act
        var trivialMany = ParetoCalculator.GetTrivialMany(data);

        // Assert
        trivialMany.Should().BeEquivalentTo(new[] { "C", "D" });
    }
}
