using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace misp
{
    class Program
    {

        static void Main(string[] clargs)
        {
            try
            {
                MISPLIB.RecordAtom GlobalScope = new MISPLIB.RecordAtom();
                GlobalScope.Variables.Upsert("@", GlobalScope);
                MISPLIB.Core.InitiateCore(Console.Write);

                MISPLIB.Core.AddCoreFunction("print +arg", (args, c) =>
                {
                    var builder = new StringBuilder();
                    foreach (var v in (args[0] as MISPLIB.ListAtom).Value)
                        if (v.Type == MISPLIB.AtomType.String)
                            builder.Append((v as MISPLIB.StringAtom).Value);
                        else
                            v.Emit(builder);
                    Console.WriteLine(builder.ToString());
                    return new MISPLIB.NilAtom();
                });

                MISPLIB.Core.AddCoreFunction("core", (args, c) =>
                {
                    var builder = new StringBuilder();
                    foreach (var func in MISPLIB.Core.CoreFunctions)
                    {
                        builder.Append(func.Name);
                        foreach (var name in func.ArgumentNames)
                        {
                            builder.Append(" ");
                            name.Emit(builder);
                        }
                        builder.Append("\n");
                    }

                    Console.WriteLine(builder.ToString());
                    return new MISPLIB.NilAtom();
                });

                //MISPLIB.Core.AddCoreFunction("@", (args, c) =>
                //{
                //    return GlobalScope;
                //});

                var place = 0;
                while (place < clargs.Length)
                {
                    if (clargs[place] == "-f" || clargs[place] == "-e")
                    {
                        ++place;
                        if (place == clargs.Length)
                        {
                            Console.WriteLine("Expected an argument to -f");
                            return;
                        }

                        var text = System.IO.File.ReadAllText(clargs[place]);
                        var parsed = MISPLIB.Core.Parse(new MISPLIB.StringIterator(text));
                        var result = MISPLIB.Core.Evaluate(parsed, GlobalScope);
                        if (result.Type != MISPLIB.AtomType.Record) throw new MISPLIB.EvaluationError("Loading of file did not produce record.");
                        GlobalScope = result as MISPLIB.RecordAtom;
                        GlobalScope.Variables.Upsert("@", GlobalScope);
                    }
                    else
                    {
                        var parsed = MISPLIB.Core.Parse(new MISPLIB.StringIterator(clargs[place]));
                        var result = MISPLIB.Core.Evaluate(parsed, GlobalScope);
                        var outputBuilder = new StringBuilder();
                        MISPLIB.Core.EmissionID = Guid.NewGuid();
                        result.Emit(outputBuilder);
                        Console.WriteLine(outputBuilder.ToString());
                    }

                    ++place;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
