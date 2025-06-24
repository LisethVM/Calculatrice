using Calculatrice.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Calculatrice.Services
{
    internal class EvaluerServices
    {
        public const char ADDITION = '+';
        public const char SOUSTRACTION = '-';
        public const char MULTIPLICATION = '*';
        public const char DIVISION = '/';
        public const char PUISSANCE = '^';

        public static double Operation(List<object> tokens)
        {
            var liste = HautePriorite(tokens);
            double valActuelle = 0;
            double valOperation = 0;
            char? operation = null;

            foreach (var item in liste)
            {
                if (char.TryParse(item.ToString(), out char character) && Operateur(character))
                {
                    operation = character;
                }
                else if (valActuelle == 0)
                {
                    valActuelle = Convert.ToDouble(item);
                }

                else if (operation != null)
                {
                    valOperation = Convert.ToDouble(item);

                    valActuelle = Evaluer(valActuelle, valOperation, operation);

                }
            }
            return valActuelle;
        }


        public static double Evaluer(double valActuelle, double valOperation, char? operateur)
        {
            switch (operateur)
            {
                case ADDITION:
                    valActuelle += valOperation;
                    break;
                case SOUSTRACTION:
                    valActuelle -= valOperation;
                    break;
                case MULTIPLICATION:
                    valActuelle = valActuelle * valOperation;
                    break;
                case DIVISION:
                    if (valOperation == 0)
                        throw new Exception("Ce n'est pas possible le division pour 0");
                    valActuelle = valActuelle / valOperation;
                    break;
                case PUISSANCE:
                    valActuelle = Math.Pow(valActuelle, valOperation);
                    break;
            }
            valActuelle = Math.Round(valActuelle, 1);
            return valActuelle;
        }


        public static List<object> HautePriorite(List<object> tokens)
        {
            var solution = new List<object>();
            int i = 0;

            while (i < tokens.Count)
            {
                // Résout les opérations avec priorité 
                if (i + 2 < tokens.Count
                    && tokens[i] is double a
                    && (tokens[i + 1].ToString() == "^" || tokens[i + 1].ToString() == "*" || tokens[i + 1].ToString() == "/")
                    && tokens[i + 2] is double b)
                {

                    double parcial = Evaluer(a, b, char.Parse(tokens[i + 1].ToString()));
                    solution.Add(parcial);
                    i += 3;
                }
                else
                {
                    // si ce n'est pas le haute priorite, il ne garde que le token
                    solution.Add(tokens[i]);
                    i++;
                }
            }
            return solution;
        }


        public static bool Operateur(char operation)
        {
            if ((operation == ADDITION) || (operation == SOUSTRACTION) || (operation == MULTIPLICATION) || (operation == DIVISION) || (operation == PUISSANCE))
            {
                return true;
            }
            else { return false; }
        }

    }
}
