using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// Author- Brandon Mouser
    /// UID- U0962682
    /// 
    /// This Evaluator class takes in an expression of numbers and variables and computes the answer, if the answer exists. This class can compute basic mathematical expressions
    /// using the following operands: (,-,+,*,/,). All math done in this program is done using Integer Mathematics. Variables passed into the function can be computed 
    /// as well if the value of the variables are given by the user and passed into the Evaluate function. The Lookup delegate searches the users Dictionary or 
    /// function provided by the user to get the value of the variable. Variables can only start with a letter and end with a number, but can be as long as the user 
    /// wants, as long as the variable is defined by the user.
    /// This class has five methods in it. A public Evaluate function that takes the expression and the users defined Lookup for their variables to compute the answer.
    /// This class also includes four private methods that are used throughout the program to add, subtract, multiply, and divide two numbers together.
    /// 
    /// </summary>
    public static class Evaluator
    {
        public delegate int Lookup(String v);
        public static Stack<int> value;
        public static Stack<string> myOperator;

        /// <summary>
        /// This method evaluates the users given mathematical expression using their Lookup and returns the value to the user, if the value exists.
        /// If the value does not exist, or something goes wrong during the calculations, an ArgumentException is thrown with a description of what happened.
        /// </summary>
        /// <param name="exp">The mathematical expression passed in as a String by the user. </param>
        /// <param name="variableEvaluator"> The Lookup delegate passed in by the user as a function to find the number associated with the given variable,
        /// if the variable and number is defined by the user. </param>
        /// <returns> This function returns an integer, the answer to the mathematical expression passed in by the user. </returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            /// Create the new stacks for the numbers in the expression and the operands in the expression.
            value = new Stack<int>();
            myOperator = new Stack<string>();
            int number = 0;
            /// Split up the given expression.
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            /// Loop through the split up array to get each number, operand, and variable and put them in their respective places.
            for(int i = 0; i < substrings.Length; i++)
            {
                /// If the object at this place is a number, add it to the stack and proceed.
                if (Int32.TryParse(substrings[i], out number))
                {
                    value.Push(number);
                    if (myOperator.Count != 0)
                    {
                        if (myOperator.Peek() == "*")
                        {
                            Multiply();
                        }
                        else if (myOperator.Peek() == "/")
                        {
                            Divide();
                        }
                    }
                }
                /// If the object at this place is a '+' or '-' sign, look at the operand before it and do the calculations, then add this to the operand stack.
                else if (substrings[i] == "+" || substrings[i] == "-")
                {
                    if (myOperator.Count != 0)
                    {
                        if (myOperator.Peek() == "+")
                        {
                            Add();
                        }
                        else if (myOperator.Peek() == "-")
                        {
                            Subtract();
                        }
                        else if(myOperator.Peek() == "/")
                        {
                            Divide();
                        }
                        else if(myOperator.Peek() == "*")
                        {
                            Multiply();
                        }
                        myOperator.Push(substrings[i]);
                    }
                    /// If there are no operators in the operator stack, just add the operator to the stack, no calculations needed.
                    else if (myOperator.Count == 0)
                    {
                        myOperator.Push(substrings[i]);
                    }
                    /// If a '+' or '-' is read without any numbers in the value stack, throw an exception.
                    else if(value.Count == 0)
                    {
                        throw new ArgumentException("Numbers must be non-negative and operands must come after numbers. Check expression to make sure every operand is there, no negative numbers, and there are no additional numbers without operands.");
                    }
                }
                /// If the object at this place is a '/' or '*' just add it to the operand stack.
                else if (substrings[i] == "/" || substrings[i] == "*")
                {
                    myOperator.Push(substrings[i]);
                }
                /// If the object at this place is a '(', just add it to the operand stack.
                else if(substrings[i] == "(")
                {
                    myOperator.Push(substrings[i]);
                }
                else if(substrings[i] == ")")
                {
                    /// If there are no other operands in the stack, throw an exception for mismatched parentheses.
                    if(myOperator.Count == 0)
                    {
                        throw new ArgumentException("Mismatched parentheses, check your expression and try again.");
                    }
                    if (myOperator.Peek() == "+")
                    {
                        Add();
                        /// The next operand should be a '('. If it is not, throw an exception.
                        if(myOperator.Count == 0 || myOperator.Pop() != "(")
                        {
                            throw new ArgumentException("Mismatched parentheses, check your expression and try again.");
                        }
                    }
                    else if (myOperator.Peek() == "-")
                    {
                        Subtract();
                        /// The next operand should be a '('. If it is not, throw an exception.
                        if (myOperator.Count == 0 || myOperator.Pop() != "(")
                        {
                            throw new ArgumentException("Mismatched parentheses, check your expression and try again.");
                        }
                    }
                    else if (myOperator.Peek() == "(")
                    {
                        myOperator.Pop();
                    }
                    /// If there are operands before the parentheses, take care of them.
                    if (myOperator.Count != 0)
                    {
                        if (myOperator.Peek() == "/")
                        {
                            Divide();
                        }
                        else if (myOperator.Peek() == "*")
                        {
                            Multiply();
                        }
                    }
                }
                /// If the number is a variable, check for it and use the provided Lookup to get the variable number.
                else if (substrings[i].Length >= 2 && !Int32.TryParse(substrings[i], out number))
                {
                    int varValue;
                    char[] variableArray = substrings[i].ToCharArray();
                    if (Char.IsLetter(variableArray[0]) == true && Char.IsNumber(variableArray[variableArray.Length - 1]) == true)
                    {
                        varValue = variableEvaluator(substrings[i]);
                        value.Push(varValue);
                        if (myOperator.Count != 0)
                        {
                            if (myOperator.Peek() == "*")
                            {
                                Multiply();
                            }
                            else if (myOperator.Peek() == "/")
                            {
                                Divide();
                            }
                        }
                    }
                    /// If the variable is not a valid variable, starting with a letter and ending with a number, throw an exception.
                    else if (Char.IsLetter(variableArray[0]) == true && Char.IsNumber(variableArray[variableArray.Length - 1]) == false)
                    {
                        throw new ArgumentException(substrings[i] + " is not a valid variable. Enter a variable name beginning with any letter and ending with any number");
                    }
                    else if (Char.IsLetter(variableArray[0]) == false && Char.IsNumber(variableArray[variableArray.Length - 1]) == false)
                    {
                        throw new ArgumentException(substrings[i] + " is not a valid variable. Enter a variable name beginning with any letter and ending with any number");
                    }
                    else if (Char.IsLetter(variableArray[0]) == false && Char.IsNumber(variableArray[variableArray.Length - 1]) == true)
                    {
                        throw new ArgumentException(substrings[i] + " is not a valid variable. Enter a variable name beginning with any letter and ending with any number");
                    }
                }
            }
            /// If there are still operands in the operand stack after the loop has run, compute the resulting number until the operand stack is empty.
            while(myOperator.Count != 0)
            {
                if (myOperator.Peek() == "+")
                {
                    Add();
                }
                else if (myOperator.Peek() == "-")
                {
                    Subtract();
                }
                else if (myOperator.Peek() == "/")
                {
                    Divide();
                }
                else if (myOperator.Peek() == "*")
                {
                    Multiply();
                }
                else if(myOperator.Peek() == "(")
                {
                    throw new ArgumentException("Mismatched parentheses, check your expression to make sure your parentheses match up and try again");
                }
            }

            /// If there are no operands left in the stack, but more than one value is in the value stack, throw an exception.
            if(myOperator.Count == 0 && value.Count > 1)
            {
                throw new ArgumentException("Not enough operands for the number of values in the expression. Check your expression to make sure all operands are included and try again.");
            }
            /// Return the final value of the mathematical expression.
            return value.Pop();
        }

        /// <summary>
        /// A static method used to add two numbers together and push the resulting number back onto the value stack.
        /// </summary>
        private static void Add()
        {
            if(value.Count < 2)
            {
                throw new ArgumentException("Not enough operands/values for this operation. Check equation to make sure operands and values line up and try again.");
            }
            /// Get the two numbers from the stack.
            int poppedNumber1 = value.Pop();
            int poppedNumber2 = value.Pop();
            /// Get rid of the '+' operator in the operator stack.
            myOperator.Pop();
            int newNumber = poppedNumber2 + poppedNumber1;
            /// Push the result into the value stack.
            value.Push(newNumber);
        }

        /// <summary>
        /// A static method used to subtract two numbers from each other and push the resulting number back onto the value stack.
        /// </summary>
        private static void Subtract()
        {
            if (value.Count < 2)
            {
                throw new ArgumentException("Not enough operands/values for this operation. Check equation to make sure operands and values line up and try again.");
            }
            /// Get the two numbers from the stack.
            int poppedNumber1 = value.Pop();
            int poppedNumber2 = value.Pop();
            /// Get rid of the '-' operator in the operator stack.
            myOperator.Pop();
            int newNumber = poppedNumber2 - poppedNumber1;
            /// Push the result into the value stack.
            value.Push(newNumber);
        }

        /// <summary>
        /// A static method used to multiply two numbers together and push the resulting number back onto the value stack.
        /// </summary>
        private static void Multiply()
        {
            if (value.Count < 2)
            {
                throw new ArgumentException("Not enough operands/values for this operation. Check equation to make sure operands and values line up and try again.");
            }
            /// Get the two numbers from the stack.
            int poppedNumber1 = value.Pop();
            int poppedNumber2 = value.Pop();
            /// Get rid of the '*' operator in the operator stack.
            myOperator.Pop();
            int newNumber = poppedNumber1 * poppedNumber2;
            /// Push the result into the value stack.
            value.Push(newNumber);
        }

        /// <summary>
        /// A static method used to divide two numbers together and push the resulting number back onto the value stack.
        /// </summary>
        private static void Divide()
        {
            if (value.Count < 2)
            {
                throw new ArgumentException("Not enough operands/values for this operation. Check equation to make sure operands and values line up and try again.");
            }
            /// Get the two numbers from the stack.
            int poppedNumber1 = value.Pop();
            int poppedNumber2 = value.Pop();
            /// Get rid of the '/' operator in the operator stack.
            myOperator.Pop();
            // If the expression is dividing by 0, throw an error. Cannot divide by 0.
            if (poppedNumber1 == 0)
            {
                throw new ArgumentException("Unable to divide by 0, solution is undefined.");
            }
            else
            {
                int newNumber = poppedNumber2 / poppedNumber1;
                /// Push the result into the value stack.
                value.Push(newNumber);
            }
        }
    }
}
