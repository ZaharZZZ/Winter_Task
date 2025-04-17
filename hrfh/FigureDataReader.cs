using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrfh
{
    public class FigureDataReader
    {
        private readonly string _filePath;

        public FigureDataReader(string filePath)
        {
            _filePath = filePath;
        }

        public List<Figure> ReadFigures()
        {
            List<Figure> figures = new List<Figure>();

            try
            {
                using (StreamReader reader = new StreamReader(_filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("!")) // Пользовательская формула
                        {
                            ParseCustomFormula(line, figures);
                        }
                        else // Стандартная фигура
                        {
                            ParseStandardFigure(line, figures);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Файл не найден: {_filePath}. Будет создан новый.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            }

            return figures;
        }

        private void ParseStandardFigure(string line, List<Figure> figures)
        {
            string[] parts = line.Split(',');
            if (parts.Length == 3)
            {
                string name = parts[0].Trim();
                string areaFormula = parts[1].Trim();
                string perimeterFormula = parts[2].Trim();
                figures.Add(new Figure(name, areaFormula, perimeterFormula));
            }
        }

        private void ParseCustomFormula(string line, List<Figure> figures)
        {
            // Формат: !Фигура,ТипФормулы,Формула,ПеременныеЧерезЗапятую
            string[] parts = line.Substring(1).Split(',');
            if (parts.Length >= 4)
            {
                string figureName = parts[0].Trim();
                string formulaType = parts[1].Trim().ToLower();
                string formula = parts[2].Trim();
                List<string> variables = parts[3].Split(',').Select(v => v.Trim()).ToList();

                var figure = figures.FirstOrDefault(f => f.Name.Equals(figureName, StringComparison.OrdinalIgnoreCase));
                if (figure != null)
                {
                    var customFormula = new CustomFormula(formula, variables);
                    if (formulaType.Contains("площад")) // "Площадь" или "пользовательская площадь"
                        figure.CustomAreaFormula = customFormula;
                    else if (formulaType.Contains("периметр")) // "Периметр" или "пользовательский периметр"
                        figure.CustomPerimeterFormula = customFormula;
                }
            }
        
    }
    }
}
