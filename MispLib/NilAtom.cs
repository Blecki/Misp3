using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISPLIB
{
    public class NilAtom : Atom
    {
        public override AtomType Type { get { return AtomType.Nil; }}
        protected override void ImplementEmit(StringBuilder Into) { Into.Append("nil"); }
        public override object GetSystemValue() { return null; }

        public override Atom Evaluate(EvaluationContext Context)
        {
            return this;
        }
    }
}
