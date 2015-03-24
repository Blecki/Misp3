using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISPLIB
{
    public class ListAtom : Atom
    {
        public List<Atom> Value;
        public override AtomType Type { get { return AtomType.List; } }

        protected override void ImplementEmit(StringBuilder Into)
        {
            Into.Append("(");
            for (int i = 0; i < Value.Count; ++i)
            {
                Value[i].Emit(Into);
                if (i < (Value.Count - 1)) Into.Append(" ");
            }
            Into.Append(")");
        }

        public override object GetSystemValue() { return this; }

        private static List<Atom> EvaluateArguments(Atom Source, Atom Destination, EvaluationContext Context)
        {
            var To = new List<Atom>();
            if (Source.Modifier == Modifier.Evaluate)
                To.Add(Source.Evaluate(Context));
            else if (Source.Modifier == Modifier.Expand)
            {
                var v = Source.Evaluate(Context);
                if (v.Type == AtomType.List)
                    To.AddRange((v as ListAtom).Value);
                else
                    To.Add(v);
            }
            else if (Destination.Modifier == Modifier.Quote)
            {
                To.Add(Source);
            }
            else
            {
                To.Add(Source.Evaluate(Context));
            }
            return To;
        }

        public static List<Atom> PrepareStandardArgumentList(List<Atom> SourceArguments, List<TokenAtom> ArgumentNames, EvaluationContext Context)
        {
            var argumentList = new List<Atom>();
            var destinationIndex = 0;
            var sourceIndex = 1;

            while (destinationIndex < ArgumentNames.Count && sourceIndex < SourceArguments.Count)
            {
                var destinationArgument = ArgumentNames[destinationIndex];
                var source = SourceArguments[sourceIndex];
                ++sourceIndex;
                var expandedArgument = EvaluateArguments(source, destinationArgument, Context);

                foreach (var argument in expandedArgument)
                {
                    if (destinationIndex >= ArgumentNames.Count) throw new EvaluationError("Too many arguments to function.");
                    destinationArgument = ArgumentNames[destinationIndex];
                    List<Atom> addTo = null;

                    if (destinationArgument.Value.StartsWith("+") || destinationArgument.Value.StartsWith("*"))
                    {
                        if (destinationIndex == argumentList.Count)
                        {
                            var listArg = new ListAtom() { Value = new List<Atom>() };
                            addTo = listArg.Value;
                            argumentList.Add(listArg);
                        }
                        else
                        {
                            var listArg = argumentList.Last() as ListAtom;
                            if (listArg == null) throw new InvalidOperationException();
                            addTo = listArg.Value;
                        }
                    }
                    else
                    {
                        addTo = argumentList;
                        ++destinationIndex;
                    }

                    addTo.Add(argument);
                }
            }

            while (argumentList.Count < ArgumentNames.Count)
            {
                var destinationArgument = ArgumentNames[argumentList.Count];
                if (destinationArgument.Value.StartsWith("*"))
                    argumentList.Add(new ListAtom() { Value = new List<Atom>() });
                else
                    throw new EvaluationError("Not enough arguments to function.");
            }

            if (sourceIndex < SourceArguments.Count) throw new EvaluationError("Too many arguments to function.");
            if (argumentList.Count != ArgumentNames.Count) throw new EvaluationError("Incorrect number of arguments to function.");
            if (ArgumentNames.Count > 0 && ArgumentNames.Last().Value.StartsWith("+"))
            {
                var listArg = argumentList.Last() as ListAtom;
                if (listArg == null) throw new InvalidOperationException();
                if (listArg.Value.Count == 0) throw new EvaluationError("+ argument demands at least one argument.");
            }

            return argumentList;
        }


        public override Atom Evaluate(EvaluationContext Context)
        {
            if (Modifier == MISPLIB.Modifier.Quote) return new ListAtom { Value = Value, Modifier = MISPLIB.Modifier.None };

            if (Value.Count == 0) throw new EvaluationError("Can't evaluate empty list.");

            if (Value[0].Type == AtomType.Token)
            {
                var functionToken = Value[0] as TokenAtom;
                var coreFunction = Core.CoreFunctions.FirstOrDefault(cf => cf.Name == functionToken.Value);
                if (coreFunction != null)
                    return coreFunction.Implementation(PrepareStandardArgumentList(Value, coreFunction.ArgumentNames, Context), Context);
            }

            var firstAtom = Value[0].Evaluate(Context);
            if (firstAtom.Type != AtomType.Function)
            {
                if (Value.Count == 1) return firstAtom;
                else throw new EvaluationError("First atom in evaluated list must be a function atom.");
            }

            var function = firstAtom as FunctionAtom;
            var argumentList = PrepareStandardArgumentList(Value, function.ArgumentNames, Context);
                       
            var saveScope = Context.ActiveScope;
            Context.ActiveScope = new RecordAtom { Parent = function.DeclarationScope };

            for (int i = 0; i < function.ArgumentNames.Count; ++i)
            {
                var name = function.ArgumentNames[i].Value;
                if (name.StartsWith("+") || name.StartsWith("*"))
                    name = name.Substring(1);
                Context.ActiveScope.Variables.Upsert(name, argumentList[i]);
            }

            var result = function.Implementation.Evaluate(Context);
            Context.ActiveScope = saveScope;

            return result;
        }
    }
}
