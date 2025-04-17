using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrfh
{
    public class ResultSaver
    {
        private readonly string _filePath;

        public ResultSaver(string filePath)
        {
            _filePath = filePath;
        }

        // Сохраняет результаты расчетов
        public void SaveResult(string figureName, double area, double perimeter)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_filePath, true))
                {
                    writer.WriteLine($"[{DateTime.Now}] Фигура: {figureName}, Площадь: {area}, Периметр: {perimeter}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении результата: {ex.Message}");
            }
        }

        // Сохраняет пользовательскую формулу в файл figures.txt
        public void SaveCustomFormula(string figureName, bool isAreaFormula, CustomFormula formula, string figuresFilePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("figures.txt", true))
                {
                    string formulaType = isAreaFormula ? "Пользовательская площадь" : "Пользовательский периметр";
                    string variables = string.Join(",", formula.Variables);
                    writer.WriteLine($"!{figureName},{formulaType},{formula.Formula},{variables}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении формулы: {ex.Message}");
            }
        }
    }
}
