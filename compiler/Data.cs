using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler {
    public enum Type : int {
        CONST = 1, IF, THEN, ELSE, WHILE, DO, CALL, BEGIN, END, REPEAT, UNTIL, READ, WRITE, PROCEDURE, VAR, ODD,//保留字
        PLUS, MINUS,
        TIMES, DIVIDE,
        GREATER, LESS, NOTLESS, NOTGREATER, EQUAL, NOTEQUAL,
        ASSIGN, //运算符
        LPAR, RPAR, COMMA, SEMI, QUOTE, DOT,  //分界符
        INTSY,//常量
        IDSY,//标识符
    }

    //Pcode指令
    enum INSTRACTION : int {
        LIT, OPR, LOD, STO, CAL, INT, JMP, JPC, RED, WRT,
    }
	
    //符号表项类型
    enum Identifier : int {
        CONSTANT, VARIABLE, PROCEDURE
    }

    class Data {

        public static ErrorList errors;

        //符号表最大长度
        public static int MaxSymSize = 100;
		//运行Pcode最大次数
        public static int MaxProcessTimes = 1000000;
        //解释器栈深度
        public static int MaxStackSize = 1000;
        public static int MaxNum = Int32.MaxValue;
		//错误显示个数
        public static int ErrorNum = 30;

        public static int MaxPcodeSize = 200;
        public static int MaxAddressSize = 2047;
		//嵌套最大层数
        public static int MaxLevel = 3;


        public static int lineNum;
        public static string token;
        public static string varName;

        public static bool LexicalSuccess;
        public static bool ParserSuccess;

    }
}
