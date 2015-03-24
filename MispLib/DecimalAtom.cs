using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISPLIB
{
    public class DecimalAtom : Atom
    {
        public float Value;
        public override AtomType Type { get { return AtomType.Decimal; } }
        protected override void ImplementEmit(StringBuilder Into) { Into.Append(Value); }
        public override Atom Evaluate(EvaluationContext Context) { return this; }
        public override object GetSystemValue() { return Value; }

        public DecimalAtom() { }
        public DecimalAtom(float Value) { this.Value = Value; }
    }
}
