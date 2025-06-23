using Calculatrice.Interface;
using Calculatrice.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Calculatrice
{
    class Program
    {

        public const char SUMA = '+';
        public const char RESTA = '-';
        public const char MULTI = '*';
        public const char DIVISION = '/';
        public const char POTENCIA = '^';

        static void Main()
        {
            Console.WriteLine("Calculatrice pro 1.0");
            //string expression = "-5+((sqrt(5^2)*3)+(10/5))";
            string expression = "2/0";

            Calculadora2 cals2 = new Calculadora2();

            var expresionCompleja = cals2.Separar(expression);
            Console.WriteLine($"Resultado: {expresionCompleja.BaseExpresion} = {expresionCompleja.Expresion}");
            

        }


        public static double Evaluar(double valActual, double valOperation, char? operation)
        {
            switch (operation)
            {
                case SUMA:
                    valActual += valOperation;
                    break;
                case RESTA:
                    valActual -= valOperation;
                    break;
                case MULTI:
                    valActual = valActual * valOperation;
                    break;
                case DIVISION:
                    if (valOperation == 0)
                        throw new MathException("Division por cero");
                    valActual = valActual / valOperation;
                    break;
                case POTENCIA: valActual = Math.Pow(valActual,valOperation);
                    break;
            }
            return valActual;
        }


        public static double Operacion2(List<object> tokens)
        {
            var lista2 = AplicarPrioridadAlta(tokens);
            double valActual = 0;
            double valOperation = 0;
            char? operation = null;

            foreach(var item in lista2)
            {
                if (char.TryParse(item.ToString(),out char character) && EsOperation(character))
                {
                    operation = character;
                }
                else if (valActual == 0)
                {
                    valActual = Convert.ToDouble(item);
                }
                
                else if (operation != null)
                {
                    valOperation = Convert.ToDouble(item);

                    valActual = Evaluar(valActual, valOperation, operation);
                }
            }
            return valActual;
        }


        public static List<object> AplicarPrioridadAlta(List<object> tokens)
        {
            var salida = new List<object>();
            int i = 0;
            while (i < tokens.Count)
            {
                // Si encontramos un número (o función-resuelto) y
                // justo después un operador de alta prioridad, lo resolvemos:
                var i1 = tokens[i];
                var i2 = tokens[i + 1];
                var i3 = tokens[i + 2];
                if (i + 2 < tokens.Count
                    && i1 is double a
                    && (i2.ToString() == "^" || i2.ToString() == "*" || i2.ToString() == "/")
                    && i3 is double b)
                {
                    // Evaluamos a op b
                    double parcial = Evaluar(a, b, char.Parse(i2.ToString()));
                    // Metemos el resultado en lugar de [a,op,b]
                    salida.Add(parcial);
                    // Saltamos esos tres
                    i += 3;
                }
                else
                {
                    // Si no es un caso de prioridad alta, copiamos el token
                    salida.Add(tokens[i]);
                    i++;
                }
            }
            return salida;
        }


        public static bool EsOperation(char operation)
        {
            if ((operation == SUMA) || (operation == RESTA) || (operation == MULTI) || (operation == DIVISION) || (operation == POTENCIA))
            {
                return true;
            }
            else { return false; }
        }

    }


}

