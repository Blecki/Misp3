using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISPLIB
{
    public class FunctionAtom : Atom
    {
        public List<TokenAtom> ArgumentNames;
        public Atom Implementation;
        public RecordAtom DeclarationScope;

        public override AtomType Type { get { return AtomType.Function; } }
        protected override void ImplementEmit(StringBuilder Into) 
        {
            Into.Append("(func (");
            foreach (var name in ArgumentNames)
            {
                name.Emit(Into);
                Into.Append(" ");
            }
            Into.Append(") ");
            Implementation.Emit(Into);
            Into.Append(")");
        }
        public override object GetSystemValue() { return this; }
        public override Atom Evaluate(EvaluationContext Context) { throw new EvaluationError("Function improperly evaluated."); }
    }
}
