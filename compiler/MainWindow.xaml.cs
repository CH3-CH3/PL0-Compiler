using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Xml;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace compiler {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {
        Random random = new Random(114514);

        Lexical lexical;
        List<Token> tokens;
        Parser parser;
        PcodeList pcodes;
        Interpreter interpreter;
        SymTab[] symTable;

        bool compileSuccess;
        string ans;

        public MainWindow() {
            InitializeComponent();
            UIControlInit();
            CompilerInit();
        }

        void UIControlInit() {
            textEditor.ShowLineNumbers = true;

            Assembly _assembly = Assembly.GetExecutingAssembly();
            using (Stream s = _assembly.GetManifestResourceStream("compiler.pl0Highlight.xshd")) {
                using (XmlTextReader reader = new XmlTextReader(s)) {
                    textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        void CompilerInit() {
            //模块初始化
            Data.errors = new ErrorList();
            lexical = new Lexical();
            parser = new Parser();
            interpreter = new Interpreter();

            compileSuccess = false;
        }

        void DataSync() {

        }

        bool OpenFile() {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "所有文件|*.*";
            if (ofd.ShowDialog() == true) {
                string sourcePath = ofd.FileName;
                try {
                    StreamReader file = new StreamReader(sourcePath);
                    textEditor.Text = file.ReadToEnd();
                    file.Close();
                }
                catch {
                    return false;
                }
                return true;
            }
            return false;
        }

        bool SaveFile() {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "所有文件|*.*";
            sfd.FileName = "pcode";
            sfd.DefaultExt = ".txt";
            if (sfd.ShowDialog() == true) {
                string savePath = sfd.FileName;
                figureAns();
                string dataFormat = ans;
                try {
                    File.Delete(savePath);
                    StreamWriter file = new StreamWriter(savePath);
                    file.Write(dataFormat);
                    file.Close();
                }
                catch {
                    return false;
                }
                return true;
            }
            return false;
        }

        void figureAns() {
            ans = "步骤".PadRight(8) + "指令".PadRight(8) + "层次差".PadRight(7) + "地址".PadRight(8) + "\n";
            foreach (Pcode p in pcodes.Pcodes) {
                ans += p.Num.ToString().PadRight(10) + p.Ins.ToString().PadRight(10) + p.Left.ToString().PadRight(10) + p.Right.ToString().PadRight(10) + "\n";
            }
        }

        //void figureAns2() {
        //    ans = "";
        //    foreach (Pcode p in pcodes.Pcodes) {
        //        ans +=  p.Ins.ToString() + " " + p.Left.ToString() + " " + p.Right.ToString() + "\r\n";
        //    }
        //}

        private void loadButton_Click(object sender, RoutedEventArgs e) {
            if (!OpenFile()) {
                Task t = ShowMessage(5);
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e) {
            if (!SaveFile()) {
                Task t = ShowMessage(6);
            }
        }

        private void compileButton_Click(object sender, RoutedEventArgs e) {

            Compile();

            dataGrid1.ItemsSource = null;
            dataGrid2.ItemsSource = null;

            dataGrid1.ItemsSource = pcodes.Pcodes;
            Data.errors.Errors.Sort();
            dataGrid2.ItemsSource = Data.errors.GetErrors(Data.ErrorNum);

            //figureAns();
        }

        private void runButton_Click(object sender, RoutedEventArgs e) {
            if (!compileSuccess) {
                Task t = ShowMessage(4);
                return;
            }

            tab3.IsSelected = true;
            string input = inputBox.Text;
            string output;
            Interpret(input, out output);

            Output(output);
        }

        private void headerButton_Click(object sender, RoutedEventArgs e) {
            int tmp = random.Next(100);
            if (tmp < 40) {
                textBlock1.Text = "Prise the sun!";
            }
            else if (tmp < 80) {
                textBlock1.Text = "May the Force be with you!";
            }
            else if (tmp < 90) {
                textBlock1.Text = @"https://granbluefantasy.jp/";
            }
            else {
                textBlock1.Text = "YOU ARE LUCKY!";
            }
        }

        void Compile() {
            Data.LexicalSuccess = false;
            Data.ParserSuccess = false;
            textBlock1.Text = "";
            textBlock1.Text += "正在进行词法分析...\n";

            lexical.LexicalAnalyse(textEditor.Text);
            tokens = lexical.GetTokens();
            if (Data.LexicalSuccess) {
                textBlock1.Text += "词法分析成功！\n";
            }
            else {
                textBlock1.Text += "词法分析失败！\n";
            }

            textBlock1.Text += "正在进行语法和语义分析...\n";
            parser.Parse(tokens);
            if (Data.ParserSuccess) {
                textBlock1.Text += "语法分析成功！\n";
            }
            else {
                textBlock1.Text += "语法分析失败！\n";
            }

            textBlock1.Text += "正在获得符号表...\n";
            symTable = parser.GetSymTable().GetLegalSymTabs();

            textBlock1.Text += "正在获得p-code...\n";
            pcodes = parser.GetPcodeList();

            compileSuccess = (Data.LexicalSuccess && Data.ParserSuccess);
            if (compileSuccess) {
                textBlock1.Text += "编译成功！\n";
                tab1.IsSelected = true;
                Task t = ShowMessage(1);
            }
            else {
                textBlock1.Text += "编译失败，共发现" + Data.errors.Errors.Count + "个错误！\n";
                tab2.IsSelected = true;
                if (Data.LexicalSuccess) {
                    Task t = ShowMessage(3);
                }
                else {
                    Task t = ShowMessage(2);
                }
            }
        }

        bool Interpret(string input, out string output) {
            string[] inputs = input.Split();
            try {
                bool flag = interpreter.Interpret(pcodes, inputs, out output);
                textBlock1.Text += "运行成功!\n";
                if (!flag) {
                    Task t = ShowMessage(8);
                }
            }
            catch {
                output = "error!";
                Task t = ShowMessage(7);
                return false;
            }
            return true;
        }

        void Output(string output) {
            string outStr = "";
            outStr += output;
            outStr += "\n";
            textBox1.Text = outStr;
        }

        private void dataGrid2_MouseDown(object sender, MouseButtonEventArgs e) {
            try { selectLine(((Error)dataGrid2.SelectedItem).Line); }
            catch { }
        }

        private void selectLine(int lineNum) {
            DocumentLine line = textEditor.Document.GetLineByNumber(lineNum);
            textEditor.Select(line.Offset, line.Length);
        }

        async Task ShowMessage(int num) {
            switch (num) {
                case 1: {
                        await this.ShowMessageAsync("恭喜", "编译成功！");
                        break;
                    }
                case 2: {
                        await this.ShowMessageAsync("编译失败", "可能遇到了一些词法问题。");
                        break;
                    }
                case 3: {
                        await this.ShowMessageAsync("编译失败", "可能遇到了一些语法问题。");
                        break;
                    }
                case 4: {
                        await this.ShowMessageAsync("运行失败", "运行前请先进行编译！");
                        break;
                    }
                case 5: {
                        await this.ShowMessageAsync("错误", "打开文件失败！");
                        break;
                    }
                case 6: {
                        await this.ShowMessageAsync("错误", "保存文件失败！");
                        break;
                    }
                case 7: {
                        await this.ShowMessageAsync("运行时异常", "程序运行时间过长，自动中断！");
                        break;
                    }
                case 8: {
                        await this.ShowMessageAsync("警告", "变量输入数不足。");
                        break;
                    }
                default: {
                        break;
                    }
            }
        }

    }
}
