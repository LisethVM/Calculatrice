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

                // Función o identificador: comienza con letra
                if (char.IsLetter(c))
                {
                    // Acumular nombre de función
                    buffer.Clear();
                    while (i < expression.Length && char.IsLetter(expression[i]))
                    {
                        buffer.Append(expression[i]);
                        i++;
                    }
                    // Si viene un paréntesis, tomamos hasta cerrar
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
                    // Decrementar i para no saltarnos el próximo carácter procesable
                    i--;

                    tokens.Add(buffer.ToString());
                    attendreNumero = false;
                    continue;
                }

                // Dígito o punto decimal: parte de número
                if (char.IsDigit(c) || c == '.')
                {
                    // Si buffer no contiene sólo el signo '-', limpiarlo
                    if (!(buffer.Length == 1 && buffer[0] == '-'))
                    {
                        buffer.Clear();
                    }
                    while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                    {
                        buffer.Append(expression[i]);
                        i++;
                    }
                    i--;
                    tokens.Add(buffer.ToString());
                    attendreNumero = false;
                    continue;
                }

                // Si hay un buffer residual, limpiarlo (seguridad)
                if (buffer.Length > 0)
                {
                    buffer.Clear();
                }

                // Operadores y paréntesis como tokens individuales
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
