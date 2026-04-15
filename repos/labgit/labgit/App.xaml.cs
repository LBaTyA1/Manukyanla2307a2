namespace TriangleCalculator.Models;

public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }
    
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
    
    public override string ToString() => $"({X}, {Y})";
    
    public bool Equals(Point other) => X == other.X && Y == other.Y;
    
    public override bool Equals(object? obj) => obj is Point other && Equals(other);
    
    public override int GetHashCode() => HashCode.Combine(X, Y);
}
