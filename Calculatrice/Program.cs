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
            Console.WriteLine("Calculatrice pro 1.0");
            
            string expression = "-5+((sqrt(5^2)*3)+(10/5))"; //"-5+((sqrt(5^2)*3)+(10/5))"; (15)+(2) -- -5+(15)

            CalculatriceServices cals2 = new CalculatriceServices();

            var expresionCompleja = cals2.Separer(expression);
            Console.WriteLine($"Resultado: {expresionCompleja.BaseExpression} = {expresionCompleja.Expression}");

        }

    }


}

