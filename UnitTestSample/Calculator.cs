using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestSample
{
    public class Calculator
    {
        public int Add(int firstNumber, int secondNumber)
        {
            return firstNumber + secondNumber;
        }

        public void SomeMethod()
        {
            throw new Exception();
        }


        public int Minus(int firstNumber, int secondNumber)
        {
            return firstNumber - secondNumber;
        }
    }
}
