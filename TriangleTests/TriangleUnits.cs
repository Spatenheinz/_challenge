using Xunit;
using CodeChallenge.Triangle;

namespace tests;

public class TriangleUnitTests {

    [Theory]
    [InlineData(TriangleType.Equilateral, 42, 42, 42)]
    [InlineData(TriangleType.Isosceles, 13, 37, 37)]
    [InlineData(TriangleType.Scalene, 2, 3, 4)]
    public void TestGoodCase(TriangleType expected, uint s1, uint s2, uint s3) {
        var t = new Triangle(s1, s2, s3);
        Assert.Equal(expected, t.Classify());
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(1, 3, 2)]
    [InlineData(2, 1, 3)]
    [InlineData(2, 3, 1)]
    [InlineData(3, 1, 2)]
    [InlineData(3, 2, 1)]
    public void TestTriangleInequalityViolation(uint s1, uint s2, uint s3) {
        Assert.Throws<ArgumentException>(() => new Triangle(s1, s2, s3));
    }
}
