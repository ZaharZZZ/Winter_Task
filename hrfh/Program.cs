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
        private const string CustomFormulasFilePath = "custom_formulas.txt";
        static void Main(string[] args)
        {
            Console.WriteLine("=== КАЛЬКУЛЯТОР ГЕОМЕТРИЧЕСКИХ ФИГУР ===");

            // Инициализация компонентов
            var figureReader = new FigureDataReader(figuresFilePath);
            var calculator = new FigureCalculator();
            var resultSaver = new ResultSaver(ResultsFilePath);

            // Загрузка фигур и пользовательских формул
            List<Figure> figures = LoadFigures(figureReader);
            LoadCustomFormulas(figures);

            // Основной цикл программы
            while (true)
            {
                try
                {
                    Console.WriteLine("\n=== ГЛАВНОЕ МЕНЮ ===");
                    Console.WriteLine("1. Выполнить расчет");
                    Console.WriteLine("2. Управление формулами");
                    Console.WriteLine("3. Просмотр истории расчетов");
                    Console.WriteLine("4. Выход");

                    int choice = GetIntFromUser("Выберите действие: ", 1, 4);

                    switch (choice)
                    {
                        case 1:
                            CalculateMode(figures, calculator, resultSaver);
                            break;
                        case 2:
                            FormulaManagementMenu(figures);
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

        private static void FormulaManagementMenu(List<Figure> figures)
        {
            while (true)
            {
                Console.WriteLine("\n=== УПРАВЛЕНИЕ ФОРМУЛАМИ ===");
                Console.WriteLine("1. Добавить новую формулу");
                Console.WriteLine("2. Просмотреть все формулы");
                Console.WriteLine("3. Удалить формулу");
                Console.WriteLine("4. Вернуться в главное меню");

                int choice = GetIntFromUser("Выберите действие: ", 1, 4);

                switch (choice)
                {
                    case 1:
                        AddCustomFormula(figures);
                        break;
                    case 2:
                        ShowAllFormulas(figures);
                        break;
                    case 3:
                        RemoveFormula(figures);
                        break;
                    case 4:
                        return;
                }
            }
        }

        #endregion

        #region Работа с фигурами и формулами

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

        private static void LoadCustomFormulas(List<Figure> figures)
        {
            if (!File.Exists(CustomFormulasFilePath))
                return;

            foreach (string line in File.ReadAllLines(CustomFormulasFilePath))
            {
                string[] parts = line.Split('|');
                if (parts.Length != 5) continue;

                string figureName = parts[0];
                string formulaType = parts[1];
                string formulaName = parts[2];
                string formula = parts[3];
                List<string> variables = parts[4].Split(',').Select(v => v.Trim()).ToList();

                var figure = figures.FirstOrDefault(f => f.Name.Equals(figureName, StringComparison.OrdinalIgnoreCase));
                if (figure == null) continue;

                var customFormula = new CustomFormula(formula, variables) { Name = formulaName };

                if (formulaType.Equals("Площадь", StringComparison.OrdinalIgnoreCase))
                {
                    figure.CustomAreaFormula = customFormula;
                }
                else if (formulaType.Equals("Периметр", StringComparison.OrdinalIgnoreCase))
                {
                    figure.CustomPerimeterFormula = customFormula;
                }
            }
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
                Console.WriteLine($"2. Пользовательская ({figure.CustomAreaFormula.Name}): {figure.CustomAreaFormula.Formula}");
            }

            Console.WriteLine("\nДоступные формулы периметра:");
            Console.WriteLine($"3. Стандартная: {figure.DefaultPerimeterFormula}");

            bool hasCustomPerimeter = figure.CustomPerimeterFormula != null;
            if (hasCustomPerimeter)
            {
                Console.WriteLine($"4. Пользовательская ({figure.CustomPerimeterFormula.Name}): {figure.CustomPerimeterFormula.Formula}");
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
                    return new[] { "width", "height" };
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

        #endregion

        #region Управление пользовательскими формулами

        private static void AddCustomFormula(List<Figure> figures)
        {
            Console.WriteLine("\n=== ДОБАВЛЕНИЕ НОВОЙ ФОРМУЛЫ ===");

            // Выбор фигуры
            Figure figure = SelectFigure(figures);

            // Выбор типа формулы
            Console.WriteLine("\nВыберите тип формулы:");
            Console.WriteLine("1. Формула площади");
            Console.WriteLine("2. Формула периметра");
            int typeChoice = GetIntFromUser("Ваш выбор: ", 1, 2);

            // Ввод данных формулы
            Console.Write("\nВведите название формулы: ");
            string formulaName = Console.ReadLine();

            Console.Write("Введите саму формулу (например: 'a * b + c'): ");
            string formula = Console.ReadLine();

            Console.Write("Введите параметры через запятую (например: 'a,b,c'): ");
            var variables = Console.ReadLine()
                .Split(',')
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .ToList();

            // Создание и сохранение формулы
            var customFormula = new CustomFormula(formula, variables) { Name = formulaName };

            if (typeChoice == 1)
            {
                figure.CustomAreaFormula = customFormula;
            }
            else
            {
                figure.CustomPerimeterFormula = customFormula;
            }

            // Сохранение в файл
            SaveCustomFormulaToFile(figure, typeChoice == 1, customFormula);

            Console.WriteLine("\nФормула успешно добавлена!");
        }

        private static void ShowAllFormulas(List<Figure> figures)
        {
            Console.WriteLine("\n=== ВСЕ ДОСТУПНЫЕ ФОРМУЛЫ ===");

            foreach (var figure in figures)
            {
                Console.WriteLine($"\nФигура: {figure.Name}");

                if (figure.CustomAreaFormula != null)
                {
                    Console.WriteLine($"  Площадь ({figure.CustomAreaFormula.Name}): {figure.CustomAreaFormula.Formula}");
                    Console.WriteLine($"    Параметры: {string.Join(", ", figure.CustomAreaFormula.Variables)}");
                }

                if (figure.CustomPerimeterFormula != null)
                {
                    Console.WriteLine($"  Периметр ({figure.CustomPerimeterFormula.Name}): {figure.CustomPerimeterFormula.Formula}");
                    Console.WriteLine($"    Параметры: {string.Join(", ", figure.CustomPerimeterFormula.Variables)}");
                }
            }
        }

        private static void RemoveFormula(List<Figure> figures)
        {
            Console.WriteLine("\n=== УДАЛЕНИЕ ФОРМУЛЫ ===");

            // Выбор фигуры
            Figure figure = SelectFigure(figures);

            // Получаем список всех формул для выбранной фигуры
            var formulas = new List<(string type, string name, CustomFormula formula)>();

            if (figure.CustomAreaFormula != null)
            {
                formulas.Add(("площади", figure.CustomAreaFormula.Name, figure.CustomAreaFormula));
            }

            if (figure.CustomPerimeterFormula != null)
            {
                formulas.Add(("периметра", figure.CustomPerimeterFormula.Name, figure.CustomPerimeterFormula));
            }

            if (formulas.Count == 0)
            {
                Console.WriteLine("Для выбранной фигуры нет пользовательских формул.");
                return;
            }

            // Вывод списка формул
            Console.WriteLine("\nДоступные формулы для удаления:");
            for (int i = 0; i < formulas.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Формула {formulas[i].type} ({formulas[i].name}): {formulas[i].formula.Formula}");
            }

            // Выбор формулы для удаления
            int choice = GetIntFromUser("Выберите формулу для удаления: ", 1, formulas.Count);
            var selectedFormula = formulas[choice - 1];

            // Удаление формулы
            if (selectedFormula.type == "площади")
            {
                figure.CustomAreaFormula = null;
            }
            else
            {
                figure.CustomPerimeterFormula = null;
            }

            // Обновление файла с формулами
            UpdateCustomFormulasFile(figures);

            Console.WriteLine("\nФормула успешно удалена!");
        }

        private static void SaveCustomFormulaToFile(Figure figure, bool isAreaFormula, CustomFormula formula)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(CustomFormulasFilePath, true))
                {
                    string formulaType = isAreaFormula ? "Площадь" : "Периметр";
                    string variables = string.Join(",", formula.Variables);
                    writer.WriteLine($"{figure.Name}|{formulaType}|{formula.Name}|{formula.Formula}|{variables}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении формулы: {ex.Message}");
            }
        }

        private static void UpdateCustomFormulasFile(List<Figure> figures)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(CustomFormulasFilePath, false))
                {
                    foreach (var figure in figures)
                    {
                        if (figure.CustomAreaFormula != null)
                        {
                            string variables = string.Join(",", figure.CustomAreaFormula.Variables);
                            writer.WriteLine($"{figure.Name}|Площадь|{figure.CustomAreaFormula.Name}|{figure.CustomAreaFormula.Formula}|{variables}");
                        }

                        if (figure.CustomPerimeterFormula != null)
                        {
                            string variables = string.Join(",", figure.CustomPerimeterFormula.Variables);
                            writer.WriteLine($"{figure.Name}|Периметр|{figure.CustomPerimeterFormula.Name}|{figure.CustomPerimeterFormula.Formula}|{variables}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении файла формул: {ex.Message}");
            }
        }

        #endregion

        #region История расчетов

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

        #region Вспомогательные методы

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
