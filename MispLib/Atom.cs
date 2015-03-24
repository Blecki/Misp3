using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISPLIB
{
    public enum Modifier
    {
        None,
        Quote,
        Expand,
        Evaluate,
    }

    public enum AtomType
    {
        Token,
        Integer,
        Decimal,
        String,
        List,
        Record,
        Function,
        Member,
        Nil,
        Error,
    }

    public class Atom
    {
        public Modifier Modifier;
        public virtual AtomType Type { get { return AtomType.Error; } }
        public virtual Atom Evaluate(EvaluationContext Context) { throw new NotImplementedException(); }
        protected virtual void ImplementEmit(StringBuilder Into) { }
        public virtual Object GetSystemValue() { throw new NotImplementedException(); }

        public void Emit(StringBuilder Into)
        {
            switch (Modifier)
            {
                case MISPLIB.Modifier.None:
                    break;
                case MISPLIB.Modifier.Expand:
                    Into.Append("$");
                    break;
                case MISPLIB.Modifier.Quote:
                    Into.Append("'");
                    break;
                case MISPLIB.Modifier.Evaluate:
                    Into.Append(":");
                    break;
            }

            ImplementEmit(Into);
        }
    }
}