using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler {
    class Interpreter {
        public bool Interpret(PcodeList pcodes,string[] input,out string output) {
            bool success = true;
            int pc = 0, bp = 0, sp = 0,readPos = 0;
            int runTimes = 0;
            output = "";
            int[] stack = new int[Data.MaxStackSize];
            stack[0] = 0;
            do {
                Pcode currentCode = pcodes.Pcodes[pc++];
                switch (currentCode.Ins) {
                    case INSTRACTION.LIT:
                        stack[sp++] = currentCode.Right;
                        break;
                    case INSTRACTION.OPR:
                        switch (currentCode.Right) {
                            case 0:
                                sp = bp;
                                pc = stack[sp + 2];
                                bp = stack[sp + 1];
                                break;
                            case 1:
                                stack[sp - 1] = -stack[sp - 1];
                                break;
                            case 2:
                                sp--;
                                stack[sp - 1] += stack[sp];
                                break;
                            case 3:
                                sp--;
                                stack[sp - 1] -= stack[sp];
                                break;
                            case 4:
                                sp--;
                                stack[sp - 1] = stack[sp - 1] * stack[sp];
                                break;
                            case 5:
                                sp--;
                                stack[sp - 1] /= stack[sp];
                                break;
                            case 6:
                                stack[sp - 1] %= 2;
                                break;
                            case 7:
                                sp--;
                                stack[sp - 1] %= stack[sp];
                                break;
                            case 8:
                                sp--;
                                stack[sp - 1] = (stack[sp] == stack[sp - 1] ? 1 : 0);
                                break;
                            case 9:
                                sp--;
                                stack[sp - 1] = (stack[sp] != stack[sp - 1] ? 1 : 0);
                                break;
                            case 10:
                                sp--;
                                stack[sp - 1] = (stack[sp - 1] < stack[sp] ? 1 : 0);
                                break;
                            case 11:
                                sp--;
                                stack[sp - 1] = (stack[sp - 1] >= stack[sp] ? 1 : 0);
                                break;
                            case 12:
                                sp--;
                                stack[sp - 1] = (stack[sp - 1] > stack[sp] ? 1 : 0);
                                break;
                            case 13:
                                sp--;
                                stack[sp - 1] = (stack[sp - 1] <= stack[sp] ? 1 : 0);
                                break;
                        }
                        break;
                    case INSTRACTION.LOD:
                        stack[sp] = stack[Base(currentCode.Left, stack, bp) + currentCode.Right];
                        sp++;
                        break;
                    case INSTRACTION.STO:
                        sp--;
                        stack[Base(currentCode.Left, stack, bp) + currentCode.Right] = stack[sp];
                        break;
                    case INSTRACTION.CAL:
                        stack[sp] = Base(currentCode.Left, stack, bp);
                        stack[sp + 1] = bp;
                        stack[sp + 2] = pc;
                        bp = sp;
                        pc = currentCode.Right;
                        break;
                    case INSTRACTION.INT:
                        sp += currentCode.Right;
                        break;
                    case INSTRACTION.JMP:
                        pc = currentCode.Right;
                        break;
                    case INSTRACTION.JPC:
                        sp--;
                        if (stack[sp] == 0) {
                            pc = currentCode.Right;
                        }
                        break;
                    case INSTRACTION.WRT:
                        output += stack[sp - 1].ToString();
                        output += Environment.NewLine;
                        sp--;
                        break;
                    case INSTRACTION.RED:
                        int tmp = 0;
                        try {
                            tmp = int.Parse(input[readPos++]);
                        }
                        catch {
                            success = false;
                        }
                        stack[sp] = tmp;
                        stack[Base(currentCode.Left, stack, bp) + currentCode.Right] = stack[sp];
                        break;
                }
                runTimes++;
                if (runTimes > Data.MaxProcessTimes) {
                    throw new OutOfMemoryException();
                }
            } while (pc != 0);
            return success;
        }

        private int Base(int level, int[] stack, int bp) {
            while (level > 0) {
                bp = stack[bp];
                level--;
            }
            return bp;
        }
        
    }
}
