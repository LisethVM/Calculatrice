using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Calculatrice.Services
{
    internal class Calculadora2
    {
        public ExpresionCompleja Separar(string expresion)
        {
            expresion = expresion.Trim();
            expresion = expresion.Replace(".", ",");
            
            ExpresionCompleja expresionCompleja = new ExpresionCompleja(expresion, 0, expresion.Length - 1, []);
            //expresionComplejas = ExtraerExpresionesComplejas(expresion);
            expresionCompleja.Hijos = SepararInternos(expresionCompleja);
            CalcularSubOperaciones(expresionCompleja);
            var a = 1;
            return expresionCompleja;
        }

        private List<ExpresionCompleja> ExtraerExpresionesComplejas(ExpresionCompleja expresion)
        {
            var expresionComplejas = new List<ExpresionCompleja>();
            string s = expresion.Expresion;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != '(')
                    continue;

                // --- NUEVO: si el paréntesis viene justo después de una letra,
                //           es parte de una función (ej. "sqrt(25)"), así que lo saltamos:
                if (i > 0 && char.IsLetter(s[i - 1]))
                    continue; // no diferencia la letra que hay antes

                // resto de tu lógica para encontrar fin del paréntesis balanceado:
                int cont = 0, fin = i;
                for (int j = i; j < s.Length; j++)
                {
                    if (s[j] == '(') cont++;
                    if (s[j] == ')') cont--;
                    if (cont == 0)
                    {
                        fin = j;
                        break;
                    }
                }

                // extraemos solo el contenido interior de paréntesis “puro”
                string sub = s.Substring(i + 1, fin - i - 1);
                var hijo = new ExpresionCompleja(sub, i + 1, fin - 1, new List<ExpresionCompleja>())
                {
                    Padre = expresion
                };
                expresionComplejas.Add(hijo);

                i = fin;  // avanzamos el índice para no re-pasar este bloque
            }

            return expresionComplejas;
        }

        private List<ExpresionCompleja> SepararInternos(ExpresionCompleja expresion)
        {
            var hijos = ExtraerExpresionesComplejas(expresion);
            foreach (var item in hijos.Where(x => x.Expresion.Contains('(')))
            {
                item.Hijos = SepararInternos(item);
            }
            return hijos;
        }

        public void CalcularSubOperaciones(ExpresionCompleja nodo)
        {
            // Primero recorre todos los hijos
            foreach (var hijo in nodo.Hijos)
            {
                CalcularSubOperaciones(hijo);
            }
            // Luego procesa el nodo actual
            Console.WriteLine(nodo.Expresion);

            var tokens = Tokenizador.Tokenizar(nodo.Expresion);
            var valores = new List<object>();
            foreach (var tok in tokens)
            {
                if (tok == "(" || tok == ")")
                    continue;
                if (tok == "+" || tok == "-" || tok == "*" || tok == "/" || tok == "^"
                    )
                {
                    valores.Add(tok);
                }
                else
                {
                    if (tok.Contains("sqrt"))
                    {
                        string expresionRaiz = tok.Replace("sqrt(", "").TrimEnd(')');
                        var miniEx = new ExpresionCompleja(expresionRaiz, 5, expresionRaiz.Length - 2, []);
                        miniEx.Hijos = SepararInternos(miniEx);
                        CalcularSubOperaciones(miniEx);
                        valores.Add(Math.Sqrt(double.Parse(miniEx.Expresion)));
                    }
                    else
                        // Parse seguro: se asume que tok es un número válido
                        valores.Add(double.Parse(tok));
                }
            }


            //var resultado = Program.Operacion(nodo.Expresion);
            try
            {
                var resultado = Program.Operacion2(valores);
                if (nodo.Padre != null)
                    nodo.Padre.Expresion = nodo.Padre.Expresion.Replace(nodo.BaseExpresion, resultado.ToString());
                else
                    nodo.Expresion = resultado.ToString();
            }
            catch (MathException)
            {
                throw;
            }
        }
        public static string ReemplazarSubcadena(
            ExpresionCompleja expresionCompleja,
            int indiceInicial,
            int indiceFinal,
            string textoReemplazo)
        {
            // Validaciones básicas
            if (expresionCompleja.BaseExpresion == null)
                throw new ArgumentNullException(nameof(expresionCompleja));
            if (indiceInicial < 0 || indiceFinal < indiceInicial || indiceFinal >= expresionCompleja.BaseExpresion.Length)
                throw new ArgumentOutOfRangeException("Índices fuera de rango o desordenados.");

            // 1) Quitar la subcadena [indiceInicial … indiceFinal]
            // 2) Insertar textoReemplazo en el mismo índiceInicial
            return expresionCompleja.BaseExpresion
                .Remove(indiceInicial, indiceFinal - indiceInicial + 1)
                .Insert(indiceInicial, textoReemplazo);
        }

    }

    
    public class ExpresionCompleja(string expresion, int IndexInicio, int IndexFinal, List<ExpresionCompleja> Hijos)
    {
        public string Expresion { get; set; } = expresion;
        public string BaseExpresion { get; private set; } = expresion;
        public int IndexInicio { get; set; } = IndexInicio;
        public int IndexFinal { get; set; } = IndexFinal;
        public ExpresionCompleja? Padre { get; set; } = null;
        public List<ExpresionCompleja> Hijos { get; set; } = Hijos;
    }

    public static class Tokenizador
    {
        /// <summary>
        /// Separa una expresión en números y operadores.
        /// </summary>
        /// <param name="expresion">
        /// Cadena de entrada, por ejemplo "58+7.5-2*79/81".
        /// </param>
        /// <returns>
        /// Lista de tokens: ["58", "+", "7.5", "-", "2", "*", "79", "/", "81"]
        /// </returns>
        /// 
        public static List<string> Tokenizar(string expresion)
        {
            var tokens = new List<string>();
            var buffer = new StringBuilder();

            // Control de contexto para distinguir signo unario
            bool esperandoNumero = true;

            for (int i = 0; i < expresion.Length; i++)
            {
                char c = expresion[i];

                if (char.IsWhiteSpace(c))
                    continue;

                // Signo unario '-'
                if (c == '-' && esperandoNumero)
                {
                    buffer.Append(c);
                    esperandoNumero = true;
                    continue;
                }

                // Función o identificador: comienza con letra
                if (char.IsLetter(c))
                {
                    // Acumular nombre de función
                    buffer.Clear();
                    while (i < expresion.Length && char.IsLetter(expresion[i]))
                    {
                        buffer.Append(expresion[i]);
                        i++;
                    }
                    // Si viene un paréntesis, tomamos hasta cerrar
                    if (i < expresion.Length && expresion[i] == '(')
                    {
                        int nivelParentesis = 0;
                        do
                        {
                            buffer.Append(expresion[i]);
                            if (expresion[i] == '(') nivelParentesis++;
                            else if (expresion[i] == ')') nivelParentesis--;
                            i++;
                        } while (i < expresion.Length && nivelParentesis > 0);
                    }
                    // Decrementar i para no saltarnos el próximo carácter procesable
                    i--;

                    tokens.Add(buffer.ToString());
                    esperandoNumero = false;
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
                    while (i < expresion.Length && (char.IsDigit(expresion[i]) || expresion[i] == '.'))
                    {
                        buffer.Append(expresion[i]);
                        i++;
                    }
                    i--;
                    tokens.Add(buffer.ToString());
                    esperandoNumero = false;
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
                    esperandoNumero = c != ')';
                }
            }

            return tokens;
        }

    }

    class MathException(string mensaje) : Exception(mensaje)
    {
    }

}
