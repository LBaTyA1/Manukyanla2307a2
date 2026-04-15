namespace TriangleCalculator.Models;

public class TriangleResult
{
    public string TriangleType { get; set; }
    public List<Point> Vertices { get; set; }
    
    public TriangleResult(string triangleType, List<Point> vertices)
    {
        TriangleType = triangleType;
        Vertices = vertices;
    }
    
    public override string ToString()
    {
        return $"Тип: {TriangleType}, Вершины: A{Vertices[0]}, B{Vertices[1]}, C{Vertices[2]}";
    }
}
