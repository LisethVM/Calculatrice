using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculatrice.Model
{

    public class ExpressionComplexeModel(string expression, int IndexDebut, int IndexFin, List<ExpressionComplexeModel> SousOperations)
    {
        public string Expression { get; set; } = expression;
        public string BaseExpression { get; private set; } = expression;
        public int IndexDebut { get; set; } = IndexDebut;
        public int IndexFin { get; set; } = IndexFin;
        public ExpressionComplexeModel? OperationPrincipale { get; set; } = null;
        public List<ExpressionComplexeModel> SousOperations { get; set; } = SousOperations;
    }

}
