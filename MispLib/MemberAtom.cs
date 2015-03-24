using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISPLIB
{
    public class MemberAtom : Atom
    {
        public Atom Lhs;
        public Atom Rhs;
        public override AtomType Type { get { return AtomType.Member; }}

        protected override void ImplementEmit(StringBuilder Into) 
        { 
            Lhs.Emit(Into); 
            Into.Append(".");
            Rhs.Emit(Into);
        }

        public override object GetSystemValue() { return this; }

        public override Atom Evaluate(EvaluationContext Context)
        {
            if (Modifier == MISPLIB.Modifier.Quote) return new MemberAtom { Lhs = Lhs, Rhs = Rhs, Modifier = MISPLIB.Modifier.None };

            Atom result = Lhs.Evaluate(Context);
            if (result.Type != AtomType.Record) throw new EvaluationError("Expected record on LHS of . operator");
            var record = result as RecordAtom;

            Atom name = Rhs;
            if (Rhs.Type != AtomType.Token || Rhs.Modifier == MISPLIB.Modifier.Evaluate)
                name = Rhs.Evaluate(Context);
            if (name.Type != AtomType.Token) throw new EvaluationError("Expected token on RHS of . operator");

            var hasMember = record.TryGetValue((name as TokenAtom).Value, out result);
            if (hasMember) return result;
            return new NilAtom();
        }
    }
}
