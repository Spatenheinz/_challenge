namespace CodeChallenge.Triangle;

interface IClassify<T> {
    T Classify();
}

public static class TriangleValidation {
    public static bool IsValid(uint s1, uint s2, uint s3) {
        return (ulong)s1 + s2 > s3
                && (ulong)s1 + s3 > s2
                && (ulong)s2 + s3 > s1;
    }
}

public struct Triangle : IClassify<TriangleType> {
    // We could also model with # NonZeroUInt,
    // But there are many considerations, what about
    // floating points etc. which would need a whole new validator
    public uint s1, s2, s3;

    public Triangle(uint s1, uint s2, uint s3) {
        if (!TriangleValidation.IsValid(s1, s2, s3)) {
            throw new ArgumentException($"Invalid triangle of sides {s1} {s2} {s3}");
        }
        this.s1 = s1;
        this.s2 = s2;
        this.s3 = s3;
    }

    public TriangleType Classify() {
        bool s1s2Same = s1 == s2;
        bool s1s3Same = s1 == s3;
        bool s2s3Same = s2 == s3;
        if (s1s2Same && s1s3Same && s2s3Same) {
            return TriangleType.Equilateral;
        }
        if (s1s2Same || s1s3Same || s2s3Same) {
            return TriangleType.Isosceles;
        }
        return TriangleType.Scalene;

    }
}

public enum TriangleType {
    Equilateral,
    Isosceles,
    Scalene
}
