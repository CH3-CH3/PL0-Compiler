using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler {
    class Lexical {
        #region Vars Definition

        ErrorList errors;
        int lineNumber;

        List<Token> words;
        int strPos;
        int symbol;
        bool fileEnd;
        string token;

        int cur_int;
        char cur_char;

        string sourceCode;

        #endregion

        public Lexical() {
            this.errors = Data.errors;
            AnalyzerInit();
        }

        #region Init and others
        private void AnalyzerInit() {
            words = new List<Token>();
            //LexicalInit();
        }

        private void LexicalInit() {
            words.Clear();
            errors.Clear();

            strPos = 0;
            lineNumber = 1;
            fileEnd = false;
        }

        #endregion
        #region Lexical Part

        /// 
        /// Lexical Part
        /// 

        void error(int errCode) {
            Data.lineNum = lineNumber;
            Data.token = token;
            errors.Add(errCode);
        }

        public void LexicalAnalyse(string str) {
            LexicalInit();
            sourceCode = str;
            sourceCode += ' ';
            while (strPos < sourceCode.Length) {
                GetSym();
                if (fileEnd) {
                    break;
                }
                Token word = new Token(token, symbol, lineNumber);
                if (symbol == (int)Type.INTSY) {
                    word.Var = cur_int;
                }
                else {
                    word.Var = token;
                }
                words.Add(word);
            }
            Data.LexicalSuccess = (errors.Errors.Count == 0 ? true : false);
        }

        public List<Token> GetTokens() {
            return words;
        }

        public string LexicalOutput() {
            string lexicalResult = ("单词".PadRight(20, ' ') + "类别".PadRight(20, ' ') + "值".PadRight(20, ' '));
            foreach (Token word in words) {
                string word_str = word.Name.PadRight(20, ' ') + word.Symbol.ToString().PadRight(20, ' ') + word.Var.ToString().PadRight(20, ' ');
                lexicalResult += (word_str + "\n");
            }
            return lexicalResult;
        }

        void GetSym() {
            token = "";
            symbol = 0;
            GetChar();
            while (IsSpace() || IsTab() || IsNewline2() || IsNewline1()) {
                if (IsNewline1()) {
                    lineNumber++;
                }
                cur_char = (char)0;
                GetChar();
            }
            if (fileEnd) {
                return;
            }
            if (IsLetter()) {
                while (IsLetter() || IsDigit()) {
                    token += cur_char;
                    GetChar();
                }
                Retract();
                int resultValue = Reserver();
                if (resultValue == 0) {
                    symbol = (int)Type.IDSY;
                }
                else {
                    symbol = resultValue;
                }
            }
            else if (IsDigit()) {
                while (IsDigit()) {
                    token += cur_char;
                    GetChar();
                }
                Retract();
                try {
                    cur_int = Int32.Parse(token);
                    symbol = (int)Type.INTSY;
                }
                catch {
                    error(28);
                }
            }
            else if (IsEqu()) {
                token += cur_char;
                symbol = (int)Type.EQUAL;
            }
            else if (IsLess()) {
                token += cur_char;
                GetChar();
                if (IsEqu()) {
                    token += cur_char;
                    symbol = (int)Type.NOTGREATER;
                }
                else if (IsGreater()) {
                    token += cur_char;
                    symbol = (int)Type.NOTEQUAL;
                }
                else {
                    Retract();
                    symbol = (int)Type.LESS;
                }
            }
            else if (IsGreater()) {
                token += cur_char;
                GetChar();
                if (IsEqu()) {
                    token += cur_char;
                    symbol = (int)Type.NOTLESS;
                }
                else {
                    Retract();
                    symbol = (int)Type.GREATER;
                }
            }
            else if (IsColon()) {
                token += cur_char;
                GetChar();
                if (IsEqu()) {
                    token += cur_char;
                    symbol = (int)Type.ASSIGN;
                }
                else {
                    Retract();
                    error(30);
                    symbol = 0;
                }
            }
            else if (IsDot()) {
                token += cur_char;
                symbol = (int)Type.DOT;
            }
            else if (IsLpar()) {
                token += cur_char;
                symbol = (int)Type.LPAR;
            }
            else if (IsRpar()) {
                token += cur_char;
                symbol = (int)Type.RPAR;
            }
            else if (IsPlus()) {
                token += cur_char;
                symbol = (int)Type.PLUS;
            }
            else if (IsMinus()) {
                token += cur_char;
                symbol = (int)Type.MINUS;
            }
            else if (IsTimes()) {
                token += cur_char;
                symbol = (int)Type.TIMES;
            }
            else if (IsDivi()) {
                token += cur_char;
                symbol = (int)Type.DIVIDE;
            }
            else if (IsSemi()) {
                token += cur_char;
                symbol = (int)Type.SEMI;
            }
            else if (IsComma()) {
                token += cur_char;
                symbol = (int)Type.COMMA;
            }
            else {
                error(31);
            }
        }

        int Reserver() {
            if (token == "const") {
                return (int)Type.CONST;
            }
            else if (token == "if") {
                return (int)Type.IF;
            }
            else if (token == "then") {
                return (int)Type.THEN;
            }
            else if (token == "else") {
                return (int)Type.ELSE;
            }
            else if (token == "while") {
                return (int)Type.WHILE;
            }
            else if (token == "do") {
                return (int)Type.DO;
            }
            else if (token == "call") {
                return (int)Type.CALL;
            }
            else if (token == "begin") {
                return (int)Type.BEGIN;
            }
            else if (token == "end") {
                return (int)Type.END;
            }
            else if (token == "repeat") {
                return (int)Type.REPEAT;
            }
            else if (token == "until") {
                return (int)Type.UNTIL;
            }
            else if (token == "read") {
                return (int)Type.READ;
            }
            else if (token == "write") {
                return (int)Type.WRITE;
            }
            else if (token == "procedure") {
                return (int)Type.PROCEDURE;
            }
            else if (token == "var") {
                return (int)Type.VAR;
            }
            else if (token == "odd") {
                return (int)Type.ODD;
            }
            else
                return 0;
        }

        bool GetChar() {
            if (strPos >= sourceCode.Length) {
                //Error();
                fileEnd = true;
                return false;
            }
            cur_char = sourceCode[strPos];
            strPos++;
            return true;
        }

        void Retract() {
            strPos--;
            cur_char = sourceCode[strPos - 1];
        }

        bool IsSpace() {
            return cur_char == ' ';
        }
        bool IsNewline1() {
            return (cur_char == '\n');
        }
        bool IsNewline2() {
            return (cur_char == '\r');
        }
        bool IsTab() {
            return cur_char == '\t';
        }
        bool IsLetter() {
            return ((cur_char >= 'a' && cur_char <= 'z') || (cur_char >= 'A' && cur_char <= 'Z'));
        }
        bool IsLegal() {
            return (cur_char > 31 && cur_char < 127);
        }
        bool IsDigit() {
            return (cur_char >= '0' && cur_char <= '9');
        }
        bool IsUnderLine() {
            return cur_char == '_';
        }
        bool IsEqu() {
            return cur_char == '=';
        }
        bool IsPlus() {
            return cur_char == '+';
        }
        bool IsMinus() {
            return cur_char == '-';
        }
        bool IsTimes() {
            return cur_char == '*';
        }
        bool IsLpar() {
            return cur_char == '(';
        }
        bool IsRpar() {
            return cur_char == ')';
        }
        bool IsLess() {
            return cur_char == '<';
        }
        bool IsGreater() {
            return cur_char == '>';
        }
        bool IsComma() {
            return cur_char == ',';
        }
        bool IsSemi() {
            return cur_char == ';';
        }
        bool IsColon() {
            return cur_char == ':';
        }
        bool IsDot() {
            return cur_char == '.';
        }
        bool IsDivi() {
            return cur_char == '/';
        }
        bool IsQuotation() {
            return cur_char == '\'';
        }
        #endregion
    }
}
