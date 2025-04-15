using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrfh
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. Чтение данных из файла
            string filePath = "figures.txt";
            FigureDataReader reader = new FigureDataReader(filePath);
            List<Figure> figures = reader.ReadFigures();

            // Пример содержимого figures.txt:
            // Круг, pi * r^2, 2 * pi * r
            // Квадрат, a^2, 4 * a
            // Прямоугольник, w * h, 2 * (w + h)
            // Треугольник, 0.5 * b * h, a + b + c

            if (figures.Count == 0)
            {
                Console.WriteLine("Нет данных о фигурах для обработки.");
                return;
            }
            bool continueCalculating = true;
            while (continueCalculating)
            {
                // 2. Выбор фигуры пользователем
                Console.WriteLine("-----------------------------------------------------------------------");
                Console.WriteLine("Доступные фигуры:");
            for (int i = 0; i < figures.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {figures[i]}");
            }

            int choice;
            while (true)
            {
                    
                    Console.Write("Выберите номер фигуры для расчета: ");

                if (int.TryParse(Console.ReadLine(), out choice) && choice > 0 && choice <= figures.Count)
                {
                    break;
                }

                Console.WriteLine("Неверный ввод. Пожалуйста, введите корректный номер.");
            }

            Figure selectedFigure = figures[choice - 1];

            // 3. Ввод параметров фигуры
            Dictionary<string, double> parameters = new Dictionary<string, double>();
            switch (selectedFigure.Name.ToLower())
            {
                case "круг":
                     parameters["radius"] = GetDoubleFromUser("Введите радиус круга: ");
                    break;
                case "квадрат":
                    parameters["side"] = GetDoubleFromUser("Введите длину стороны квадрата: ");
                    break;
                case "прямоугольник":
                    parameters["width"] = GetDoubleFromUser("Введите ширину прямоугольника: ");
                    parameters["height"] = GetDoubleFromUser("Введите высоту прямоугольника: ");
                    break;
                case "треугольник":
                    parameters["base"] = GetDoubleFromUser("Введите длину основания треугольника: ");
                    parameters["height"] = GetDoubleFromUser("Введите высоту треугольника: ");
                    Console.WriteLine("Для расчета периметра введите длины трех сторон треугольника:");
                    parameters["a"] = GetDoubleFromUser("Введите длину стороны a: ");
                    parameters["b"] = GetDoubleFromUser("Введите длину стороны b: ");
                    parameters["c"] = GetDoubleFromUser("Введите длину стороны c: ");
                    break;
                default:
                    Console.WriteLine("Неизвестная фигура. Расчет невозможен.");
                    return;
            }

            // 4. Вычисление площади и периметра
            FigureCalculator calculator = new FigureCalculator();
            double area = calculator.CalculateArea(selectedFigure, parameters);
            double perimeter = calculator.CalculatePerimeter(selectedFigure, parameters);

            // 5. Вывод результатов
            Console.WriteLine($"Площадь {selectedFigure.Name}: {area}");
            Console.WriteLine($"Периметр {selectedFigure.Name}: {perimeter}");

            // 6. Сохранение результатов в файл
            string resultsFilePath = "results.txt";
            ResultSaver saver = new ResultSaver(resultsFilePath);
            saver.SaveResult(selectedFigure.Name, area, perimeter);
            }

            Console.WriteLine("Программа завершена.");
            Console.ReadKey();
        }

        // Вспомогательная функция для получения double значения от пользователя с обработкой ошибок
        private static double GetDoubleFromUser(string prompt)
        {
            double value;
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out value))
                {
                    return value;
                }
                Console.WriteLine("Неверный ввод. Пожалуйста, введите число.");
            }
        }
    }
       
}
