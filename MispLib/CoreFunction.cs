using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISPLIB
{
    public class CoreFunction
    {
        public String Name;
        public List<TokenAtom> ArgumentNames;
        public Func<List<Atom>, EvaluationContext, Atom> Implementation;
    }
}
