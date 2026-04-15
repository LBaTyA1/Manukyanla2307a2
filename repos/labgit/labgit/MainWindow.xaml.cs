using TriangleCalculator.Models;

namespace TriangleCalculator.Geometry;

public static class TriangleGeometry
{
    /// <summary>
    /// Вычисляет координаты вершин треугольника на поле 100x100 px
    /// </summary>
    public static List<Point> CalculateVertices(double sideA, double sideB, double sideC)
    {
        // Проверка существования треугольника
        if (!IsTriangleValid(sideA, sideB, sideC))
        {
            return new List<Point>
            {
                new Point(-1, -1),
                new Point(-1, -1),
                new Point(-1, -1)
            };
        }
        
        // Размещаем треугольник в поле 100x100 с отступами
        const int canvasSize = 100;
        const int margin = 10;
        int workArea = canvasSize - 2 * margin;
        
        // Вершина A в точке (margin, canvasSize - margin)
        Point A = new Point(margin, canvasSize - margin);
        
        // Вершина B на расстоянии sideC от A по горизонтали (масштабирование)
        double maxSide = Math.Max(sideA, Math.Max(sideB, sideC));
        double scale = workArea / maxSide;
        
        int scaledSideC = (int)(sideC * scale);
        Point B = new Point(margin + scaledSideC, canvasSize - margin);
        
        // Вершина C - пересечение окружностей
        double scaledSideA = sideA * scale;
        double scaledSideB = sideB * scale;
        
        // Расстояние между A и B
        double ab = Math.Sqrt(Math.Pow(B.X - A.X, 2) + Math.Pow(B.Y - A.Y, 2));
        
        if (Math.Abs(ab) < 0.001)
        {
            return new List<Point> { A, B, new Point(margin + scaledSideC / 2, margin) };
        }
        
        // Находим координаты точки C
        double dx = B.X - A.X;
        double dy = B.Y - A.Y;
        double d = Math.Sqrt(dx * dx + dy * dy);
        
        // Используем формулу для нахождения точки пересечения двух окружностей
        double a = (scaledSideA * scaledSideA - scaledSideB * scaledSideB + d * d) / (2 * d);
        double h = Math.Sqrt(Math.Abs(scaledSideA * scaledSideA - a * a));
        
        double xm = A.X + (dx * a / d);
        double ym = A.Y + (dy * a / d);
        
        double cx = xm + h * (-dy / d);
        double cy = ym + h * (dx / d);
        
        // Проверяем, чтобы координаты были в пределах canvas
        cx = Math.Clamp(cx, margin, canvasSize - margin);
        cy = Math.Clamp(cy, margin, canvasSize - margin);
        
        Point C = new Point((int)cx, (int)cy);
        
        return new List<Point> { A, B, C };
    }
    
    /// <summary>
    /// Определяет тип треугольника
    /// </summary>
    public static string GetTriangleType(double a, double b, double c)
    {
        if (!IsTriangleValid(a, b, c))
            return "не треугольник";
        
        const double epsilon = 0.0001;
        
        if (Math.Abs(a - b) < epsilon && Math.Abs(b - c) < epsilon)
            return "равносторонний";
        
        if (Math.Abs(a - b) < epsilon || Math.Abs(b - c) < epsilon || Math.Abs(a - c) < epsilon)
            return "равнобедренный";
        
        return "разносторонний";
    }
    
    /// <summary>
    /// Проверяет, существует ли треугольник с заданными сторонами
    /// </summary>
    private static bool IsTriangleValid(double a, double b, double c)
    {
        return a + b > c && a + c > b && b + c > a;
    }
}
