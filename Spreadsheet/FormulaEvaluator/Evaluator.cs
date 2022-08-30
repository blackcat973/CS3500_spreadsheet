using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace FormulaEvaluator
{
    public static class Evaluator
    {
        public delegate int Lookup(string v);
        public static int Evaluate(string exp, Lookup variableEvaluator)
        {
            Stack<int> valueStack = new Stack<int>();
            Stack<string> operatorStack = new Stack<string>();
            int result = 0;

            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            foreach (string s in substrings)
            {
                if (int.TryParse(s, out int value))
                    pushToVar(valueStack, operatorStack, value);
                else if (s.Equals("(") || s.Equals(")") || s.Equals("+") || s.Equals("-") || s.Equals("/") || s.Equals("*"))
                    pushToOp(valueStack, operatorStack, s);
                else if (s.Equals("") || s.Equals(" "))
                    continue;
                else
                {
                    try
                    {
                        bool checkString = s.Any(char.IsDigit);
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

            if (valueStack.Count == 0 && operatorStack.Count == 0)
                throw new ArgumentException("Cannot use empty string.");
            else if (valueStack.Count == 0 && operatorStack.Count >= 1)
                throw new ArgumentException("There is no variable.");
            else if (valueStack.Count == 1 && operatorStack.Count >= 1)
                throw new ArgumentException("Input too many operators.");
            else if (valueStack.Count >= 2 + operatorStack.Count)
                throw new ArgumentException("Operator is not enough.");

            if (operatorStack.Count() != 0 && valueStack.Count == 2)
            {
                if (operatorStack.Peek().Equals("+"))
                    result = addVariable(valueStack.Pop(), valueStack.Pop());
                else if (operatorStack.Peek().Equals("-"))
                {
                    result = subtractVariable(valueStack.Pop(), valueStack.Pop());
                }
                //else
                //    result = valueStack.Pop();
            }
            else
                result = valueStack.Pop();

            return result;
        }

        public static void pushToVar(Stack<int> varStack, Stack<string> opStack, int num)
        {
            varStack.Push(num);
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

        public static void pushToOp(Stack<int> varStack, Stack<string> opStack, string opr)
        {
            if ((opr.Equals("+") || opr.Equals("-")) && varStack.Count >= 2)
            {
                if (opStack.Peek().Equals("+"))
                {
                    //varStack.Push(addVariable(varStack.Pop(), varStack.Pop()));
                    opStack.Pop();
                    pushToVar(varStack, opStack, addVariable(varStack.Pop(), varStack.Pop()));
                }
                else if (opStack.Peek().Equals("-"))
                {
                    //varStack.Push(subtractVariable(varStack.Pop(), varStack.Pop()));
                    opStack.Pop();
                    pushToVar(varStack, opStack, subtractVariable(varStack.Pop(), varStack.Pop()));
                }
                opStack.Push(opr);
            }
            else if (opr.Equals(")"))
            {
                if (varStack.Count >= 2)
                {
                    if (opStack.Peek().Equals("+"))
                    {
                        //varStack.Push(addVariable(varStack.Pop(), varStack.Pop()));
                        opStack.Pop();
                        pushToVar(varStack, opStack, addVariable(varStack.Pop(), varStack.Pop()));
                    }
                    else if (opStack.Peek().Equals("-"))
                    {
                        //varStack.Push(subtractVariable(varStack.Pop(), varStack.Pop()));
                        opStack.Pop();
                        pushToVar(varStack, opStack, subtractVariable(varStack.Pop(), varStack.Pop()));

                    }

                    if (opStack.Count == 0 || !opStack.Peek().Equals("("))
                        throw new ArgumentException("'(' isn't found where expected.");
                    else
                    {
                        opStack.Pop();
                        if (opStack.Count != 0)
                        {
                            if (opStack.Peek().Equals("/") && varStack.Count >= 1)
                            {
                                //varStack.Push(divideVariable(varStack.Pop(), varStack.Pop()));
                                opStack.Pop();
                                pushToVar(varStack, opStack, divideVariable(varStack.Pop(), varStack.Pop()));
                            }
                            else if (opStack.Peek().Equals("*") && varStack.Count >= 1)
                            {
                                //varStack.Push(multiVariable(varStack.Pop(), varStack.Pop()));
                                opStack.Pop();
                                pushToVar(varStack, opStack, multiVariable(varStack.Pop(), varStack.Pop()));
                            }
                        }
                    }
                }
                else if (varStack.Count <= 1)
                {
                    if (opStack.Count == 0 || !opStack.Peek().Equals("("))
                        throw new ArgumentException("'(' isn't found where expected.");
                    else if (opStack.Peek().Equals("("))
                        opStack.Pop();
                    else
                        throw new ArgumentException("You input too many operator.");
                }
            }
            else
                opStack.Push(opr);
        }

        public static int addVariable(int num1, int num2)
        {
            return num1 + num2;
        }

        public static int subtractVariable(int num1, int num2)
        {
            return num2 - num1;
        }

        public static int multiVariable(int num1, int num2)
        {
            return num1 * num2;
        }

        public static int divideVariable(int num1, int num2)
        {
            int result;
            try
            {
                if (num1 != 0)
                    result = num2 / num1;
                else
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