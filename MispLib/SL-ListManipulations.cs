using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISPLIB
{
    public class SL_ListManipulations : CoreFactory
    {
        public override void Create() {

            Core.AddCoreFunction("map 'x list 'code", (args, c) =>
                {
                    if (args[0].Type != AtomType.Token) throw new EvaluationError("Expected argument name as first argument to map.");
                    if (args[1].Type != AtomType.List) throw new EvaluationError("Expected list as second argument to map.");

                    var scope = new RecordAtom { Parent = c.ActiveScope };
                    c.ActiveScope = scope;
                    var r = new List<Atom>();

                    foreach (var v in (args[1] as ListAtom).Value)
                    {
                        scope.Variables.Upsert((args[0] as TokenAtom).Value, v);
                        r.Add(args[2].Evaluate(c));
                    }

                    c.ActiveScope = scope.Parent;
                    return new ListAtom { Value = r };
                });

            Core.AddCoreFunction("mapex 'x 'next-x 'code", (args, c) =>
            {
                if (args[0].Type != AtomType.Token) throw new EvaluationError("Expected argument name as first argument to map.");

                var scope = new RecordAtom { Parent = c.ActiveScope };
                c.ActiveScope = scope;
                var r = new List<Atom>();

                MISPLIB.Atom v = new NilAtom();
                scope.Variables.Upsert((args[0] as TokenAtom).Value, v);

                do
                {
                    v = args[1].Evaluate(c);
                    scope.Variables.Upsert((args[0] as TokenAtom).Value, v);
                    if (v.Type != AtomType.Nil) r.Add(args[2].Evaluate(c));
                } 
                while (v.Type != AtomType.Nil);
                
                c.ActiveScope = scope.Parent;
                return new ListAtom { Value = r };
            });

            Core.AddCoreFunction("fold 'x 'y first-value list 'code", (args, c) =>
                {
                    if (args[0].Type != AtomType.Token) throw new EvaluationError("Expected argument name as first argument to fold.");
                    if (args[1].Type != AtomType.Token) throw new EvaluationError("Expected argument name as second argument to fold.");
                    if (args[3].Type != AtomType.List) throw new EvaluationError("Expected list as fourth argument to fold.");

                    var v = args[2]; //first-value

                    var scope = new RecordAtom { Parent = c.ActiveScope };
                    c.ActiveScope = scope;

                    foreach (var a in (args[3] as ListAtom).Value)
                    {
                        scope.Variables.Upsert((args[0] as TokenAtom).Value, v);
                        scope.Variables.Upsert((args[1] as TokenAtom).Value, a);
                        v = args[4].Evaluate(c);
                    }

                    c.ActiveScope = scope.Parent;
                    return v;
                });

            Core.AddCoreFunction("where 'x list 'code", (args, c) =>
                {
                    if (args[0].Type != AtomType.Token) throw new EvaluationError("Expected argument name as first argument to where.");
                    if (args[1].Type != AtomType.List) throw new EvaluationError("Expected list as second argument to where.");

                    var scope = new RecordAtom { Parent = c.ActiveScope };
                    c.ActiveScope = scope;
                    var r = new List<Atom>();

                    foreach (var v in (args[1] as ListAtom).Value)
                    {
                        scope.Variables.Upsert((args[0] as TokenAtom).Value, v);
                        var testResult = args[2].Evaluate(c);
                        if (testResult.Type == AtomType.Integer && (testResult as IntegerAtom).Value != 0)
                            r.Add(v);
                    }

                    c.ActiveScope = scope.Parent;
                    return new ListAtom { Value = r };
                });

            Core.AddCoreFunction("for 'v from to 'code", (args, c) =>
                {
                    if (args[0].Type != AtomType.Token) throw new EvaluationError("Expected argument name as first argument to for.");
                    if (args[1].Type != AtomType.Integer) throw new EvaluationError("Expected minimum index as second argument to for.");
                    if (args[2].Type != AtomType.Integer) throw new EvaluationError("Expected maximum exclusive index as third argument to for.");

                    Atom v = new NilAtom();
                    var scope = new RecordAtom { Parent = c.ActiveScope };
                    c.ActiveScope = scope;

                    for (int i = (args[1] as IntegerAtom).Value; i < (args[2] as IntegerAtom).Value; ++i)
                    {
                        scope.Variables.Upsert((args[0] as TokenAtom).Value, new IntegerAtom { Value = i });
                        v = args[3].Evaluate(c);
                    }

                    c.ActiveScope = scope.Parent;

                    return v;
                });

        }
    }
}
