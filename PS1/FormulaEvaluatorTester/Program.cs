using FormulaEvaluator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaEvaluatorTester
{
    class Tester
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Evaluator.Evaluate("(3+5)+(3+5))", SimpleLookup));
            Console.WriteLine(Evaluator.Evaluate("10/6/3)/2", OtherLookup));
            Console.Read();

        }
        public static int SimpleLookup(string v)
        {
            if (v == "3A")
                return 3;
            else
            {
                throw new ArgumentException();
            }
        }
        public static int OtherLookup(string v)
        {
            if (v == "ZZR45")
                return 10;
            else
                throw new ArgumentException();
        }
    }
}
