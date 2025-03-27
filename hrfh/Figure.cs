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
        public string AreaFormula { get; set; }
        public string PerimeterFormula { get; set; }

        public Figure(string name, string areaFormula, string perimeterFormula)
        {
            Name = name;
            AreaFormula = areaFormula;
            PerimeterFormula = perimeterFormula;
        }

        public override string ToString()
        {
            return $"Название: {Name}, Площадь: {AreaFormula}, Периметр: {PerimeterFormula}";
        }
    }
}
