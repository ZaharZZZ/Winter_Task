using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrfh
{
    public class FigureCalculator
    {
        
        // Вычисляет площадь фигуры с возможностью выбора между стандартной и пользовательской формулой
  
        public double CalculateArea(Figure figure, Dictionary<string, double> parameters, bool useCustomFormula)
        {
            try
            {
                // Если выбрана пользовательская формула и она существует
                if (useCustomFormula && figure.CustomAreaFormula != null)
                {
                    Console.WriteLine($"Используется пользовательская формула площади: {figure.CustomAreaFormula.Formula}");
                    return CalculateByCustomFormula(figure.CustomAreaFormula, parameters);
                }

                // Стандартные формулы для различных фигур
                switch (figure.Name.ToLower())
                {
                    case "круг":
                        ValidateParameters(parameters, "radius");
                        return Math.PI * parameters["radius"] * parameters["radius"];

                    case "квадрат":
                        ValidateParameters(parameters, "side");
                        return parameters["side"] * parameters["side"];

                    case "прямоугольник":
                        ValidateParameters(parameters, "width", "height");
                        return parameters["width"] * parameters["height"];

                    case "треугольник":
                        ValidateParameters(parameters, "base", "height");
                        return 0.5 * parameters["base"] * parameters["height"];

                    case "ромб":
                        ValidateParameters(parameters, "diagonal1", "diagonal2");
                        return (parameters["diagonal1"] * parameters["diagonal2"]) / 2;

                    case "трапеция":
                        ValidateParameters(parameters, "base1", "base2", "height");
                        return ((parameters["base1"] + parameters["base2"]) / 2) * parameters["height"];

                    default:
                        throw new ArgumentException($"Неизвестная фигура: {figure.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка расчета площади: {ex.Message}");
                return double.NaN;
            }
        }

        /// <summary>
        /// Вычисляет периметр фигуры с возможностью выбора между стандартной и пользовательской формулой
        /// </summary>
        public double CalculatePerimeter(Figure figure, Dictionary<string, double> parameters, bool useCustomFormula)
        {
            try
            {
                // Если выбрана пользовательская формула и она существует
                if (useCustomFormula && figure.CustomPerimeterFormula != null)
                {
                    Console.WriteLine($"Используется пользовательская формула периметра: {figure.CustomPerimeterFormula.Formula}");
                    return CalculateByCustomFormula(figure.CustomPerimeterFormula, parameters);
                }

                // Стандартные формулы для различных фигур
                switch (figure.Name.ToLower())
                {
                    case "круг":
                        ValidateParameters(parameters, "radius");
                        return 2 * Math.PI * parameters["radius"];

                    case "квадрат":
                        ValidateParameters(parameters, "side");
                        return 4 * parameters["side"];

                    case "прямоугольник":
                        ValidateParameters(parameters, "width", "height");
                        return 2 * (parameters["width"] + parameters["height"]);

                    case "треугольник":
                        ValidateParameters(parameters, "a", "b", "c");
                        return parameters["a"] + parameters["b"] + parameters["c"];

                    case "ромб":
                        ValidateParameters(parameters, "side");
                        return 4 * parameters["side"];

                    case "трапеция":
                        ValidateParameters(parameters, "side1", "side2", "base1", "base2");
                        return parameters["base1"] + parameters["base2"] + parameters["side1"] + parameters["side2"];

                    default:
                        throw new ArgumentException($"Неизвестная фигура: {figure.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка расчета периметра: {ex.Message}");
                return double.NaN;
            }
        }

        /// <summary>
        /// Вычисляет значение по пользовательской формуле
        /// </summary>
        private double CalculateByCustomFormula(CustomFormula formula, Dictionary<string, double> parameters)
        {
            try
            {
                // Проверяем наличие всех необходимых параметров
                ValidateParameters(parameters, formula.Variables.ToArray());

                // Создаем копию формулы для замены переменных
                string expression = formula.Formula;

                // Заменяем переменные на их значения
                foreach (var variable in formula.Variables)
                {
                    expression = expression.Replace(variable, parameters[variable].ToString(CultureInfo.InvariantCulture));
                }

                // Вычисляем выражение с помощью DataTable.Compute
                var result = new System.Data.DataTable().Compute(expression, null);

                return Convert.ToDouble(result);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Ошибка вычисления формулы '{formula.Formula}': {ex.Message}");
            }
        }

        /// <summary>
        /// Проверяет наличие всех необходимых параметров
        /// </summary>
        private void ValidateParameters(Dictionary<string, double> parameters, params string[] requiredParams)
        {
            foreach (var param in requiredParams)
            {
                if (!parameters.ContainsKey(param))
                {
                    throw new ArgumentException($"Не хватает параметра: {param}");
                }
            }
        }

        /// <summary>
        /// Вычисляет площадь по названию фигуры и параметрам (удобная обертка)
        /// </summary>
        public double CalculateArea(string figureName, Dictionary<string, double> parameters)
        {
            var figure = new Figure(figureName, "", "");
            return CalculateArea(figure, parameters, false);
        }

        /// <summary>
        /// Вычисляет периметр по названию фигуры и параметрам (удобная обертка)
        /// </summary>
        public double CalculatePerimeter(string figureName, Dictionary<string, double> parameters)
        {
            var figure = new Figure(figureName, "", "");
            return CalculatePerimeter(figure, parameters, false);
        }
    }
}
