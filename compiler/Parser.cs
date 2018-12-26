using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler {
    class Parser {
        List<Token> tokens;
        SymSet declareFirstSym;
        SymSet statementFirstSym;
        SymSet factorFirstSym;

        SymTable table;
        PcodeList pcodes;
        int tokenPos;

        Token token {
            get {
                Data.lineNum = tokens[TokenPos].Line;
                Data.token = tokens[TokenPos].Name;
                if (tokens[TokenPos].Symbol == (int)Type.IDSY) {
                    Data.varName = Data.token;
                }
                if (tokens[TokenPos].Name == "tmp") {
                    ;
                }

                return tokens[TokenPos];
            }
        }

        public int TokenPos {
            get => tokenPos;
            set {
                tokenPos = value;
                if ((tokens != null) && TokenPos != tokens.Count - 1 &&  tokens[TokenPos].Symbol == (int)Type.DOT ) {
                    Data.errors.Add(33);
                    tokenPos++;
                }
            }
        }

        public Parser() {
            ParserInit();
        }

        void ParserInit() {
            statementFirstSym = new SymSet();
            statementFirstSym.Add(Type.BEGIN, Type.CALL, Type.IF, Type.WHILE, Type.REPEAT);

            declareFirstSym = new SymSet();
            declareFirstSym.Add(Type.CONST, Type.VAR, Type.PROCEDURE);

            factorFirstSym = new SymSet();
            factorFirstSym.Add(Type.IDSY, Type.INTSY, Type.LPAR);
            
            table = new SymTable();
            pcodes = new PcodeList();
            TokenPos = 0;

            Pcode.sum = 0;
        }

        /// <summary>
        /// 将符号串转为符号表和Pcode
        /// </summary>
        /// <param name="_tokens">待处理的符号串</param>
        public void Parse(List<Token> _tokens) {
            ParserInit();
            if (_tokens == null) {
                return;
            }
            tokens = _tokens;

            SymSet tmp = new SymSet();
            tmp.UnionWith(declareFirstSym);
            tmp.UnionWith(statementFirstSym);
            tmp.Add(Type.DOT);

            try {
                if (tokens.Count == 0) {
                    Data.errors.Add(9);
                }
                Block(0, 0, tmp);
                Data.ParserSuccess = (Data.errors.Errors.Count == 0 ? true : false);
                if(tokens[tokens.Count - 1].Symbol != (int)Type.DOT) {
                    Data.errors.Add(9);
                }
            }
            catch {
                if (tokens[tokens.Count - 1].Symbol != (int)Type.DOT) {
                    Data.lineNum = tokens[tokens.Count - 1].Line;
                    Data.token = tokens[tokens.Count - 1].Name;
                    Data.errors.Add(9);
                }
            }

        }

        public PcodeList GetPcodeList() {
            return pcodes;
        }

        public SymTable GetSymTable() {
            return table;
        }

        void isLegal(SymSet s1, SymSet s2, int n) {
            if (!s1.Contains((Type)token.Symbol)) {
                Data.errors.Add(n);
                JumpTokens(s1, s2);
            }
        }

        //出现错误时跳过Token，直到能正常运行
        void JumpTokens(SymSet s1, SymSet s2) {
            while (!s1.Contains((Type)token.Symbol) && !s2.Contains((Type)token.Symbol))
                TokenPos++;
        }

        void Block(int level, int symTableSize, SymSet firstSym) {
            int dataSize = 3, symTableSize1, pcodeSize1; //用于生成符号表
            SymSet tmpSet;
            symTableSize1 = symTableSize;
            table.Table[symTableSize].Address = pcodes.Count;
            pcodes.Add(INSTRACTION.JMP, 0, 0);

            if (level > Data.MaxLevel) {//嵌套层数>3
                Data.errors.Add(31);
            }

            do {
                if (token.Symbol == (int)Type.CONST) {
                    TokenPos++;
                    do {
                        ConstDeclaration(level, ref symTableSize, ref dataSize);
                        while (token.Symbol == (int)Type.COMMA) {
                            TokenPos++;
                            ConstDeclaration(level, ref symTableSize, ref dataSize);
                        }
                        if (token.Symbol == (int)Type.SEMI) {
                            TokenPos++;
                            break;
                        }
                        else
                            Data.errors.Add(5);
                    } while (token.Symbol == (int)Type.IDSY);
                }
                if (token.Symbol == (int)Type.VAR) {
                    TokenPos++;
                    do {
                        VariableDeclaration(level, ref symTableSize, ref dataSize);
                        while (token.Symbol == (int)Type.COMMA) {
                            TokenPos++;
                            VariableDeclaration(level, ref symTableSize, ref dataSize);
                        }
                        if (token.Symbol == (int)Type.SEMI) {
                            TokenPos++;
                            break;
                        }
                        else
                            Data.errors.Add(5);
                    } while (token.Symbol == (int)Type.IDSY);
                }

                while (token.Symbol == (int)Type.PROCEDURE) {
                    TokenPos++;
                    if (token.Symbol == (int)Type.IDSY) {
                        table.Add(Identifier.PROCEDURE, ref dataSize, level, ref symTableSize, token);//添加符号表
                        TokenPos++;
                    }
                    else {
                        Data.errors.Add(4);
                    }

                    if (token.Symbol == (int)Type.SEMI) {
                        TokenPos++;
                    }
                    else {
                        Data.errors.Add(5);
                    }

                    tmpSet = new SymSet();
                    tmpSet.UnionWith(firstSym);
                    tmpSet.Add(Type.SEMI);

                    Block(level + 1, symTableSize, tmpSet);

                    if (token.Symbol == (int)Type.SEMI) {
                        TokenPos++;
                        tmpSet = new SymSet();
                        tmpSet.UnionWith(statementFirstSym);
                        tmpSet.Add(Type.IDSY, Type.PROCEDURE,Type.DOT);

                        isLegal(tmpSet, firstSym, 6);
                    }
                    else {
                        Data.errors.Add(5);
                    }
                }
                tmpSet = new SymSet();
                tmpSet.UnionWith(statementFirstSym);
                tmpSet.Add(Type.IDSY, Type.DOT);
                isLegal(tmpSet, declareFirstSym, 7);
            } while (declareFirstSym.Contains((Type)token.Symbol));

            pcodes.Pcodes[table.Table[symTableSize1].Address].Right = pcodes.Count;
            table.Table[symTableSize1].Address = pcodeSize1 = pcodes.Count;

            pcodes.Add(INSTRACTION.INT, 0, dataSize);//INT指令，分配dataSize大小

            tmpSet = new SymSet();
            tmpSet.UnionWith(firstSym);
            tmpSet.Add(Type.SEMI, Type.END);
            Statement(tmpSet, level, ref symTableSize);
            pcodes.Add(INSTRACTION.OPR, 0, 0);//OPR指令，释放数据

            tmpSet = new SymSet();
            isLegal(firstSym, tmpSet, 8);
        }

        void ConstDeclaration(int level, ref int symTableSize, ref int dataSize) {
            if (token.Symbol == (int)Type.IDSY) {
                Token preToken = token;
                TokenPos++;
                if (token.Symbol == (int)Type.EQUAL || token.Symbol == (int)Type.ASSIGN) {
                    if (token.Symbol == (int)Type.ASSIGN) {//将=写成:=的处理
                        Data.errors.Add(1);
                    }

                    TokenPos++;
                    if (token.Symbol == (int)Type.INTSY) {
                        table.Add(Identifier.CONSTANT, ref dataSize, level, ref symTableSize, token);
                        TokenPos++;
                    }
                    else {
                        Data.errors.Add(2);
                    }
                }
                else {
                    Data.errors.Add(3);
                }
            }
            else {
                Data.errors.Add(4);
            }
        }

        void VariableDeclaration(int level, ref int symTableSize, ref int dataSize) {
            if (token.Symbol == (int)Type.IDSY) {
                table.Add(Identifier.VARIABLE, ref dataSize, level, ref symTableSize, token);
                TokenPos++;
            }
            else {
                Data.errors.Add(4);
            }
        }

        void Statement(SymSet firstSym, int level, ref int symTableSize) {
            int i, pcodePos1, pcodePos2;
            if (token.Symbol == (int)Type.IDSY) {
                i = table.Find(token.Name, symTableSize);//查看标识符是否声明
                if (i == 0) {
                    Data.errors.Add(11);
                }
                else if (table.Table[i].Type != Identifier.VARIABLE) {
                    Data.errors.Add(12);
                    i = 0;
                }
                TokenPos++;
                if (token.Symbol == (int)Type.ASSIGN) {
                    TokenPos++;
                }
                else {
                    Data.errors.Add(13);
                }

                Expression(firstSym, level, ref symTableSize);
                if (i != 0) {
                    pcodes.Add(INSTRACTION.STO, level - table.Table[i].Level, table.Table[i].Address);//STO指令，保存数据栈顶
                }
            }
            else if (token.Symbol == (int)Type.CALL) {
                TokenPos++;
                if (token.Symbol != (int)Type.IDSY) {
                    Data.errors.Add(14);
                }
                else {
                    i = table.Find(token.Name, symTableSize);
                    if (i == 0)
                        Data.errors.Add(11);
                    else {
                        if (table.Table[i].Type == Identifier.PROCEDURE) {
                            pcodes.Add(INSTRACTION.CAL, level - table.Table[i].Level, table.Table[i].Address);
                        }
                        else {
                            Data.errors.Add(15);
                        }
                    }
                    TokenPos++;
                }
            }
            else if (token.Symbol == (int)Type.IF) {
                TokenPos++;

                SymSet tmpSet = new SymSet();
                tmpSet.UnionWith(firstSym);
                tmpSet.Add(Type.THEN, Type.DO);

                Condition(tmpSet, level, ref symTableSize);

                if (token.Symbol == (int)Type.THEN) {
                    TokenPos++;
                }
                else {
                    Data.errors.Add(16);
                }

                pcodePos1 = pcodes.Count;
                pcodes.Add(INSTRACTION.JPC, 0, 0);

                tmpSet = new SymSet();
                tmpSet.UnionWith(firstSym);
                tmpSet.Add(Type.ELSE);
                Statement(tmpSet, level, ref symTableSize);

                if (token.Symbol == (int)Type.ELSE) {
                    TokenPos++;
                    pcodePos2 = pcodes.Count;
                    pcodes.Add(INSTRACTION.JMP, 0, 0);
                    pcodes.Pcodes[pcodePos1].Right = pcodes.Count;
                    Statement(firstSym, level, ref symTableSize);
                    pcodes.Pcodes[pcodePos2].Right = pcodes.Count;
                }
                else {
                    pcodes.Pcodes[pcodePos1].Right = pcodes.Count;
                }
            }
            else if (token.Symbol == (int)Type.BEGIN) {
                TokenPos++;
                SymSet tmpSet = new SymSet();
                tmpSet.UnionWith(firstSym);
                tmpSet.Add(Type.SEMI, Type.END);
                Statement(tmpSet, level, ref symTableSize);

                SymSet tmpSet2 = new SymSet();
                tmpSet2.UnionWith(statementFirstSym);
                tmpSet2.Add(Type.SEMI);
                while (tmpSet2.Contains((Type)token.Symbol)) {
                    if (token.Symbol == (int)Type.SEMI) {
                        TokenPos++;
                    }
                    else {
                        Data.errors.Add(10);
                    }
                    if (token.Symbol == (int)Type.END) {
                        break;
                    }

                    Statement(tmpSet, level, ref symTableSize);
                }

                if (token.Symbol == (int)Type.END) {
                    TokenPos++;
                }
                else {
                    Data.errors.Add(17);
                }
            }
            else if (token.Symbol == (int)Type.WHILE) {
                pcodePos1 = pcodes.Count;
                TokenPos++;
                SymSet tmpSet = new SymSet(); ;
                tmpSet.UnionWith(firstSym);
                tmpSet.Add(Type.DO);
                Condition(tmpSet, level, ref symTableSize);
                pcodePos2 = pcodes.Count;
                pcodes.Add(INSTRACTION.JPC, 0, 0);
                if (token.Symbol == (int)Type.DO) {
                    TokenPos++;
                }
                else {
                    Data.errors.Add(18);
                }
                Statement(firstSym, level, ref symTableSize);
                pcodes.Add(INSTRACTION.JMP, 0, pcodePos1);
                pcodes.Pcodes[pcodePos2].Right = pcodes.Count;
            }
            else if (token.Symbol == (int)Type.READ) {
                TokenPos++;
                if (token.Symbol == (int)Type.LPAR) {
                    do {
                        TokenPos++;
                        if (token.Symbol == (int)Type.IDSY) {
                            i = table.Find(token.Name, symTableSize);
                            if (i == 0) {
                                Data.errors.Add(11);
                            }
                            else {
                                if (table.Table[i].Type != Identifier.VARIABLE) {
                                    Data.errors.Add(12);
                                    i = 0;
                                }
                                else {
                                    pcodes.Add(INSTRACTION.RED, level - table.Table[i].Level, table.Table[i].Address);
                                }
                            }
                        }
                        else {
                            Data.errors.Add(27);
                        }
                        TokenPos++;
                    } while (token.Symbol == (int)Type.COMMA);
                }
                else
                    Data.errors.Add(29);

                if (token.Symbol != (int)Type.RPAR) {
                    Data.errors.Add(22);
                }
                TokenPos++;
            }
            else if (token.Symbol == (int)Type.WRITE) {
                TokenPos++;
                if (token.Symbol == (int)Type.LPAR) {
                    SymSet tmpSet = new SymSet();
                    tmpSet.UnionWith(firstSym);
                    tmpSet.Add(Type.RPAR, Type.COMMA);
                    do {
                        TokenPos++;
                        Expression(tmpSet, level, ref symTableSize);
                        pcodes.Add(INSTRACTION.WRT, 0, 0);
                    } while (token.Symbol == (int)Type.COMMA);

                    if (token.Symbol != (int)Type.RPAR) {
                        Data.errors.Add(22);
                    }
                    TokenPos++;
                }
                else
                    Data.errors.Add(29);
            }
            else if (token.Symbol == (int)Type.REPEAT) {
                pcodePos1 = pcodes.Count;
                TokenPos++;
                SymSet tmpSet = new SymSet();
                tmpSet.UnionWith(firstSym);
                tmpSet.Add(Type.SEMI, Type.UNTIL);
                Statement(tmpSet, level, ref symTableSize);

                SymSet tmpSet2 = new SymSet();
                //tmpSet.UnionWith(firstSym);
                //tmpSet.Add(Type.BEGIN, Type.CALL, Type.IF, Type.WHILE, Type.SEMI);
                tmpSet2.UnionWith(statementFirstSym);
                tmpSet2.Add(Type.SEMI);

                while (tmpSet2.Contains((Type)token.Symbol)) {
                    if (token.Symbol == (int)Type.SEMI) {
                        TokenPos++;
                        if (token.Symbol == (int)Type.UNTIL) {
                            break;
                        }
                    }
                    else {
                        Data.errors.Add(5);
                    }
                    Statement(tmpSet, level, ref symTableSize);
                }
                if (token.Symbol == (int)Type.UNTIL) {
                    TokenPos++;
                    Condition(firstSym, level, ref symTableSize);
                    pcodes.Add(INSTRACTION.JPC, 0, pcodePos1);
                }
                else {
                    Data.errors.Add(25); //缺少UNTIL
                }
            }
            SymSet tmp = new SymSet();
            isLegal(firstSym, tmp, 19);
        }

        void Expression(SymSet firstSym, int level, ref int symTableSize) {
            Type addop;
            SymSet tmp = new SymSet();
            tmp.UnionWith(firstSym);
            tmp.Add(Type.PLUS, Type.MINUS);

            if (token.Symbol == (int)Type.PLUS || token.Symbol == (int)Type.MINUS) {
                addop = (Type)token.Symbol;
                TokenPos++;
                Term(tmp, level, ref symTableSize);
                if (addop == Type.MINUS) {
                    pcodes.Add(INSTRACTION.OPR, 0, 1);//负数
                }
            }
            else {
                Term(tmp, level, ref symTableSize);
            }

            while (token.Symbol == (int)Type.PLUS || token.Symbol == (int)Type.MINUS) {
                addop = (Type)token.Symbol;
                TokenPos++;
                Term(tmp, level, ref symTableSize);
                if (addop == Type.PLUS) {
                    pcodes.Add(INSTRACTION.OPR, 0, 2);
                }
                else {
                    pcodes.Add(INSTRACTION.OPR, 0, 3);
                }
            }
        }

        void Term(SymSet firstSym, int level, ref int symTableSize) {
            Type mulop;
            SymSet tmp = new SymSet();
            tmp.UnionWith(firstSym);
            tmp.Add(Type.TIMES, Type.DIVIDE);
            Factor(tmp, level, ref symTableSize);

            while (token.Symbol == (int)Type.TIMES || token.Symbol == (int)Type.DIVIDE) {
                mulop = (Type)token.Symbol;
                TokenPos++;
                Factor(tmp, level, ref symTableSize);
                if (mulop == Type.TIMES) {
                    pcodes.Add(INSTRACTION.OPR, 0, 4);
                }
                else {
                    pcodes.Add(INSTRACTION.OPR, 0, 5);
                }
            }
        }

        void Factor(SymSet firstSym, int level, ref int symTableSize) {
            int i;
            isLegal(factorFirstSym, firstSym, 24);
            while (factorFirstSym.Contains((Type)token.Symbol)) {
                if (token.Symbol == (int)Type.IDSY) {
                    i = table.Find(token.Name, symTableSize);
                    if (i == 0) {
                        Data.errors.Add(11);
                    }
                    else {
                        switch (table.Table[i].Type) {
                            case Identifier.CONSTANT:
                                pcodes.Add(INSTRACTION.LIT, 0, table.Table[i].Value);
                                break;
                            case Identifier.VARIABLE:
                                pcodes.Add(INSTRACTION.LOD, level - table.Table[i].Level, table.Table[i].Address);
                                break;
                            case Identifier.PROCEDURE:
                                Data.errors.Add(21);
                                break;
                            default:
                                break;
                        }
                    }
                    TokenPos++;
                }
                else if (token.Symbol == (int)Type.INTSY) {
                    pcodes.Add(INSTRACTION.LIT, 0, (int)token.Var);
                    TokenPos++;
                }
                else if (token.Symbol == (int)Type.LPAR) {
                    TokenPos++;
                    SymSet tmpSet = new SymSet();
                    tmpSet.UnionWith(firstSym);
                    tmpSet.Add(Type.RPAR);
                    Expression(tmpSet, level, ref symTableSize);

                    if (token.Symbol == (int)Type.RPAR) {
                        TokenPos++;
                    }
                    else {
                        Data.errors.Add(22);
                    }
                }
                SymSet tmp = new SymSet();
                tmp.UnionWith(firstSym);
                tmp.Add(Type.LPAR);
                isLegal(firstSym, tmp, 23);
            }
        }

        void Condition(SymSet firstSym, int level, ref int symTableSize) {
            Type relop;
            if (token.Symbol == (int)Type.ODD) {
                TokenPos++;
                Expression(firstSym, level, ref symTableSize);
                pcodes.Add(INSTRACTION.OPR, 0, 6);
            }
            else {
                SymSet tmp = new SymSet();
                tmp.UnionWith(firstSym); ;
                tmp.Add(Type.EQUAL, Type.NOTEQUAL, Type.LESS, Type.GREATER, Type.NOTLESS, Type.NOTGREATER);
                Expression(tmp, level, ref symTableSize);

                tmp = new SymSet();
                tmp.Add(Type.EQUAL, Type.NOTEQUAL, Type.LESS, Type.GREATER, Type.NOTLESS, Type.NOTGREATER);
                if (!tmp.Contains((Type)token.Symbol)) {
                    Data.errors.Add(20);
                }
                else {
                    relop = (Type)token.Symbol;
                    TokenPos++;
                    Expression(firstSym, level, ref symTableSize);
                    switch (relop) {
                        case Type.EQUAL:
                            pcodes.Add(INSTRACTION.OPR, 0, 8);
                            break;
                        case Type.NOTEQUAL:
                            pcodes.Add(INSTRACTION.OPR, 0, 9);
                            break;
                        case Type.LESS:
                            pcodes.Add(INSTRACTION.OPR, 0, 10);
                            break;
                        case Type.NOTLESS:
                            pcodes.Add(INSTRACTION.OPR, 0, 11);
                            break;
                        case Type.GREATER:
                            pcodes.Add(INSTRACTION.OPR, 0, 12);
                            break;
                        case Type.NOTGREATER:
                            pcodes.Add(INSTRACTION.OPR, 0, 13);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

    }
}
