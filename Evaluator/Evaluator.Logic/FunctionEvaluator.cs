﻿using System.Globalization;

namespace Evaluator.Logic;

public class FunctionEvaluator
{
    public static double Evalute(string infix)
    {
        var postfix = ToPostfix(infix);
        return Calculate(postfix);
    }

    private static double Calculate(List<string> postfix)
    {
        var stack = new Stack<double>();
        foreach (var item in postfix)
        {
            if (IsOperator(item[0]) && item.Length == 1)
            {
                var operator2 = stack.Pop();
                var operator1 = stack.Pop();
                stack.Push(Result(operator1, item[0], operator2));
            }
            else
            {
                stack.Push(double.Parse(item, CultureInfo.InvariantCulture));
            }
        }
        return stack.Pop();
    }

    private static double Result(double operator1, char item, double operator2)
    {
        return item switch
        {
            '+' => operator1 + operator2,
            '-' => operator1 - operator2,
            '*' => operator1 * operator2,
            '/' => operator1 / operator2,
            '^' => Math.Pow(operator1, operator2),
            _ => throw new Exception("Invalid expresion"),
        };
    }

    private static List<string> ToPostfix(string infix)
    {
        var stack = new Stack<char>();
        var postfix = new List<string>();
        var number = string.Empty;

        foreach (var item in infix)
        {
            if (char.IsDigit(item) || item == '.')
            {
                number += item;
            }
            else if (IsOperator(item))
            {
                if (!string.IsNullOrEmpty(number))
                {
                    postfix.Add(number);
                    number = string.Empty;
                }

                if (item == ')')
                {
                    while (stack.Peek() != '(')
                    {
                        postfix.Add(stack.Pop().ToString());
                    }
                    stack.Pop();
                }
                else
                {
                    while (stack.Count > 0 && PriorityExpression(item) <= PriorityStack(stack.Peek()))
                    {
                        postfix.Add(stack.Pop().ToString());
                    }
                    stack.Push(item);
                }
            }
            else if (item == '(')
            {
                if (!string.IsNullOrEmpty(number))
                {
                    postfix.Add(number);
                    number = string.Empty;
                }
                stack.Push(item);
            }
            else if (char.IsWhiteSpace(item))
            {
                if (!string.IsNullOrEmpty(number))
                {
                    postfix.Add(number);
                    number = string.Empty;
                }
            }
        }

        if (!string.IsNullOrEmpty(number))
        {
            postfix.Add(number);
        }

        while (stack.Count > 0)
        {
            postfix.Add(stack.Pop().ToString());
        }

        return postfix;
    }

    private static int PriorityStack(char item)
    {
        return item switch
        {
            '^' => 3,
            '*' => 2,
            '/' => 2,
            '+' => 1,
            '-' => 1,
            '(' => 0,
            _ => throw new Exception("Invalid expression."),
        };
    }

    private static int PriorityExpression(char item)
    {
        return item switch
        {
            '^' => 4,
            '*' => 2,
            '/' => 2,
            '+' => 1,
            '-' => 1,
            '(' => 5,
            _ => throw new Exception("Invalid expression."),
        };
    }

    private static bool IsOperator(char item) => "()^*/+-".IndexOf(item) >= 0;
}