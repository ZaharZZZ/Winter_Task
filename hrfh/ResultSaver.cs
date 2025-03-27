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

        public void SaveResult(string figureName, double area, double perimeter)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_filePath, true)) // Append to the file
                {
                    writer.WriteLine($"Фигура: {figureName}, Площадь: {area}, Периметр: {perimeter}");
                }
                Console.WriteLine($"Результат сохранен в файл: {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении в файл: {ex.Message}");
            }
        }
    }
}
