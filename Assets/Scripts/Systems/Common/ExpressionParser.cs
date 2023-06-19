using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Systems.Common
{
    using System;
    using System.Linq.Expressions;

    public static class ExpressionParser
    {
        public static Func<int, float> ParseExpression(string expression)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(int), "x");
            Expression parsedExpression = DynamicExpressionParser.Parse(expression, parameter);
            Expression<Func<int, float>> lambda = Expression.Lambda<Func<int, float>>(parsedExpression, parameter);
            return lambda.Compile();
        }
    }

    public static class DynamicExpressionParser
    {
        public static Expression Parse(string expression, ParameterExpression parameter)
        {
            string[] tokens = expression.Split(' ');
            return ParseTokens(tokens, parameter);
        }

        private static Expression ParseTokens(string[] tokens, ParameterExpression parameter)
        {
            Expression left = ParseTerm(tokens[0], parameter);
            for (int i = 1; i < tokens.Length; i += 2)
            {
                string op = tokens[i];
                Expression right = ParseTerm(tokens[i + 1], parameter);
                left = ParseOperator(left, right, op);
            }
            return left;
        }

        private static Expression ParseTerm(string token, ParameterExpression parameter)
        {
            if (int.TryParse(token, out int constant))
            {
                return Expression.Constant(constant, typeof(int));
            }
            else if (token == "x")
            {
                return parameter;
            }
            else
            {
                throw new ArgumentException($"Invalid token: {token}");
            }
        }

        private static Expression ParseOperator(Expression left, Expression right, string op)
        {
            switch (op)
            {
                case "+":
                    return Expression.Add(left, right);
                case "-":
                    return Expression.Subtract(left, right);
                case "*":
                    return Expression.Multiply(left, right);
                case "/":
                    return Expression.Divide(left, right);
                default:
                    throw new ArgumentException($"Invalid operator: {op}");
            }
        }
    }

}
