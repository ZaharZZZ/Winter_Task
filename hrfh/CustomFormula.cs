using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hrfh
{
    public class CustomFormula
    {
        public string Formula { get; set; }  // Например, "a * b + c"
        public List<string> Variables { get; set; } = new List<string>();  // Список переменных (a, b, c...)

        public CustomFormula(string formula, List<string> variables)
        {
            Formula = formula;
            Variables = variables;
        }
    }
}
