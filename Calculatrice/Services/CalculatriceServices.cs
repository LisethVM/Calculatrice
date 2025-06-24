using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Calculatrice.Model;
using Calculatrice.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Calculatrice.Services
{
    internal class CalculatriceServices
    {
        public ExpressionComplexeModel Separer(string expression)
        {
            expression = expression.Trim();
            expression = expression.Replace(".", ",");
            
            ExpressionComplexeModel expressionComplexe = new ExpressionComplexeModel(expression, 0, expression.Length - 1, []);
            expressionComplexe.SousOperations = SeparerSousOperations(expressionComplexe);
            EvaluerSousOperations(expressionComplexe);
            
            return expressionComplexe;
        }
        private List<ExpressionComplexeModel> SeparerSousOperations(ExpressionComplexeModel expression)
        {
            var sousOperation = ExtraireExpressionComplexe(expression);
            foreach (var item in sousOperation.Where(x => x.Expression.Contains('(')))
            {
                item.SousOperations = SeparerSousOperations(item);
            }
            return sousOperation;
        }

        private List<ExpressionComplexeModel> ExtraireExpressionComplexe(ExpressionComplexeModel expression)
        {
            var expressionComplexes = new List<ExpressionComplexeModel>();
            string s = expression.Expression;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != '(')
                    continue;

                // Il fait l'evaluation s'il y a une lettre avant du ( pour definir la racine carrée
                if (i > 0 && char.IsLetter(s[i - 1]))
                    continue; // Il rest d'optimiser la différenciation des lettres

                // Il trouve la parenthèse final d'expression principale
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

                // Il n'extrait que le contenu interne de la parenthèse 
                string sub = s.Substring(i + 1, fin - i - 1);
                var sousOperation = new ExpressionComplexeModel(sub, i + 1, fin - 1, new List<ExpressionComplexeModel>())
                {
                    OperationPrincipale = expression
                };
                expressionComplexes.Add(sousOperation);

                i = fin;  // avanzamos el índice para no re-pasar este bloque
            }

            return expressionComplexes;
        }


        public void EvaluerSousOperations(ExpressionComplexeModel noeud)
        {
            // Première route de tous les sous-operations
            foreach (var sousOperation in noeud.SousOperations)
            {
                EvaluerSousOperations(sousOperation);
            }
            // Il fait l'evaluation du chaque sous-operation
            Console.WriteLine("");

            var tokens = TokeniseurServices.Tokeniser(noeud.Expression);
            var valeurs = new List<object>();
            // fait le trajet du tokens
            foreach (var tok in tokens)
            {
                if (tok == "(" || tok == ")")
                    continue;
                if (tok == "+" || tok == "-" || tok == "*" || tok == "/" || tok == "^")
                {
                    valeurs.Add(tok);
                }
                else
                {
                    // s'il y a un operation de la racine carrée
                    if (tok.Contains("sqrt"))
                    {
                        string racineCarree = tok.Replace("sqrt(", "").TrimEnd(')');
                        var miniEx = new ExpressionComplexeModel(racineCarree, 5, racineCarree.Length - 2, []);
                        miniEx.SousOperations = SeparerSousOperations(miniEx);
                        EvaluerSousOperations(miniEx);
                        valeurs.Add(Math.Sqrt(double.Parse(miniEx.Expression)));
                    }
                    else
                        valeurs.Add(double.Parse(tok));
                }
            }

            try
            {
                var resultado = EvaluerServices.Operation(valeurs);
                // Optimizer pour les expressions repetitives ((2+3)+1+(2+3))
                if (noeud.OperationPrincipale != null)
                    noeud.OperationPrincipale.Expression = noeud.OperationPrincipale.Expression.Replace(noeud.BaseExpression, resultado.ToString());
                else
                    noeud.Expression = resultado.ToString();
            }
            catch (MathException)
            {
                throw;
            }
        }
        

    }


}
