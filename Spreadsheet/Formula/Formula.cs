// Skeleton written by Profs Zachary, Kopta and Martin for CS 3500
// Read the entire skeleton carefully and completely before you
// do anything else!

// Change log:
// Last updated: 9/16, updated for non-nullable types
/** @author     SangYoon Cho
 *  @date       2022/09/02 (Y/M/D)
 *  @version    1.1 ver
 *                  -> Fill in each methods with copying Evaluator.cs 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        // List for storing tokens
        private List<string> tokens;

        //private HashSet<string> wholeFormula;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// 
        /// Need to be immutable. Do not make any modify methods in this class.
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            // Formula is not nullable.
            if (formula == null || formula.Count() == 0)
                throw new FormulaFormatException("Formula could not be a null or empty.");
            // For checking Balanced Parenthesis Rule
            int rightParenthesis = 0, leftParenthesis = 0;
            string prevToken = "";
            // List to store tokens
            tokens = new List<string>(GetTokens(formula));
            //wholeFormula = new HashSet<string>();

            // Starting/Ending token rule
            if (!tokens.First().Equals("(") && !Double.TryParse(tokens.First(), out double firstDouble) && !IsValidVariable(tokens.First()))
                throw new FormulaFormatException("The first token of an expression must be a number, a variable, or an opening parenthesis.");
            if (!tokens.Last().Equals(")") && !Double.TryParse(tokens.Last(), out double lastDouble) && !IsValidVariable(tokens.Last()))
                throw new FormulaFormatException("The last token of an expression must be a number, a variable, or a closing parenthesis.");

            // Must be included one operation between numbers and variables.
            if (tokens.Count() >= 2 && !(tokens.Contains("+") || tokens.Contains("-") || tokens.Contains("*") || tokens.Contains("/")))
                throw new FormulaFormatException("There must be at least one operator.");

            for (int i = 0; i < tokens.Count; i++)
            {
                string currToken = tokens[i];

                if (currToken.Equals("("))
                    rightParenthesis++;
                else if (currToken.Equals(")"))
                {
                    // Parenthesis/Operator Following Rule
                    if (prevToken.Equals("+") || prevToken.Equals("-") || prevToken.Equals("*") || prevToken.Equals("/"))
                        throw new FormulaFormatException("Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis.");

                    leftParenthesis++;
                }
                else if (currToken.Equals("+") || currToken.Equals("-") || currToken.Equals("*") || currToken.Equals("/"))
                {
                    // Parenthesis/Operator Following Rule
                    if (prevToken.Equals("+") || prevToken.Equals("-") || prevToken.Equals("*") || prevToken.Equals("/") || prevToken.Equals("("))
                        throw new FormulaFormatException("Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis.");
                }
                else if (Double.TryParse(tokens[i], out double value))
                {
                    // Parenthesis Following Rule
                    if (prevToken.Equals(")"))
                        throw new FormulaFormatException("Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis.");

                    tokens[i] = value.ToString();
                }
                // Check if string is valid variable.
                else if (IsValidVariable(currToken))
                {
                    // If variable can convert to double, it means that it is not variable.
                    if (Double.TryParse(prevToken, out double prevValue))
                        throw new FormulaFormatException("Variable is invalid.");
                    // Normalize and replace original one with this.
                    if (isValid(normalize(currToken)))
                        tokens[i] = normalize(currToken);
                    else
                        throw new FormulaFormatException("Variable is invalid.");

                }
                else
                    throw new FormulaFormatException("Don't use syntactically incorrect string.");
                // Store previous token for checking rules.
                prevToken = currToken;
            }

            if (rightParenthesis != leftParenthesis)
                throw new FormulaFormatException("The total number of opening parentheses must equal the total number of closing parentheses.");
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            // Store integer
            Stack<double> valueStack = new Stack<double>();
            // Store operator(tokens)
            Stack<string> operatorStack = new Stack<string>();
            double result = 0, checkZero = 0;

            foreach (string s in tokens)
            {
                // Check bool if s can change to the integer, and if true, store into the valueStack.
                if (Double.TryParse(s, out double value))
                {
                    //pushToVar(valueStack, operatorStack, value);
                    valueStack.Push(value);
                    // Check if there's an operator in the operatorStack
                    // which means check if there's expression that didn't calculate yet.
                    // This is only for multiply or division.
                    if (operatorStack.Count() != 0)
                    {
                        if (operatorStack.Peek().Equals("/") && valueStack.Count >= 1)
                        {
                            operatorStack.Pop();
                            checkZero = valueStack.Peek();
                            if (checkZero == 0)
                                return new FormulaError("Divided by 0(Zero).");
                            else
                                valueStack.Push(calculateVariable("/", valueStack.Pop(), valueStack.Pop()));
                        }
                        else if (operatorStack.Peek().Equals("*") && valueStack.Count >= 1)
                        {
                            operatorStack.Pop();
                            valueStack.Push(calculateVariable("*", valueStack.Pop(), valueStack.Pop()));
                        }
                    }
                }
                // If string is operator
                else if (s.Equals("(") || s.Equals(")") || s.Equals("+") || s.Equals("-") || s.Equals("/") || s.Equals("*"))
                {
                    if ((s.Equals("+") || s.Equals("-")) && valueStack.Count >= 2)
                    {
                        // If + or - is at the top of the operator stack,
                        // pop the value stack twice and the operator stack once,
                        // then apply the popped operator to the popped numbers,
                        // then push the result onto the value stack.
                        if (operatorStack.Peek().Equals("+"))
                        {
                            operatorStack.Pop();
                            valueStack.Push(calculateVariable("+", valueStack.Pop(), valueStack.Pop()));
                        }
                        else if (operatorStack.Peek().Equals("-"))
                        {
                            operatorStack.Pop();
                            valueStack.Push(calculateVariable("-", valueStack.Pop(), valueStack.Pop()));
                            
                        }
                        operatorStack.Push(s);
                    }
                    // If opr is a right parenthesis
                    else if (s.Equals(")"))
                    {
                        // If the value stack contains more than 2 values 
                        if (valueStack.Count >= 2)
                        {
                            if (operatorStack.Peek().Equals("+"))
                            {
                                operatorStack.Pop();
                                valueStack.Push(calculateVariable("+", valueStack.Pop(), valueStack.Pop()));
                            }
                            else if (operatorStack.Peek().Equals("-"))
                            {
                                operatorStack.Pop();
                                valueStack.Push(calculateVariable("-", valueStack.Pop(), valueStack.Pop()));
                            }
                            operatorStack.Pop();
                            // Finally if * or / is at the top of the oeprator stack,
                            // calculate it with two popped numbers.
                            if (operatorStack.Count != 0)
                            {
                                if (operatorStack.Peek().Equals("/") && valueStack.Count >= 1)
                                {
                                    operatorStack.Pop();
                                    checkZero = valueStack.Peek();
                                    if (checkZero == 0)
                                        return new FormulaError("Divided by 0(Zero).");
                                    else
                                        valueStack.Push(calculateVariable("/", valueStack.Pop(), valueStack.Pop()));
                                }
                                else if (operatorStack.Peek().Equals("*") && valueStack.Count >= 1)
                                {
                                    operatorStack.Pop();
                                    valueStack.Push(calculateVariable("*", valueStack.Pop(), valueStack.Pop()));  
                                }
                            }
                        }
                    }
                    else
                        operatorStack.Push(s);
                }
                else
                {
                    // Catch the name error.
                    try
                    {
                        // Catch the error from here.
                        valueStack.Push(lookup(s));

                        if (operatorStack.Count() != 0)
                        {
                            if (operatorStack.Peek().Equals("/") && valueStack.Count >= 1)
                            {
                                operatorStack.Pop();
                                checkZero = valueStack.Peek();
                                if (checkZero == 0)
                                    return new FormulaError("Divided by 0(Zero).");
                                else
                                    valueStack.Push(calculateVariable("/", valueStack.Pop(), valueStack.Pop()));
                            }
                            else if (operatorStack.Peek().Equals("*") && valueStack.Count >= 1)
                            {
                                operatorStack.Pop();
                                valueStack.Push(calculateVariable("*", valueStack.Pop(), valueStack.Pop()));
                            }
                        }
                    }
                    catch
                    {
                        return new FormulaError("Name is undefined.");
                    }
                }
            }
            // Check the operatorStack if there's still remain operator(token).
            // Only + and - are accepted.
            if (operatorStack.Count() != 0 && valueStack.Count >= 2)
            {
                if (operatorStack.Peek().Equals("+"))
                    result = calculateVariable("+", valueStack.Pop(), valueStack.Pop());
                else if (operatorStack.Peek().Equals("-"))
                    result = calculateVariable("-", valueStack.Pop(), valueStack.Pop());
            }
            else
                result = valueStack.Pop();

            return result;
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            HashSet<string> listVariable = new HashSet<string>();
            String varPattern = @"[a-zA-Z](?: [a-zA-Z]|\d)*";

            foreach (string s in tokens)
            {
                if (Regex.IsMatch(s, varPattern, RegexOptions.Singleline))
                    listVariable.Add(s);
            }
            return listVariable;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            string result = string.Join("", tokens);
            return result;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (!(obj is Formula))
                return false;

            Formula objFor = (Formula)obj;

            int num1 = objFor.tokens.Count;
            int num2 = this.tokens.Count;

            if(num1 > num2 || num2 < num1)
                return false;


            for (int i = 0; i < this.tokens.Count; i++)
            {
                if (!Double.TryParse(this.tokens[i], out double isThisDouble))
                {
                    if (!(this.tokens[i].Equals(objFor.tokens[i])))
                        return false;
                }
                // If tokens[i] can be a double
                else if (Double.Parse(this.tokens[i]) != Double.Parse(objFor.tokens[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that f1 and f2 cannot be null, because their types are non-nullable
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that f1 and f2 cannot be null, because their types are non-nullable
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            int hashFromFormula = this.ToString().GetHashCode();
            int len = tokens.Count;
            return len * hashFromFormula;
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                    yield return s;
            }
        }

        /// <summary>
        /// Return true if variable is a valid variable, other false.
        /// </summary>
        private static bool IsValidVariable(String variable)
        {
            String varPattern = @"[a-zA-Z_]+[0-9]*";

            if (Regex.IsMatch(variable, varPattern, RegexOptions.Singleline))
                return true;

            return false;
        }

        /// <summary>
        /// Calculator method. This method takes the operator and calculate for each operator.
        /// When operator is 'divide', 0 cannot be assigned in the num1.
        /// </summary>
        /// <param name="num1"> popped number from the variable stack </param>
        /// <param name="num2"> popped number from the variable stack </param>
        /// <returns> result of num1 (operator) num2 </returns>
        private static double calculateVariable(string opr, double num1, double num2)
        {
            double result = 0;

            if (opr.Equals("+"))
                result = num1 + num2;
            else if (opr.Equals("-"))
                result = num2 - num1;
            else if (opr.Equals("*"))
                result = num1 * num2;
            else
            {
                if (num1 != 0)
                    result = num2 / num1;
            }
            return result;
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}

