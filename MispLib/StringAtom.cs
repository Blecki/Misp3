using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISPLIB
{
    public class StringAtom : Atom
    {
        public String Value;
        public override AtomType Type { get { return AtomType.String; } }
        protected override void ImplementEmit(StringBuilder Into) { Into.Append("\"" + Value.Replace("\\", "\\\\") + "\""); }
        public override Atom Evaluate(EvaluationContext Context) { return this; }
        public override object GetSystemValue() { return Value; }
    }
}
