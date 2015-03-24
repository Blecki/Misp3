using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISPLIB
{
    public class RecordAtom : Atom
    {
        public bool Literal = false;
        public override AtomType Type { get { return AtomType.Record; } }
        public RecordAtom Parent;
        public Dictionary<String, Atom> Variables = new Dictionary<String, Atom>();
        public override object GetSystemValue() { return this; }
        public override Atom Evaluate(EvaluationContext Context)
        {
            var r = new RecordAtom { Variables = new Dictionary<string, Atom>() };
            foreach (var p in Variables)
                r.Variables.Upsert(p.Key, p.Value.Evaluate(Context));
            r.Literal = false;
            return r;
        }

        public bool TryGetValue(String Name, out Atom Value, List<RecordAtom> VisitedRecords = null)
        {
            Value = null;
            if (Variables.TryGetValue(Name, out Value)) return true;
            if (Parent != null)
            {
                if (VisitedRecords == null) VisitedRecords = new List<RecordAtom>();
                else if (VisitedRecords.Contains(Parent)) return false;
                VisitedRecords.Add(this);
                return Parent.TryGetValue(Name, out Value, VisitedRecords);
            }
            return false;
        }

        private Guid EmissionID = Guid.Empty;

        protected override void ImplementEmit(StringBuilder Into)
        {
            if (EmissionID == Core.EmissionID) Into.Append("<circular reference>");
            else
            {
                EmissionID = Core.EmissionID;

                Into.Append("[");
                foreach (var value in Variables)
                {
                    Into.Append("(");
                    Into.Append(value.Key);
                    Into.Append(" ");
                    value.Value.Emit(Into);
                    Into.Append(")");
                    //if (Variables.Count > 0) Into.Append("\n");
                }
                Into.Append("]");
            }
        }
    }
}
