﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculatrice.Exceptions
{
    class MathException(string mensaje) : Exception(mensaje)
    {
    }
}
