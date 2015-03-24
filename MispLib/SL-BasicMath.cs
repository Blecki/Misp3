using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISPLIB
{
    public class SL_BasicMath : CoreFactory
    {
        private static int ival(Atom Atom)
        {
            if (Atom.Type == AtomType.Integer) return (Atom as IntegerAtom).Value;
            else if (Atom.Type == AtomType.Decimal) return (int)(Atom as DecimalAtom).Value;
            else throw new EvaluationError("Can't get ival of non numeric atom.");
        }

        private static float fval(Atom Atom)
        {
            if (Atom.Type == AtomType.Integer) return (float)(Atom as IntegerAtom).Value;
            else if (Atom.Type == AtomType.Decimal) return (Atom as DecimalAtom).Value;
            else throw new EvaluationError("Can't get fval of non numeric atom.");
        }

        public override void Create()
        {
            Core.AddCoreFunction("+ +value", (args, c) =>
            {
                var realArgs = (args[0] as ListAtom).Value;
                foreach (var v in realArgs) if (v.Type != AtomType.Integer && v.Type != AtomType.Decimal) throw new EvaluationError("Incorrect argument type passed to +");
                if (realArgs.Count(v => v.Type == AtomType.Decimal) > 0)
                {
                    var sum = 0.0f;
                    foreach (var v in realArgs) sum += fval(v);
                    return new DecimalAtom(sum);
                }
                else
                {
                    var sum = 0;
                    foreach (var v in realArgs) sum += ival(v);
                    return new IntegerAtom { Value = sum };
                }
            });

            Core.AddCoreFunction("- a b", (args, c) =>
                {
                    foreach (var v in args) if (v.Type != AtomType.Integer && v.Type != AtomType.Decimal) throw new EvaluationError("Incorrect argument type passed to -");
                    if (args[0].Type == AtomType.Integer && args[1].Type == AtomType.Integer)
                        return new IntegerAtom(ival(args[0]) - ival(args[1]));
                    else
                        return new DecimalAtom(fval(args[0]) - fval(args[1]));
                });

            Core.AddCoreFunction("* a b", (args, c) =>
            {
                foreach (var v in args) if (v.Type != AtomType.Integer && v.Type != AtomType.Decimal) throw new EvaluationError("Incorrect argument type passed to -");
                if (args[0].Type == AtomType.Integer && args[1].Type == AtomType.Integer)
                    return new IntegerAtom(ival(args[0]) * ival(args[1]));
                else
                    return new DecimalAtom(fval(args[0]) * fval(args[1]));
            });

            Core.AddCoreFunction("/ a b", (args, c) =>
            {
                foreach (var v in args) if (v.Type != AtomType.Integer && v.Type != AtomType.Decimal) throw new EvaluationError("Incorrect argument type passed to -");
                if (args[0].Type == AtomType.Integer && args[1].Type == AtomType.Integer)
                    return new IntegerAtom(ival(args[0]) / ival(args[1]));
                else
                    return new DecimalAtom(fval(args[0]) / fval(args[1]));
            });
        }
    }
}
