using Serilog;
using Serilog.Events;
using System.Text;
using TriangleCalculator;
using TriangleCalculator.Models;

// Настройка кодировки консоли для поддержки кириллицы
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(
        restrictedToMinimumLevel: LogEventLevel.Information,
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .WriteTo.File(
        "logs/triangle_log.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

var processor = new TriangleProcessor();

// Тестовые случаи
var testCases = new List<(string a, string b, string c, string description)>
{
    // Успешные случаи
    ("5", "5", "5", "Равносторонний треугольник"),
    ("5", "5", "7", "Равнобедренный треугольник"),
    ("3", "4", "5", "Разносторонний треугольник (прямоугольный)"),
    ("6", "8", "10", "Разносторонний треугольник"),
    
    // Не треугольник
    ("1", "1", "3", "Не треугольник - нарушение неравенства треугольника"),
    ("1", "2", "3", "Не треугольник - вырожденный"),
    
    // Невалидные данные
    ("abc", "5", "5", "Нечисловые данные - строка"),
    ("-3", "4", "5", "Отрицательная сторона"),
    ("0", "5", "5", "Нулевая сторона"),
    ("5.5", "5,5", "5.5", "Разные форматы разделителей"),
    ("", "5", "5", "Пустая строка"),
    ("   ", "5", "5", "Пробелы"),
    
    // Граничные случаи
    ("0.1", "0.1", "0.1", "Очень маленький равносторонний"),
    ("1000", "1000", "1000", "Очень большой равносторонний"),
};

Console.WriteLine("=== Вычисление вида треугольника и координат вершин ===\n");
Console.WriteLine("Поле отрисовки: 100x100 px\n");

foreach (var testCase in testCases)
{
    Console.WriteLine($"\n--- Тест: {testCase.description} ---");
    Console.WriteLine($"Стороны: a={testCase.a}, b={testCase.b}, c={testCase.c}");
    
    var result = processor.ProcessTriangle(testCase.a, testCase.b, testCase.c);
    
    Console.WriteLine($"\nРезультат:");
    Console.WriteLine($"Тип треугольника: {result.TriangleType}");
    Console.WriteLine($"Вершины:");
    Console.WriteLine($"  A: {result.Vertices[0]}");
    Console.WriteLine($"  B: {result.Vertices[1]}");
    Console.WriteLine($"  C: {result.Vertices[2]}");
    
    // Логирование
    string verticesStr = $"A{result.Vertices[0]}, B{result.Vertices[1]}, C{result.Vertices[2]}";
    
    if (result.TriangleType == "равносторонний" || 
        result.TriangleType == "равнобедренный" || 
        result.TriangleType == "разносторонний")
    {
        Log.Information("УСПЕШНО | Стороны: {A}, {B}, {C} | Тип: {Type} | Вершины: {Vertices}",
            testCase.a, testCase.b, testCase.c, result.TriangleType, verticesStr);
    }
    else
    {
        string errorMsg = result.TriangleType == "" ? "Нечисловые входные данные" : "Не треугольник";
        Log.Error("НЕУСПЕШНО | Стороны: {A}, {B}, {C} | Ошибка: {Error} | Координаты: {Vertices}",
            testCase.a, testCase.b, testCase.c, errorMsg, verticesStr);
    }
    
    // Визуализация треугольника (для успешных случаев)
    if (result.TriangleType != "" && result.TriangleType != "не треугольник")
    {
        Console.WriteLine("\nВизуализация (приблизительная):");
        DrawTriangle(result.Vertices);
    }
    
    Console.WriteLine(new string('-', 70));
}

Console.WriteLine("\nЛогирование завершено. Проверьте файл logs/triangle_log.txt");
Console.WriteLine("Нажмите любую клавишу для выхода...");
Console.ReadKey();

Log.CloseAndFlush();

// Функция для отрисовки треугольника в консоли
void DrawTriangle(List<Point> vertices)
{
    char[,] canvas = new char[25, 50];
    for (int i = 0; i < 25; i++)
        for (int j = 0; j < 50; j++)
            canvas[i, j] = '.';
    
    // Масштабирование для консоли
    int[] xCoords = { vertices[0].X / 2, vertices[1].X / 2, vertices[2].X / 2 };
    int[] yCoords = { vertices[0].Y / 4, vertices[1].Y / 4, vertices[2].Y / 4 };
    
    // Отрисовка линий (простая аппроксимация)
    DrawLine(canvas, xCoords[0], yCoords[0], xCoords[1], yCoords[1]);
    DrawLine(canvas, xCoords[1], yCoords[1], xCoords[2], yCoords[2]);
    DrawLine(canvas, xCoords[2], yCoords[2], xCoords[0], yCoords[0]);
    
    // Отметка вершин
    canvas[yCoords[0], xCoords[0]] = 'A';
    canvas[yCoords[1], xCoords[1]] = 'B';
    canvas[yCoords[2], xCoords[2]] = 'C';
    
    // Вывод
    for (int i = 0; i < 25; i++)
    {
        for (int j = 0; j < 50; j++)
        {
            Console.Write(canvas[i, j]);
        }
        Console.WriteLine();
    }
}

void DrawLine(char[,] canvas, int x0, int y0, int x1, int y1)
{
    int dx = Math.Abs(x1 - x0);
    int dy = Math.Abs(y1 - y0);
    int sx = x0 < x1 ? 1 : -1;
    int sy = y0 < y1 ? 1 : -1;
    int err = dx - dy;
    
    int x = x0, y = y0;
    
    while (true)
    {
        if (x >= 0 && x < 50 && y >= 0 && y < 25)
            canvas[y, x] = '*';
        
        if (x == x1 && y == y1) break;
        
        int e2 = 2 * err;
        if (e2 > -dy)
        {
            err -= dy;
            x += sx;
        }
        if (e2 < dx)
        {
            err += dx;
            y += sy;
        }
    }
}
