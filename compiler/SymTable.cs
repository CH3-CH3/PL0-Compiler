using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler {
    class SymTab {
        string name;
        Identifier type;
        int value;
        int level;
        int address;

        public SymTab() {
            this.name = null;
            this.type = 0;
            this.value = 0;
            this.level = 0;
            this.address = 0;
        }

        public SymTab(string name, Identifier type, int value, int level, int address) {
            this.name = name;
            this.type = type;
            this.value = value;
            this.level = level;
            this.address = address;
        }

        public string Name { get => name; set => name = value; }
        public Identifier Type { get => type; set => type = value; }
        public int Value { get => value; set => this.value = value; }
        public int Level { get => level; set => level = value; }
        public int Address { get => address; set => address = value; }
    }

    class SymTable {
        SymTab[] table;

        internal SymTab[] Table { get => table; set => table = value; }

        public SymTable() {
            table = new SymTab[Data.MaxSymSize];
            for(int i = 0; i < Data.MaxSymSize;i++) {
                table[i] = new SymTab();
            }
        }

        public SymTab[] GetLegalSymTabs() {
            int i = 0;
            for( ; i < Data.MaxSymSize; i++) {
                if (table[i].Name == null) {
                    break;
                }
            }
            SymTab[] tabs = new SymTab[i];
            Array.Copy(table, 0, tabs, 0, i);
            return tabs;
        }

        /// <summary>
        /// 新增符号表项
        /// </summary>
        /// <param name="k">标识符的种类</param>
        /// <param name="dx">变量相对地址</param>
        /// <param name="lev">标识符所在层次</param>
        /// <param name="tx">符号表尾指针</param>
        /// <param name="token">符号</param>
        public void Add(Identifier k, ref int dx, int lev, ref int tx, Token token) {
            tx++;
            table[tx].Name = Data.varName;
            table[tx].Type = k;
            switch (k) {
                case Identifier.CONSTANT:
                    table[tx].Value = (int)token.Var;
                    break;
                case Identifier.VARIABLE:
                    table[tx].Level = lev;
                    table[tx].Address = dx++;
                    break;
                case Identifier.PROCEDURE:
                    table[tx].Level = lev;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 倒序查找标识符
        /// </summary>
        /// <param name="_name">标识符名</param>
        /// <param name="tx">符号表尾</param>
        /// <returns>返回标识符位置，失败则返回0。</returns>
        public int Find(string _name,int tx) {
            table[0].Name = _name;
            while (table[tx].Name != _name) {
                tx--;
            }
            return tx;
        }
    }
}
