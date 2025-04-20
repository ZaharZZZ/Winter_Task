using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrfh
{
    public class CustomFormula
    {
        public string Name { get; set; }  // Название формулы
        public string Formula { get; set; }  // Сама формула
        public List<string> Variables { get; set; } = new List<string>();  // Список переменных

        public CustomFormula(string formula, List<string> variables)
        {
            Formula = formula;
            Variables = variables;
        }
    }
}
