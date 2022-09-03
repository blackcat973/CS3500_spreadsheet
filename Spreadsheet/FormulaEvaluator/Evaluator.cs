using System;
using EvaExtentions;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

/**
 * This program is for evaluating arithmetic expression using standard infix notation.
 * I made it possible to calculate according to the priority of the tokens(operators).
 *  
 *  @author     SangYoon Cho
 *  @date       2022/09/02 (Y/M/D)
 *  @version    1.4 ver
 *                  -> Modify calculator method 
 *                  -> Combine 4 operator methods in one method
 */


/**
 * This namespace is for Extiontion methods for Evaluator.
 * It invokes stack push method with string(token) variable, and operator calculation helper methods. *
 */
namespace EvaExtentions
{
    public static class Extensions
    {
        /// <summary>
        /// This method is for push into the variable stack with one conditions. 
        /// </summary>
        /// <param name="varStack"> variable stack </param>
        /// <param name="opStack"> operator stack </param>
        /// <param name="num"> the integer stored into the variable stack </param>
        public static void pushToVar(Stack<int> varStack, Stack<string> opStack, int num)
        {
            varStack.Push(num);
            // Check if there's an operator in the operatorStack
            // which means check if there's expression that didn't calculate yet.
            // This is only for multiply or division.
            if (opStack.Count() != 0)
            {
                if (isOnTop(opStack, "/") && countStackValue(varStack, 1))
                {
                    opStack.Pop();
                    varStack.Push(calculateVariable("/", varStack.Pop(), varStack.Pop()));
                }
                else if (isOnTop(opStack, "*") && countStackValue(varStack, 1))
                {
                    opStack.Pop();
                    varStack.Push(calculateVariable("*", varStack.Pop(), varStack.Pop()));
                }
            }
        }

        /// <summary>
        /// This method is for push into the operator stack.
        /// It checks if the conditions of each of the operators are met.
        /// </summary>
        /// <param name="varStack"> the variable stack </param>
        /// <param name="opStack"> the operator stack </param>
        /// <param name="opr"> string - the operator stored into the operator stack </param>
        /// <exception cref="ArgumentException">
        /// Exception 1. If there isn't '(' in the oeprator stack.
        /// Exception 2. If there's too many operators in the operatorStack.
        /// </exception>
        public static void pushToOp(Stack<int> varStack, Stack<string> opStack, string opr)
        {
            if ((opr.Equals("+") || opr.Equals("-")) && countStackValue(varStack, 2))
            {
                // If + or - is at the top of the operator stack,
                // pop the value stack twice and the operator stack once,
                // then apply the popped operator to the popped numbers,
                // then push the result onto the value stack.
                if (isOnTop(opStack, "+"))
                {
                    opStack.Pop();
                    pushToVar(varStack, opStack, calculateVariable("+", varStack.Pop(), varStack.Pop()));
                }
                else if (isOnTop(opStack, "-"))
                {
                    opStack.Pop();
                    pushToVar(varStack, opStack, calculateVariable("-", varStack.Pop(), varStack.Pop()));
                }
                opStack.Push(opr);
            }
            // If opr is a right parenthesis
            else if (opr.Equals(")"))
            {
                // If the value stack contains more than 2 values 
                if (countStackValue(varStack, 2))
                {
                    if (isOnTop(opStack, "+"))
                    {
                        opStack.Pop();
                        pushToVar(varStack, opStack, calculateVariable("+", varStack.Pop(), varStack.Pop()));
                    }
                    else if (isOnTop(opStack, "-"))
                    {
                        opStack.Pop();
                        pushToVar(varStack, opStack, calculateVariable("-", varStack.Pop(), varStack.Pop()));

                    }
                    // Exception 1
                    // After evaluating + and -, the top of the operator next to + and - should be a '('.
                    if (opStack.Count == 0 || !isOnTop(opStack, "("))
                        throw new ArgumentException("'(' isn't found where expected.");
                    else
                    {
                        opStack.Pop();
                        // Finally if * or / is at the top of the oeprator stack,
                        // calculate it with two popped numbers.
                        if (opStack.Count != 0)
                        {
                            if (isOnTop(opStack, "/") && countStackValue(varStack, 1))
                            {
                                opStack.Pop();
                                pushToVar(varStack, opStack, calculateVariable("/", varStack.Pop(), varStack.Pop()));
                            }
                            else if (isOnTop(opStack, "*") && countStackValue(varStack, 1))
                            {
                                opStack.Pop();
                                pushToVar(varStack, opStack, calculateVariable("*", varStack.Pop(), varStack.Pop()));
                            }
                        }
                    }
                }
                // If there's no any operator in the operatorStack or There's no '('.
                else if (!countStackValue(varStack, 2))
                {
                    // Exception 1
                    if (opStack.Count == 0 || !isOnTop(opStack, "("))
                        throw new ArgumentException("'(' isn't found where expected.");
                    else if (isOnTop(opStack, "("))
                        opStack.Pop();
                    // Exception 2
                    else
                        throw new ArgumentException("You input too many operator.");
                }
            }
            else
                opStack.Push(opr);
        }

