using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISPLIB
{
    public class TokenAtom : Atom
    {
        public String Value;
        public override AtomType Type { get { return AtomType.Token; }}
        protected override void ImplementEmit(StringBuilder Into) { Into.Append(Value); }
        public override object GetSystemValue() { return Value; }

        public override Atom Evaluate(EvaluationContext Context)
        {
            if (Modifier == MISPLIB.Modifier.Quote) return new TokenAtom { Value = Value, Modifier = MISPLIB.Modifier.None };

            Atom result = null;
            if (!Context.ActiveScope.TryGetValue(Value, out result))
                throw new EvaluationError("Could not find token in active scope.");

            return result;
        }
    }
}
