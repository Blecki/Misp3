using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISPLIB
{
    public class EvaluationContext
    {
        public static Dictionary<String, Func<List<Atom>, Atom>> BuiltInFunctions;

        public RecordAtom ActiveScope;
    }
}
