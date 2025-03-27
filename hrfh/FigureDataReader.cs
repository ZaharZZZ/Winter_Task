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
                        string[] parts = line.Split(','); // Разделяем строку на части по запятой
                        if (parts.Length == 3)
                        {
                            string name = parts[0].Trim();
                            string areaFormula = parts[1].Trim();
                            string perimeterFormula = parts[2].Trim();
                            figures.Add(new Figure(name, areaFormula, perimeterFormula));
                        }
                        else
                        {
                            Console.WriteLine($"Неверный формат строки в файле: {line}");
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Файл не найден: {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            }

            return figures;
        }
    }
}
