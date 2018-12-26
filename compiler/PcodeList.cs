using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler {
    class Pcode {
        public static int sum = 0;
        int num;
        INSTRACTION ins;
        int left;
        int right;

        public Pcode(INSTRACTION ins, int left, int right) {
            Pcode.sum++;
            num = sum;

            Ins = ins;
            Left = left;
            Right = right;
        }

        public int Left { get => left; set => left = value; }
        public int Right { get => right; set => right = value; }
        internal INSTRACTION Ins { get => ins; set => ins = value; }

        public int Num { get => num; }
        public string INST { get => ins.ToString(); }
    }

    class PcodeList {
        private List<Pcode> pcodes;

        public PcodeList() {
            Pcodes = new List<Pcode>();
        }

        public void Add(INSTRACTION ins, int left, int right) {
            if (right > Data.MaxAddressSize) {
                Data.errors.Add(26);
                throw new Exception();
            }
            Pcodes.Add(new Pcode(ins, left, right));
        }

        public int Count { get => Pcodes.Count; }
        internal List<Pcode> Pcodes { get => pcodes; set => pcodes = value; }
    }
}
