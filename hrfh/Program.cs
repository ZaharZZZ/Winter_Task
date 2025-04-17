using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrfh
{
    internal class Program
    {
        private const string figuresFilePath = "figures.txt";
        private const string ResultsFilePath = "results.txt";
        static void Main(string[] args)
        {
            Console.WriteLine("=== КАЛЬКУЛЯТОР ГЕОМЕТРИЧЕСКИХ ФИГУР ===");

            // Инициализация компонентов
            var figureReader = new FigureDataReader(figuresFilePath);
            var calculator = new FigureCalculator();
            var resultSaver = new ResultSaver(ResultsFilePath);

            // Загрузка фигур
            List<Figure> figures = LoadFigures(figureReader);

            // Основной цикл программы
            while (true)
            {
                try
                {
                    Console.WriteLine("\n=== ГЛАВНОЕ МЕНЮ ===");
                    Console.WriteLine("1. Выполнить расчет");
                    Console.WriteLine("2. Управление формулами");
                    Console.WriteLine("3. Просмотр истории");
                    Console.WriteLine("4. Выход");

                    int choice = GetIntFromUser("Выберите действие: ", 1, 4);

                    switch (choice)
                    {
                        case 1:
                            CalculateMode(figures, calculator, resultSaver);
                            break;
                        case 2:
                            FormulasManagementMode(figures, resultSaver);
                            break;
                        case 3:
                            ShowHistory();
                            break;
                        case 4:
                            Console.WriteLine("Программа завершена.");
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    Console.WriteLine("Попробуйте еще раз.");
                }
            }
        }

        #region Режимы работы

        private static void CalculateMode(List<Figure> figures, FigureCalculator calculator, ResultSaver resultSaver)
        {
            Console.WriteLine("\n=== РЕЖИМ РАСЧЕТА ===");

            // Выбор фигуры
            Figure figure = SelectFigure(figures);

            // Выбор формул
            var (useCustomArea, useCustomPerimeter) = SelectFormulas(figure);

            // Ввод параметров
            Dictionary<string, double> parameters = InputParameters(figure, useCustomArea, useCustomPerimeter);

            // Выполнение расчетов
            CalculationResult result = PerformCalculations(figure, parameters, calculator, useCustomArea, useCustomPerimeter);

            // Вывод результатов
            DisplayResults(figure, result, useCustomArea, useCustomPerimeter);

            // Сохранение результатов
            resultSaver.SaveResult(figure.Name, result.Area, result.Perimeter);
        }

        private static void FormulasManagementMode(List<Figure> figures, ResultSaver resultSaver)
        {
            Console.WriteLine("\n=== УПРАВЛЕНИЕ ФОРМУЛАМИ ===");
            Console.WriteLine("1. Добавить новую формулу");
            Console.WriteLine("2. Просмотреть все формулы");
            Console.WriteLine("3. Вернуться");

            int choice = GetIntFromUser("Выберите действие: ", 1, 3);

            switch (choice)
            {
                case 1:
                    AddCustomFormula(figures, resultSaver);
                    break;
                case 2:
                    ShowAllFormulas(figures);
                    break;
            }
        }

        #endregion

        #region Вспомогательные методы

        private static List<Figure> LoadFigures(FigureDataReader reader)
        {
            List<Figure> figures = reader.ReadFigures();

            if (figures.Count == 0)
            {
                figures = new List<Figure>
            {
                new Figure("Круг", "Pi * r^2", "2 * Pi * r"),
                new Figure("Квадрат", "a^2", "4 * a"),
                new Figure("Прямоугольник", "a * b", "2(a + b)"),
                new Figure("Треугольник", "(b * h)/2", "a + b + c"),
               
            };
                Console.WriteLine("Загружены стандартные фигуры");
            }

            return figures;
        }

        private static Figure SelectFigure(List<Figure> figures)
        {
            Console.WriteLine("\nДоступные фигуры:");
            for (int i = 0; i < figures.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {figures[i].Name}");
            }

            int choice = GetIntFromUser("Выберите фигуру: ", 1, figures.Count);
            return figures[choice - 1];
        }

        private static (bool useCustomArea, bool useCustomPerimeter) SelectFormulas(Figure figure)
        {
            Console.WriteLine("\nДоступные формулы площади:");
            Console.WriteLine($"1. Стандартная: {figure.DefaultAreaFormula}");

            bool hasCustomArea = figure.CustomAreaFormula != null;
            if (hasCustomArea)
            {
                Console.WriteLine($"2. Пользовательская: {figure.CustomAreaFormula.Formula}");
            }

            Console.WriteLine("\nДоступные формулы периметра:");
            Console.WriteLine($"3. Стандартная: {figure.DefaultPerimeterFormula}");

            bool hasCustomPerimeter = figure.CustomPerimeterFormula != null;
            if (hasCustomPerimeter)
            {
                Console.WriteLine($"4. Пользовательская: {figure.CustomPerimeterFormula.Formula}");
            }

            int areaChoice = GetIntFromUser(
                "Выберите формулу для площади: ",
                1,
                hasCustomArea ? 2 : 1);

            int perimeterChoice = GetIntFromUser(
                "Выберите формулу для периметра: ",
                3,
                hasCustomPerimeter ? 4 : 3);

            return (areaChoice == 2, perimeterChoice == 4);
        }


        private static Dictionary<string, double> InputParameters(
            Figure figure,
            bool useCustomArea,
            bool useCustomPerimeter)
        {
            var parameters = new Dictionary<string, double>();

            // Получаем список всех необходимых параметров
            var requiredParams = new HashSet<string>();

            if (useCustomArea && figure.CustomAreaFormula != null)
            {
                requiredParams.UnionWith(figure.CustomAreaFormula.Variables);
            }
            else if (!useCustomArea)
            {
                requiredParams.UnionWith(GetStandardParametersForFigure(figure, forArea: true));
            }

            if (useCustomPerimeter && figure.CustomPerimeterFormula != null)
            {
                requiredParams.UnionWith(figure.CustomPerimeterFormula.Variables);
            }
            else if (!useCustomPerimeter)
            {
                requiredParams.UnionWith(GetStandardParametersForFigure(figure, forArea: false));
            }

            // Запрашиваем значения параметров
            foreach (var param in requiredParams)
            {
                parameters[param] = GetDoubleFromUser($"Введите значение '{param}': ");
            }

            return parameters;
        }

        private static IEnumerable<string> GetStandardParametersForFigure(Figure figure, bool forArea)
        {
            switch (figure.Name.ToLower())
            {
                case "круг":
                    return new[] { "radius" };
                case "квадрат":
                    return new[] { "side" };
                case "прямоугольник":
                    return forArea ? new[] { "width", "height" }
                                   : new[] { "width", "height" };
                case "треугольник":
                    return forArea ? new[] { "base", "height" }
                                   : new[] { "a", "b", "c" };
                case "ромб":
                    return forArea ? new[] { "diagonal1", "diagonal2" }
                                   : new[] { "side" };
                case "трапеция":
                    return forArea ? new[] { "base1", "base2", "height" }
                                   : new[] { "base1", "base2", "side1", "side2" };
                default:
                    throw new ArgumentException($"Неизвестная фигура: {figure.Name}");
            }
        }

        private static CalculationResult PerformCalculations(
            Figure figure,
            Dictionary<string, double> parameters,
            FigureCalculator calculator,
            bool useCustomArea,
            bool useCustomPerimeter)
        {
            double area = calculator.CalculateArea(figure, parameters, useCustomArea);
            double perimeter = calculator.CalculatePerimeter(figure, parameters, useCustomPerimeter);

            return new CalculationResult(area, perimeter);
        }

        private static void DisplayResults(
            Figure figure,
            CalculationResult result,
            bool useCustomArea,
            bool useCustomPerimeter)
        {
            Console.WriteLine("\n=== РЕЗУЛЬТАТЫ ===");
            Console.WriteLine($"Фигура: {figure.Name}");

            Console.WriteLine("\nПлощадь:");
            Console.WriteLine($"Формула: {(useCustomArea ? figure.CustomAreaFormula.Formula : figure.DefaultAreaFormula)}");
            Console.WriteLine($"Результат: {result.Area}");

            Console.WriteLine("\nПериметр:");
            Console.WriteLine($"Формула: {(useCustomPerimeter ? figure.CustomPerimeterFormula.Formula : figure.DefaultPerimeterFormula)}");
            Console.WriteLine($"Результат: {result.Perimeter}");
        }

        private static void AddCustomFormula(List<Figure> figures, ResultSaver resultSaver)
        {
            Figure figure = SelectFigure(figures);

            Console.WriteLine("\nВыберите тип формулы:");
            Console.WriteLine("1. Формула площади");
            Console.WriteLine("2. Формула периметра");
            int typeChoice = GetIntFromUser("Ваш выбор: ", 1, 2);

            Console.Write("\nВведите формулу (например: 'a * b + c'): ");
            string formula = Console.ReadLine();

            Console.Write("Введите параметры через запятую (например: 'a,b,c'): ");
            var variables = Console.ReadLine()
                .Split(',')
                .Select(v => v.Trim())
                .ToList();

            var customFormula = new CustomFormula(formula, variables);

            if (typeChoice == 1)
            {
                figure.CustomAreaFormula = customFormula;
            }
            else
            {
                figure.CustomPerimeterFormula = customFormula;
            }

            // Сохраняем в файл
            resultSaver.SaveCustomFormula(
                figure.Name,
                typeChoice == 1,
                customFormula,
                figuresFilePath);

            Console.WriteLine("\nФормула успешно добавлена!");
        }

        private static void ShowAllFormulas(List<Figure> figures)
        {
            Console.WriteLine("\n=== ВСЕ ФОРМУЛЫ ===");
            foreach (var figure in figures)
            {
                Console.WriteLine($"\nФигура: {figure.Name}");
                Console.WriteLine($"Площадь: {figure.DefaultAreaFormula}");
                if (figure.CustomAreaFormula != null)
                {
                    Console.WriteLine($"Пользовательская площадь: {figure.CustomAreaFormula.Formula}");
                    Console.WriteLine($"Параметры: {string.Join(", ", figure.CustomAreaFormula.Variables)}");
                }

                Console.WriteLine($"\nПериметр: {figure.DefaultPerimeterFormula}");
                if (figure.CustomPerimeterFormula != null)
                {
                    Console.WriteLine($"Пользовательский периметр: {figure.CustomPerimeterFormula.Formula}");
                    Console.WriteLine($"Параметры: {string.Join(", ", figure.CustomPerimeterFormula.Variables)}");
                }
            }
        }

        private static void ShowHistory()
        {
            if (!File.Exists(ResultsFilePath))
            {
                Console.WriteLine("История расчетов пуста.");
                return;
            }

            Console.WriteLine("\n=== ИСТОРИЯ РАСЧЕТОВ ===");
            foreach (string line in File.ReadAllLines(ResultsFilePath))
            {
                Console.WriteLine(line);
            }
        }

        #endregion

        #region Вспомогательные методы ввода

        private static int GetIntFromUser(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int result) && result >= min && result <= max)
                {
                    return result;
                }
                Console.WriteLine($"Ошибка! Введите число от {min} до {max}.");
            }
        }

        private static double GetDoubleFromUser(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                {
                    return result;
                }
                Console.WriteLine("Ошибка! Введите число.");
            }
        }

        #endregion

        #region Вспомогательные классы

        private class CalculationResult
        {
            public double Area { get; }
            public double Perimeter { get; }

            public CalculationResult(double area, double perimeter)
            {
                Area = area;
                Perimeter = perimeter;
            }
        }

        #endregion
    }

}
