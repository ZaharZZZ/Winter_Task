using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrfh
{
    public class Figure
    {
        public string Name { get; set; }
        public string DefaultAreaFormula { get; set; }  // Стандартная формула
        public string DefaultPerimeterFormula { get; set; }  // Стандартная формула
        public CustomFormula CustomAreaFormula { get; set; }  // Пользовательская формула
        public CustomFormula CustomPerimeterFormula { get; set; }  // Пользовательская формула

        public Figure(string name, string defaultAreaFormula, string defaultPerimeterFormula)
        {
            Name = name;
            DefaultAreaFormula = defaultAreaFormula;
            DefaultPerimeterFormula = defaultPerimeterFormula;
        }

        public override string ToString()
        {
            string areaInfo = CustomAreaFormula != null
                ? $"Площадь (пользовательская): {CustomAreaFormula.Formula}"
                : $"Площадь: {DefaultAreaFormula}";

            string perimeterInfo = CustomPerimeterFormula != null
                ? $"Периметр (пользовательская): {CustomPerimeterFormula.Formula}"
                : $"Периметр: {DefaultPerimeterFormula}";

            return $"Название: {Name}, {areaInfo}, {perimeterInfo}";
        }
    }
}
