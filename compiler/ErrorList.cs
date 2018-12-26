using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler {
    public class Error : IComparable<Error> {
        int line;
        int code;
        string detail;

        public Error(int line, int code, string detail) {
            this.line = line;
            this.code = code;
            this.detail = detail;
        }

        public int Line { get => line; set => line = value; }
        public int ErrorCode { get => code; }
        public string ErrorInfo { get => Error.GetInfo(code); }
        public string Detail { get => detail; set => detail = value; }

        public static string GetInfo(int code) {
            return errorMsg[code];
        }

        public int CompareTo(Error e) {
            if(Line == e.Line) {
                return 0;
            }
            return (Line > e.Line ? 1 : -1);
        }

        public static readonly string[] errorMsg = {//参考课本P316
            "",
            "常数声明时应该用'='而不是':='",
            "常数声明时'='后应为数字",
            "常数声明时标识符后应为'='",
            "const,var,procedure后应为标识符",
            "漏掉逗号或分号",
            "过程说明后的符号不正确",
            "应为语句开始符号",
            "程序体内语句部分后的符号不正确",
            "程序结尾应为句号",
            "语句之间漏了分号",
            "标识符未说明",
            "不可向常量或过程赋值",
            "赋值语句中应为赋值运算符':='",
            "call后应为标识符",
            "call后标识符属性应为过程,不可调用常量或变量",
            "条件语句中缺失then",
            "语句结束时应为分号或end",
            "while型循环语句中缺失do",
            "语句后的符号不正确",
            "应为关系运算符",
            "表达式内不可有过程标识符",
            "缺失右括号",
            "因子后不可为此符号",
            "表达式不能以此符号开始",
            "repeat循环中缺失until",
            "地址池过大",
            "Read语句括号内不是标识符",
            "这个数太大",
            "缺失左括号",
            "不存在此运算符",
            "非法的字符输入",
            "过程嵌套次数过大",
            "结束符应在程序末尾出现"
        };
        
    }

    class ErrorList {
        List<Error> errors;

        public ErrorList() {
            errors = new List<Error>();
        }

        public List<Error> Errors { get => errors; set => errors = value; }

        public void Add(int errorCode) {
            //防止重复错误
            try {
                Error err = errors.Find(x => x.Line == Data.lineNum);
                if(err.Detail == Data.token && err.ErrorCode == errorCode) {
                    return;
                }
            }
            catch { }
            
            errors.Add(new Error(Data.lineNum, errorCode, Data.token));
        }

        public void Clear() {
            errors.Clear();
        }

        public List<Error> GetErrors(int num) {
            return errors.Take(num).ToList<Error>();
        }
    }
}
