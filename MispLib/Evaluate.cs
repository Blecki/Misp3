using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISPLIB
{
    public class EvaluationError : Exception
    {
        public EvaluationError(String Message) : base(Message) { }
    }

    public static partial class Core
    {
        public static Atom Evaluate(Atom Atom, RecordAtom GlobalScope)
        {
            if (GlobalScope == null) GlobalScope = new RecordAtom();
            var context = new EvaluationContext { ActiveScope = GlobalScope };
            return Atom.Evaluate(context);
        }
    }
}
