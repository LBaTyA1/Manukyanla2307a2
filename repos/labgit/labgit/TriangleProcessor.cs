using System.Globalization;
using TriangleCalculator.Models;
using TriangleCalculator.Geometry;

namespace TriangleCalculator;

public class TriangleProcessor
{
    /// <summary>
    /// Обрабатывает запрос на вычисление треугольника
    /// </summary>
    public TriangleResult ProcessTriangle(string sideAStr, string sideBStr, string sideCStr)
    {
        // Парсинг входных данных
        if (!TryParseSide(sideAStr, out double sideA) ||
            !TryParseSide(sideBStr, out double sideB) ||
            !TryParseSide(sideCStr, out double sideC))
        {
            // Невалидные данные (нечисловые)
            return new TriangleResult(
                "",
                new List<Point>
                {
                    new Point(-2, -2),
                    new Point(-2, -2),
                    new Point(-2, -2)
                }
            );
        }
        
        // Проверка положительности чисел
        if (sideA <= 0 || sideB <= 0 || sideC <= 0)
        {
            return new TriangleResult(
                "не треугольник",
                new List<Point>
                {
                    new Point(-1, -1),
                    new Point(-1, -1),
                    new Point(-1, -1)
                }
            );
        }
        
        // Определяем тип треугольника
        string triangleType = TriangleGeometry.GetTriangleType(sideA, sideB, sideC);
        
        // Вычисляем координаты вершин
        List<Point> vertices = TriangleGeometry.CalculateVertices(sideA, sideB, sideC);
        
        return new TriangleResult(triangleType, vertices);
    }
    
    /// <summary>
    /// Парсит строку в вещественное число типа float
    /// </summary>
    private bool TryParseSide(string input, out double value)
    {
        value = 0;
        
        if (string.IsNullOrWhiteSpace(input))
            return false;
        
        // Пробуем парсить с учетом разных культур
        if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
        {
            return true;
        }
        
        if (double.TryParse(input, NumberStyles.Float, CultureInfo.CurrentCulture, out value))
        {
            return true;
        }
        
        return false;
    }
}
