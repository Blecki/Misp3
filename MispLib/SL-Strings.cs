using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISPLIB
{
    public class SL_Strings : CoreFactory
    {
        public override void Create()
        {
            Core.AddCoreFunction("slength string", (args, c) =>
                {
                    if (args[0].Type != AtomType.String) throw new EvaluationError("Expected string as argument to slength");
                    return new IntegerAtom((args[0] as StringAtom).Value.Length);
                });

            Core.AddCoreFunction("sindex string index", (args, c) =>
            {
                if (args[1].Type != AtomType.Integer) throw new EvaluationError("Expected integer index as second argument to sindex.");
                if (args[0].Type == AtomType.String) return new IntegerAtom { Value = (args[0] as StringAtom).Value[(args[1] as IntegerAtom).Value] };
                else throw new EvaluationError("Expected string as first argument to sindex.");
            });

            Core.AddCoreFunction("to-char int", (args, c) =>
                {
                    if (args[0].Type != AtomType.Integer) throw new EvaluationError("Expected integer as argument to to-char.");
                    return new StringAtom { Value = new String((char)(args[0] as IntegerAtom).Value, 1) };
                });

            Core.AddCoreFunction("ssubstring string start length", (args, c) =>
                {
                    if (args[0].Type != AtomType.String) throw new EvaluationError("Expected string as first argument to ssubstring");
                    if (args[1].Type != AtomType.Integer) throw new EvaluationError("Expected integer as second argument to ssubstring.");
                    if (args[2].Type != AtomType.Integer) throw new EvaluationError("Expected integer as third argument to ssubstring.");

                    var str = (args[0] as StringAtom).Value;
                    var start = (args[1] as IntegerAtom).Value;
                    var len = (args[2] as IntegerAtom).Value;

                    if (start < 0)
                    {
                        len += start;
                        start = 0;
                    }

                    if (start >= str.Length) return new StringAtom { Value = "" };
                    if (start + len > str.Length) len = str.Length - start;
                    if (len == 0) return new StringAtom { Value = "" };
                    return new StringAtom { Value = str.Substring(start, len) };
                });

            Core.AddCoreFunction("rmatch regex string", (args, c) =>
                {
                    if (args[0].Type != AtomType.String) throw new EvaluationError("Expected string as first argument to rmatch");
                    if (args[1].Type != AtomType.String) throw new EvaluationError("Expected string as second argument to rmatch");

                    var result = System.Text.RegularExpressions.Regex.IsMatch((args[1] as StringAtom).Value, (args[0] as StringAtom).Value);

                    if (result)
                        return new IntegerAtom { Value = 1 };
                    else
                        return new IntegerAtom { Value = 0 };
                });
        }
    }
}
