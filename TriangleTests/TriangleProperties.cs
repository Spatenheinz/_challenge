using Xunit;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using CodeChallenge.Triangle;

namespace tests;

public class TriangleProperties
{
  [Property(Arbitrary = new[] { typeof(EquilateralGen) })]
  public void TestEquilateral(Triangle t) {
      Assert.Equal(TriangleType.Equilateral, t.Classify());
  }

  [Property(Arbitrary = new[] { typeof(IsoscelesGen) })]
  public void TestIsosceles(Triangle t) {
      Assert.Equal(TriangleType.Isosceles, t.Classify());
  }

  [Property(Arbitrary = new[] { typeof(ScaleneGen) })]
  public void TestScalene(Triangle t) {
      Assert.Equal(TriangleType.Scalene, t.Classify());
  }

  [Property(Arbitrary = new[] { typeof(TriangleInequalityViolationGen) })]
  public void TestTriangleInequalityViolation((uint s1, uint s2, uint s3) t) {
      Assert.Throws<ArgumentException>(() => new Triangle(t.s1, t.s2, t.s3));
  }

  [Property(Arbitrary = new[] { typeof(ZeroSideViolationGen) })]
  public void TestZeroSidesViolation((uint s1, uint s2, uint s3) t) {
      Assert.Throws<ArgumentException>(() => new Triangle(t.s1, t.s2, t.s3));
  }
}

public static class EquilateralGen
{
  public static Arbitrary<Triangle> Generate() {

      return Arb.From(Gen.Choose(1, int.MaxValue)
                .Select(side => new Triangle((uint)side, (uint)side, (uint)side)));
  }
}

public static class IsoscelesGen
{
  public static Arbitrary<Triangle> Arbitrary() {

      return Arb.From(Gen.Choose(1, int.MaxValue).Two()
                .Where(t => TriangleValidation.IsValid((uint)t.Item1, (uint)t.Item1, (uint)t.Item2)
                            && t.Item1 != t.Item2
                )
                .Select(t => new Triangle((uint)t.Item1, (uint)t.Item1, (uint)t.Item2)));
  }
}

public static class ScaleneGen
{
  public static Arbitrary<Triangle> Arbitrary() {

      return Arb.From(Gen.Choose(1, int.MaxValue).Three()
                .Where(t => TriangleValidation.IsValid((uint)t.Item1, (uint)t.Item2, (uint)t.Item3)
                            && t.Item1 != t.Item2
                            && t.Item1 != t.Item3
                            && t.Item2 != t.Item3
                )
                .Select(t => new Triangle((uint)t.Item1, (uint)t.Item2, (uint)t.Item3)));
  }
}

public static class TriangleInequalityViolationGen
{
  public static Arbitrary<(uint, uint, uint)> Arbitrary() {

      return Arb.From(Gen.Choose(1, int.MaxValue).Three()
                .Where(t => !TriangleValidation.IsValid((uint)t.Item1, (uint)t.Item2, (uint)t.Item3))
                .Select(t => ((uint)t.Item1, (uint)t.Item2, (uint)t.Item3)));
  }
}

public static class ZeroSideViolationGen
{
  public static Arbitrary<(uint, uint, uint)> Arbitrary() {

      return Arb.From(Gen.Choose(0, 2).Three()
                .Where(t => t.Item1 == 0 || t.Item2 == 0 || t.Item3 == 0)
                .Select(t => ((uint)t.Item1, (uint)t.Item2, (uint)t.Item3)));
  }
}
