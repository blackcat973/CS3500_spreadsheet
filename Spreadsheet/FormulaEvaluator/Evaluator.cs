using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

/**
 * This program is for evaluating arithmetic expression using standard infix notation.
 * I made it possible to calculate according to the priority of the tokens(operators).
 *  
 *  @author     SangYoon Cho
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
        /// 1. If variable doesn't consist of one or more letters followed by the number 
        /// 2. If there're no datas in the stack. -> Empty string
        /// 3. If there're only tokens(operators). -> Empty integer
        /// 4. If tokens are overbalanced.
        /// 5. If integers are overbalanced.
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
                    pushToVar(valueStack, operatorStack, value);
                else if (s.Equals("(") || s.Equals(")") || s.Equals("+") || s.Equals("-") || s.Equals("/") || s.Equals("*"))
                    pushToOp(valueStack, operatorStack, s);
                // Exclude white space
                else if (s.Equals("") || s.Equals(" "))
                    continue;
                else
                {
                    try
                    {
                        bool checkString = s.Any(char.IsDigit);
                        // 1
                        // Check if string doesn't include the integer, throw exception
                        if (!checkString)
                            throw new ArgumentException("Variable doesn't consist of one or more letters FOLLOWED by one or more digits.");
                        else
                            pushToVar(valueStack, operatorStack, variableEvaluator(s));
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }                    
                }                    
            }

            // exception - 2
            if (valueStack.Count == 0 && operatorStack.Count == 0)
                throw new ArgumentException("Cannot use empty string.");
            // 3
            else if (valueStack.Count == 0 && operatorStack.Count >= 1)
                throw new ArgumentException("There is no variable.");
            // 4
            else if (valueStack.Count == 1 && operatorStack.Count >= 1)
                throw new ArgumentException("Input too many operators.");
            // 5
            else if (valueStack.Count >= 2 + operatorStack.Count)
                throw new ArgumentException("Operator is not enough.");

            // Check the operatorStack if there's still remain operator(token).
            // Only + and - are accepted.
            if (operatorStack.Count() != 0 && valueStack.Count == 2)
            {
                if (operatorStack.Peek().Equals("+"))
                    result = addVariable(valueStack.Pop(), valueStack.Pop());
                else if (operatorStack.Peek().Equals("-"))
                {
                    result = subtractVariable(valueStack.Pop(), valueStack.Pop());
                }
            }
            else
                result = valueStack.Pop();

            return result;
        }

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
                if (opStack.Peek().Equals("/") && varStack.Count >= 1)
                {
                    opStack.Pop();
                    varStack.Push(divideVariable(varStack.Pop(), varStack.Pop()));
                }
                else if (opStack.Peek().Equals("*") && varStack.Count >= 1)
                {
                    opStack.Pop();
                    varStack.Push(multiVariable(varStack.Pop(), varStack.Pop()));
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
        /// 1. If there isn't '(' in the oeprator stack.
        /// 2. If there's too many operators in the operatorStack.
        /// </exception>
        public static void pushToOp(Stack<int> varStack, Stack<string> opStack, string opr)
        {
            if ((opr.Equals("+") || opr.Equals("-")) && varStack.Count >= 2)
            {
                // If + or - is at the top of the operator stack,
                // pop the value stack twice and the operator stack once,
                // then apply the popped operator to the popped numbers,
                // then push the result onto the value stack.
                if (opStack.Peek().Equals("+"))
                {
                    opStack.Pop();
                    pushToVar(varStack, opStack, addVariable(varStack.Pop(), varStack.Pop()));
                }
                else if (opStack.Peek().Equals("-"))
                {
                    opStack.Pop();
                    pushToVar(varStack, opStack, subtractVariable(varStack.Pop(), varStack.Pop()));
                }
                opStack.Push(opr);
            }
            // If opr is a right parenthesis
            else if (opr.Equals(")"))
            {
                // If the value stack contains more than 2 values 
                if (varStack.Count >= 2)
                {
                    if (opStack.Peek().Equals("+"))
                    {
                        opStack.Pop();
                        pushToVar(varStack, opStack, addVariable(varStack.Pop(), varStack.Pop()));
                    }
                    else if (opStack.Peek().Equals("-"))
                    {
                        opStack.Pop();
                        pushToVar(varStack, opStack, subtractVariable(varStack.Pop(), varStack.Pop()));

                    }
                    // 1
                    // After evaluating + and -, the top of the operator next to + and - should be a '('.
                    if (opStack.Count == 0 || !opStack.Peek().Equals("("))
                        throw new ArgumentException("'(' isn't found where expected.");
                    else
                    {
                        opStack.Pop();
                        // Finally if * or / is at the top of the oeprator stack,
                        // calculate it with two popped numbers.
                        if (opStack.Count != 0)
                        {
                            if (opStack.Peek().Equals("/") && varStack.Count >= 1)
                            {
                                opStack.Pop();
                                pushToVar(varStack, opStack, divideVariable(varStack.Pop(), varStack.Pop()));
                            }
                            else if (opStack.Peek().Equals("*") && varStack.Count >= 1)
                            {
                                opStack.Pop();
                                pushToVar(varStack, opStack, multiVariable(varStack.Pop(), varStack.Pop()));
                            }
                        }
                    }
                }
                // If there's no any operator in the operatorStack or There's no '('.
                else if(varStack.Count <= 1)
                {
                    // 1
                    if (opStack.Count == 0 || !opStack.Peek().Equals("("))
                        throw new ArgumentException("'(' isn't found where expected.");
                    else if (opStack.Peek().Equals("("))
                        opStack.Pop();
                    // 2
                    else
                        throw new ArgumentException("You input too many operator.");
                }
            }
            else
                opStack.Push(opr);
        }

        /// <summary>
        /// Add method
        /// </summary>
        /// <param name="num1"> popped number from the variable stack </param>
        /// <param name="num2"> popped number from the variable stack </param>
        /// <returns> result of num1 + num2 </returns>
        public static int addVariable(int num1, int num2)
        {
            return num1 + num2;
        }
        /// <summary>
        /// Subtract method
        /// </summary>
        /// <param name="num1"> popped number from the variable stack </param>
        /// <param name="num2"> popped number from the variable stack </param>
        /// <returns> result of num2 - num1 </returns>
        public static int subtractVariable(int num1, int num2)
        {
            return num2 - num1;
        }
        /// <summary>
        /// Multiply method
        /// </summary>
        /// <param name="num1"> popped number from the variable stack </param>
        /// <param name="num2"> popped number from the variable stack </param>
        /// <returns> result of num1 * num2 </returns>
        public static int multiVariable(int num1, int num2)
        {
            return num1 * num2;
        }

        /// <summary>
        /// Division method
        /// </summary>
        /// <param name="num1"> popped number from the variable stack </param>
        /// <param name="num2"> popped number from the variable stack </param>
        /// <returns> result of num2 / num1 </returns>
        /// <exception cref="ArgumentException">
        /// 1. If it trys to divide number by ZERO.
        /// </exception>
        public static int divideVariable(int num1, int num2)
        {
            int result;
            try
            {
                if (num1 != 0)
                    result = num2 / num1;
                else
                    // 1
                    throw new ArgumentException("Attempted to divide by zero.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            return result;
        }
    }
}