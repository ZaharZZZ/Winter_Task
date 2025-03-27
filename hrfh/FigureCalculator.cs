using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrfh
{
    public class FigureCalculator
    {
        public double CalculateArea(Figure figure, Dictionary<string, double> parameters)
        {
            switch (figure.Name.ToLower())
            {
                case "круг":
                    double radius = parameters["radius"];
                    return Math.PI * radius * radius;
                case "квадрат":
                    double side = parameters["side"];
                    return side * side;
                case "прямоугольник":
                    double width = parameters["width"];
                    double height = parameters["height"];
                    return width * height;
                case "треугольник":
                    double baseLength = parameters["base"];
                    double heightTriangle = parameters["height"];
                    return 0.5 * baseLength * heightTriangle;
                default:
                    Console.WriteLine($"Неизвестная фигура: {figure.Name}. Невозможно вычислить площадь.");
                    return double.NaN; // NaN - Not a Number
            }
        }

        public double CalculatePerimeter(Figure figure, Dictionary<string, double> parameters)
        {
            switch (figure.Name.ToLower())
            {
                case "круг":
                    double radius = parameters["radius"];
                    return 2 * Math.PI * radius;
                case "квадрат":
                    double side = parameters["side"];
                    return 4 * side;
                case "прямоугольник":
                    double width = parameters["width"];
                    double height = parameters["height"];
                    return 2 * (width + height);
                case "треугольник":
                    double a = parameters["a"];
                    double b = parameters["b"];
                    double c = parameters["c"];
                    return a + b + c;
                default:
                    Console.WriteLine($"Неизвестная фигура: {figure.Name}. Невозможно вычислить периметр.");
                    return double.NaN;
            }
        }
    }
}
