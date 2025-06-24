using Calculatrice.Interface;
using Calculatrice.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Calculatrice.Exceptions;

namespace Calculatrice
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Calculatrice EZO");

            //string expression = "2.8*3-1"; 
            string expression = FonctionConsole();

            while (expression != "" && expression.ToLower() != "Quitter")
            {
                try
                {
                    CalculatriceServices cals2 = new CalculatriceServices();

                    var expresionCompleja = cals2.Separer(expression);
                    Console.WriteLine($"Resultado: {expresionCompleja.BaseExpression} = {expresionCompleja.Expression}");

                    expression = FonctionConsole();
                }
                catch (Exception)
                {
                    Console.WriteLine("Ce n'est pas possible le division pour 0 main");
                    expression = FonctionConsole();
                }
            }
        }

        private static string FonctionConsole()
        {
            Console.WriteLine("Ajouter une fonction: ");
            return Console.ReadLine();
        }

    }


}

