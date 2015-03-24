using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISPLIB
{
    public class IntegerAtom : Atom
    {
        public int Value;
        public override AtomType Type { get { return AtomType.Integer; } }
        protected override void ImplementEmit(StringBuilder Into) { Into.Append(Value); }
        public override Atom Evaluate(EvaluationContext Context) { return this; }
        public override object GetSystemValue() { return Value; }

        public IntegerAtom() { }
        public IntegerAtom(int Value) { this.Value = Value; }
    }
}
