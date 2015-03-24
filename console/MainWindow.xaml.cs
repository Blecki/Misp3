using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SharpConsole
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<String> CommandMemory = new List<string>();
        public int MemoryScrollIndex = 0;
        MISPLIB.RecordAtom GlobalScope = new MISPLIB.RecordAtom();
        public bool Echo = true;
        public bool ShowResult = true;
        String OpenFilePath = null;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                TextBox_TextChanged(null, null);

                InputBox.Focus();

                this.Title = "New Environment";

                #region Setup MISP

                MISPLIB.Core.InitiateCore(s => __print(s));

                MISPLIB.Core.AddCoreFunction("recall function", (args, c) =>
                {
                    MISPLIB.Core.EmissionID = Guid.NewGuid();
                    var builder = new StringBuilder();
                    args[0].Emit(builder);
                    InputBox.Text = builder.ToString();
                    return args[0];
                });

                //MISPLIB.Core.AddCoreFunction("@", (args, c) =>
                //{
                //    return GlobalScope;
                //});

                MISPLIB.Core.AddCoreFunction("print +arg", (args, c) =>
                {
                    var builder = new StringBuilder();
                    foreach (var v in (args[0] as MISPLIB.ListAtom).Value)
                        if (v.Type == MISPLIB.AtomType.String)
                            builder.Append((v as MISPLIB.StringAtom).Value);
                        else
                            v.Emit(builder);
                    __print(builder.ToString());
                    return new MISPLIB.NilAtom();
                });

                MISPLIB.Core.AddCoreFunction("clear", (args, c) =>
                {
                    __clear();
                    return new MISPLIB.NilAtom();
                });

                MISPLIB.Core.AddCoreFunction("save file", (args, c) =>
                {
                    if (args[0].Type != MISPLIB.AtomType.String) throw new MISPLIB.EvaluationError("Expected string as first argument to save.");
                    var saveFileName = (args[0] as MISPLIB.StringAtom).Value;

                    SaveFile(saveFileName);
                    return new MISPLIB.StringAtom { Value = OpenFilePath };
                });

                MISPLIB.Core.AddCoreFunction("load file", (args, c) =>
                {
                    if (args[0].Type != MISPLIB.AtomType.String) throw new MISPLIB.EvaluationError("Expected path as first argument to load.");

                    LoadFile((args[0] as MISPLIB.StringAtom).Value);
                    return GlobalScope;
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

                    __print("<pre>" + builder.ToString() + "</pre>");
                    return new MISPLIB.NilAtom();
                });

                MISPLIB.Core.AddCoreFunction("setting 'name value", (args, c) =>
                {
                    if (args[0].Type != MISPLIB.AtomType.Token) throw new MISPLIB.EvaluationError("Expected setting name as first argument to setting.");
                    if (args[1].Type != MISPLIB.AtomType.Integer) throw new MISPLIB.EvaluationError("Expected integer value as second argument to setting.");

                    var newValue = (args[1] as MISPLIB.IntegerAtom).Value != 0;
                    var setting = (args[0] as MISPLIB.TokenAtom).Value;

                    if (setting == "echo") Echo = newValue;
                    else if (setting == "show-result") ShowResult = newValue;
                    else throw new MISPLIB.EvaluationError("Unknown setting.");

                    return args[1];
                });

                #endregion
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            var startContent = @"<script>
    function scroll()
    {
        window.scrollTo(0,document.body.scrollHeight);
    } 
