using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculatrice.Services
{
    public static class TokeniseurServices
    {
        public static List<string> Tokeniser(string expression)
        {
            var tokens = new List<string>();
            var buffer = new StringBuilder();
            bool attendreNumero = true;

            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression[i];

                if (char.IsWhiteSpace(c))
                    continue;

                // Signe negative '-'
                if (c == '-' && attendreNumero)
                {
                    buffer.Append(c);
                    attendreNumero = true;
                    continue;
                }

                if (char.IsLetter(c))
                {
                    buffer.Clear();
                    while (i < expression.Length && char.IsLetter(expression[i]))
                    {
                        buffer.Append(expression[i]);
                        i++;
                    }
                    // Il prende une parenthèse jusqu'à sa fermeture
                    if (i < expression.Length && expression[i] == '(')
                    {
                        int nivelParentesis = 0;
                        do
                        {
                            buffer.Append(expression[i]);
                            if (expression[i] == '(') nivelParentesis++;
                            else if (expression[i] == ')') nivelParentesis--;
                            i++;
                        } while (i < expression.Length && nivelParentesis > 0);
                    }
                    
                    i--;

                    tokens.Add(buffer.ToString());
                    attendreNumero = false;
                    continue;
                }

                // Evaluation des nombres décimaux ou de plus 2 chiffres 
                if (char.IsDigit(c) || c == ',')
                {
                    if (!(buffer.Length == 1 && buffer[0] == '-'))
                    {
                        buffer.Clear();
                    }
                    while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == ','))
                    {
                        buffer.Append(expression[i]);
                        i++;
                    }
                    i--;
                    tokens.Add(buffer.ToString());
                    attendreNumero = false;
                    continue;
                }

                // Nettogaye du buffer résiduel 
                if (buffer.Length > 0)
                {
                    buffer.Clear();
                }

                if ("+-*/^()".IndexOf(c) >= 0)
                {
                    tokens.Add(c.ToString());
                    attendreNumero = c != ')';
                }
            }

            return tokens;
        }

    }
}