        /// <summary>
        /// Extension method for checking peek of operator stack.
        /// </summary>
        /// <param name="opStack"> popped string from the operator stack </param>
        /// <param name="topStack"> the string which is gonna be checking </param>
        /// <returns> If topStack value is equal to the peek value of the operator stack, return true. 
        ///                                                                               else false
        /// </returns>
        public static bool isOnTop(Stack<string> opStack, string topStack)
        {
            if (opStack.Peek().Equals(topStack))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Extension method for checking the count(size) of the value stack.
        /// </summary>
        /// <param name="valStack"> Value stack for checking count </param>
        /// <returns> If value stack has more than input value, return true
        ///                                                      else false. 
        /// </returns>
        public static bool countStackValue(Stack<int> valStack, int value)
        {
            if (valStack.Count >= value)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Calculator method. This method takes the operator and calculate for each operator.
        /// </summary>
        /// <param name="num1"> popped number from the variable stack </param>
        /// <param name="num2"> popped number from the variable stack </param>
        /// <returns> result of num1 (operator) num2 </returns>
        /// <exception cref="ArgumentException">
        /// Exception 1. If it trys to divide number by ZERO.
        /// </exception>
        public static int calculateVariable(string opr, int num1, int num2)
        {
            if (opr.Equals("+"))
                return num1 + num2;
            else if (opr.Equals("-"))
                return num2 - num1;
            else if (opr.Equals("*"))
                return num1 * num2;
            else if (opr.Equals("/"))
            {
                int result = 0;
                try
                {
                    if (num1 != 0)
                        result = num2 / num1;
                    else
                        // Exception 1
                        throw new ArgumentException("Attempted to divide by zero.");
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
                return result;
            }
            else
                throw new ArgumentException("Wrong operator is used.");
        }
    }
}

/**
 * This namespace is for actual Evaluator.
 */
namespace FormulaEvaluator
{
    /// <summary>
    /// This class includes every class for evaluation.
    /// It splits input string to store every single data type into each proper type of the stack
    /// for checking integer or token.
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Delegate for input variable (not integer).
        /// </summary>
        /// <param name="v"> First string parameter </param>
        /// <returns> int value for converting string to int </returns>
        public delegate int Lookup(string v);
        /// <summary>
        /// This method splits string and stores data into the proper stack such as
        /// if data is an integer, it is stored into the int stack.
        /// Also it returns result value of the arithmetic expression.
        /// </summary>
        /// <param name="exp"> Input string </param>
        /// <param name="variableEvaluator"> 
        /// If there's an variable, delegate change it to the integer what user want to input 
        /// </param>
        /// <returns> The result of the arithmetic expression </returns>
        /// <exception cref="ArgumentException">
        /// There are 5 exceptions. Every exception throws ArgumentException
        /// Exception 1. If variable doesn't consist of one or more letters followed by the number 
        /// Exception 2. If there're no datas in the stack. -> Empty string
        /// Exception 3. If there're only tokens(operators). -> Empty integer
        /// Exception 4. If tokens are overbalanced.
        /// Exception 5. If integers are overbalanced.
        /// </exception>
        public static int Evaluate(string exp, Lookup variableEvaluator)
        {
            // Store integer
            Stack<int> valueStack = new Stack<int>();
            // Store operator(tokens)
            Stack<string> operatorStack = new Stack<string>();
            int result = 0;

            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            foreach (string s in substrings)
            {
                // Check bool if s can change to the integer, and if true, store into the valueStack.
                if (int.TryParse(s, out int value))
                    Extensions.pushToVar(valueStack, operatorStack, value);
                else if (s.Equals("(") || s.Equals(")") || s.Equals("+") || s.Equals("-") || s.Equals("/") || s.Equals("*"))
                    Extensions.pushToOp(valueStack, operatorStack, s);
                // Exclude white space
                else if (s.Equals("") || s.Equals(" "))
                    continue;
                else
                {
                    try
                    {
                        bool checkString = s.Any(char.IsDigit);
                        // Exception 1
                        // Check if string doesn't include the integer, throw exception
                        if (!checkString)
                            throw new ArgumentException("Variable doesn't consist of one or more letters FOLLOWED by one or more digits.");
                        else
                            Extensions.pushToVar(valueStack, operatorStack, variableEvaluator(s));
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                }
            }

            // Exception 2
            if (valueStack.Count == 0 && operatorStack.Count == 0)
                throw new ArgumentException("Cannot use empty string.");
            // Exception 3
            else if (valueStack.Count == 0 && operatorStack.Count >= 1)
                throw new ArgumentException("There is no variable.");
            // Exception 4
            else if (valueStack.Count == 1 && operatorStack.Count >= 1)
                throw new ArgumentException("Input too many operators.");
            // Exception 5
            else if (valueStack.Count >= 2 + operatorStack.Count)
                throw new ArgumentException("Operator is not enough.");

            // Check the operatorStack if there's still remain operator(token).
            // Only + and - are accepted.
            if (operatorStack.Count() != 0 && Extensions.countStackValue(valueStack, 2))
            {
                if (Extensions.isOnTop(operatorStack, "+"))
                    result = Extensions.calculateVariable("+", valueStack.Pop(), valueStack.Pop());
                else if (Extensions.isOnTop(operatorStack, "-"))
                {
                    result = Extensions.calculateVariable("-", valueStack.Pop(), valueStack.Pop());
                }
            }
            else
                result = valueStack.Pop();

            return result;
        }
    }
}