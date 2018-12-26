using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler {
    class Token {
        string name;
        int symbol;
        object var;
        int line;

        public int Line { get => line; set => line = value; }
        public object Var { get => var; set => var = value; }
        public int Symbol { get => symbol; set => symbol = value; }
        public string Name { get => name; set => name = value; }

        public Token(string name, int symbol, int line) {
            this.name = name;
            this.symbol = symbol;
            this.line = line;
        }

        public Token() {
            name = null;
            symbol = 0;
            var = null;
            line = 0;
        }

        public Token(string name, int symbol, object var, int line) {
            this.name = name;
            this.symbol = symbol;
            this.var = var;
            this.line = line;
        }
    }
}