</script><div>Welcome to MISP 3.0<br>";

            try
            {
                LoadFile("auto.misp");
            } 
            catch (Exception e)
            {
                startContent += "Could not load auto environment: " + e.Message + "<br>";
                GlobalScope.Variables.Upsert("@", GlobalScope);
            }

            OutputBox.NavigateToString(startContent);
        }

        private void SaveFile(string saveFileName)
        {
            var serializer = new MISPLIB.SerializationContext();
            var builder = new StringBuilder();
            serializer.Serialize(GlobalScope, builder);

            var dirName = System.IO.Path.GetDirectoryName(saveFileName);
            if (!String.IsNullOrEmpty(dirName))
                System.IO.Directory.CreateDirectory(dirName);

            System.IO.File.WriteAllText(saveFileName, builder.ToString());
            OpenFilePath = saveFileName;
            this.Title = OpenFilePath;
        }

        private void LoadFile(String FileName)
        {
            if (!System.IO.File.Exists(FileName))
                throw new MISPLIB.EvaluationError("File not found");

            var text = System.IO.File.ReadAllText(FileName);
            var parsed = MISPLIB.Core.Parse(new MISPLIB.StringIterator(text));
            var result = MISPLIB.Core.Evaluate(parsed, GlobalScope);
            if (result.Type != MISPLIB.AtomType.Record) throw new MISPLIB.EvaluationError("Loading of file did not produce record.");
            OpenFilePath = FileName;
            this.Title = OpenFilePath;
            GlobalScope = result as MISPLIB.RecordAtom;
            GlobalScope.Variables.Upsert("@", GlobalScope);
        }

        private void __print(String s)
        {
            var doc = OutputBox.Document as mshtml.HTMLDocument;
            doc.body.innerHTML += s.Replace("\n", "<br>");
            OutputBox.InvokeScript("scroll");
        }

        private void __clear()
        {
            OutputBox.NavigateToString(
@"<script>
    function scroll()
    {
        window.scrollTo(0,document.body.scrollHeight);
    } 
</script><div>");
        }

        private void __setInput(String s)
        {
            InputBox.Text = s;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var realFontSize = this.InputBox.FontSize * this.InputBox.FontFamily.LineSpacing;
            var adjustedLineCount = System.Math.Max(this.InputBox.LineCount, 1) + 1;
            var newHeight = realFontSize * adjustedLineCount;
            if (newHeight > this.ActualHeight * 0.75f) newHeight = this.ActualHeight * 0.75f;
            if (newHeight < realFontSize * 2) newHeight = realFontSize * 2;
            this.BottomRow.Height = new GridLength(newHeight);
        }

        private void InputBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Up)
            {
                MemoryScrollIndex -= 1;
                if (MemoryScrollIndex < 0) MemoryScrollIndex = 0;
                if (MemoryScrollIndex < CommandMemory.Count)
                {
                    InputBox.Text = CommandMemory[MemoryScrollIndex];
                    InputBox.Focus();
                    InputBox.SelectAll();
                }

                e.Handled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Down)
            {
                MemoryScrollIndex += 1;
                if (MemoryScrollIndex > CommandMemory.Count) MemoryScrollIndex = CommandMemory.Count;
                if (MemoryScrollIndex < CommandMemory.Count)
                {
                    InputBox.Text = CommandMemory[MemoryScrollIndex];
                    InputBox.Focus();
                    InputBox.SelectAll();
                }

                e.Handled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Return)
            {
                SendInput();
                e.Handled = true;
            }
        }

        private void SendInput()
        {
            var saveInput = InputBox.Text.Trim();
            var code = "(" + saveInput + ")";

            try
            {
                if (Echo) __print("<div color=gray>" + code + "</div>");

                var parsedMisp = MISPLIB.Core.Parse(new MISPLIB.StringIterator(code));
                InputBox.Clear();

                var evaluatedResult = MISPLIB.Core.Evaluate(parsedMisp, GlobalScope);

                if (ShowResult)
                {
                    var outputBuilder = new StringBuilder();
                    MISPLIB.Core.EmissionID = Guid.NewGuid();
                    evaluatedResult.Emit(outputBuilder);
                    __print("<div color=green>" + outputBuilder.ToString() + "</div>");
                }

                CommandMemory.Add(saveInput);
                MemoryScrollIndex = CommandMemory.Count;
            }
            catch (Exception x)
            {
                __print("<div color=red>" + x.Message + "</div>");
                InputBox.Text = saveInput;
            }
        }

        private void OutputBox_LoadCompleted(object sender, NavigationEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (OpenFilePath == "auto.misp")
                SaveFile("auto.misp");
        }

    }
}
