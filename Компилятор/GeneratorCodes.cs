using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Компилятор
{
    class GeneratorCodes
    {
        static BinaryWriter writer = new BinaryWriter(File.Open("code.dat", FileMode.OpenOrCreate));
        static StreamWriter a = new StreamWriter("a.txt");

        public static void write_2bytecom(ulong byte2)
        {
            writer.Write(byte2);
            Console.WriteLine(byte2);
        }

        public static void CP_operation(ulong code, ulong regnum1, ulong regnum2)//команда для C и P регистров
        {
            ulong byte2; //формируемая команда
            Console.WriteLine(code + " " + regnum1 + " " + regnum2);
            byte2 = (code << 9) + (regnum1 << 7) + regnum2;
            write_2bytecom(byte2); //запись команды
        }

        //Пересылка непосредственного данного на регистры
        public static void mov_RC(
            ulong numregs,/* КОЛ */
            ulong regtype,/* поле ТО — тип регистра */
            ulong regnum,/* поле ОП — номер первогорегистра */
            ulong constant /* непосредственное данное */
        )
        {
            ulong hw /* формируемая команда */;
            /* формирование командного полуслова */
            hw = (0x88ul << 24) + (numregs << 15) +
            (0x0A << 10) + (regtype << 8) + regnum;
            /* запись командного полуслова в объектный модуль */
            //write_halfword(hw);
            write_2bytecom(hw);
            /* запись операндного полуслова в объектный модуль */
            //write_halfword(constant);
            write_2bytecom(constant);
        }

        //Пересылка данных между регистрами
        public static void mov_RM(
            ulong numregs,/* КОЛ */
            ulong opcode,/* код операции */
            ulong regtype,/* поле ТО — тип регистра */
            ulong regnum,/* номер первого регистра группы */
            ulong dreg,/* номер дисплей-регистра */
            ulong ireg,/* номер индекса */
            ulong offset /* смещение */
        )
        {
            ulong
            com_hw /* командное полуслово */,
            data_hw /* операндное полуслово */,
            help1, help2 /* вспомогательные переменные для формирования смещения */;
            /* формирование командного полуслова */
            com_hw = (0x88ul << 24) + (numregs << 15) + (opcode << 10) + (regtype << 8) + regnum;
            /* формирование операндного полуслова (см. 8.3) */
            help1 = (offset / 32768ul) & 0xFFul;
            help2 = offset & 0x7FFFul;
            data_hw = (help1 << 24) + (ireg << 20) + (dreg << 15) + help2;
            /* запись команды в объектный модуль */
            //write_halfword(com_hw);
            write_2bytecom(com_hw);
            //write_halfword(data_hw);
            write_2bytecom(data_hw);
        }

        /* Переход с сохранением адреса возврата */
        public static void branch(
            ulong code, /* код операции */
            ulong inum, /* номер И-регистра */
            ulong offset /* полусловное смещение - ПСМ */)
        {
            ulong hw;
            /* заполнение командного полуслова: */
            hw = (code << 25) + (inum << 20) + offset;
            /* запись команды в объектный модуль */
            //write_halfword(hw);
            write_2bytecom(hw);
        }

        /* Условный переход по состоянию С-регистра*/
        public static void branch_on_condition(
            ulong cnum,/* номер С-регистра */
            ulong condition,/* номер условия */
            ulong offset /* полусловное смещение ПСМ */
        )
        {
            ulong hw;
            /* заполнение командного полуслова: */
            hw = (0x54ul << 25) + (cnum << 23) +
            (condition << 20) + offset;
            /* запись команды в объектный модуль */
            //write_halfword(hw);
            write_2bytecom(hw);
        }


        static Stack<StackOperand> opstack = new Stack<StackOperand>();
        /* запись в СО информации о локальной переменной или параметре - значении */
        public static void push_reference(
            byte optype, /* указатель на дескриптор типа операнда */
            ulong dreg,/* номер Д-регистра, на котором хранится адрес области данных */
            ulong ireg,/* номер И-регистра, на котором хранится динамическое смещение в области данных */
            ulong memoff /* смещение переменной в области данных */
        )
        {
            StackOperand temp = new StackOperand();
            temp.loctype = StackOperand.REFERENCE;
            temp.optype = optype;
            temp.opsize = sizeof(byte);
            temp.memloc.dreg = dreg;
            temp.memloc.ireg = ireg;
            temp.memloc.memoff = memoff;
            opstack.Push(temp);
        }

        /* запись в СО информации об операнде, значение которого хранится на регистре */
        public static void push_register(
            byte optype /* указатель на дескриптор типа операнда */,
            string whatreg /* тип регистра */,
            ulong regnum /* номер регистра */)
        {
            StackOperand temp = new StackOperand();
            temp.loctype = StackOperand.REFERENCE;
            temp.optype = optype;
            temp.opsize = sizeof(byte);
            temp.opreg.whatreg = whatreg;
            temp.opreg.regnum = regnum;
            opstack.Push(temp);
        }


        public static string type_analyze()
        {
            StackOperand temp1 = opstack.Pop();
            StackOperand temp2 = opstack.Pop();
            if (temp1.optype == temp2.optype)
            {
                return temp1.optype == LexicalAnalyzer.integersy ?
                    "INT_INT" : "SET_SET";
            }
            else
            {
                    return "INT_REAL";
            }
        }
        static ulong pCount = 4;
        static ulong cCount = 0;
        public static ulong op_analyze(string kindreg)
        {
            if (kindreg == "C_REG" && cCount < 4)
            {
                return cCount++;
            }
            if (kindreg == "P_REG" && pCount < 128)
            {
                return pCount++;
            }
            return 129;
        }

        public static void multop(
            ulong operation,/* код операции */
            byte exptype /* указатель на дескриптортипа результата операции */
        )
        {
            string optypes = type_analyze();
            ulong reg_c1 = op_analyze("C_REG"), /* номер С-регистра */
                  reg_c2,/* номер С-регистра */
                  reg_p = op_analyze("P_REG"); /* номер Р-регистра */
            switch (operation)
            {
                case LexicalAnalyzer.andsy:
                    CP_operation(047, reg_c1, reg_p);
                    break;
                case LexicalAnalyzer.star:
                    switch (optypes)
                    {
                        case "INT_INT":
                            CP_operation(031, reg_c1, reg_p);
                            break;
                        case "INT_REAL":
                            reg_c2 = get_C_reg(); /* запрос С-регистра */
                            CP_operation(013, reg_c2, reg_p);
                            CP_operation(005, reg_c1, reg_c2);
                            free_C_reg(reg_c2); /* освобождение С-регистра */
                            break;
                        case "REAL_INT":
                            CP_operation(013, reg_c1, reg_c1);
                            break;
                        case "REAL_REAL":
                            CP_operation(005, reg_c1, reg_p);
                            break;
                    }
                    break;
                default:
                    break;
            }
            push_register(exptype, "C_REG", reg_c1);
            free_P_reg(reg_p); /* освобождение Р-регистра */
        }

        public static ulong get_C_reg()
        {
            if (cCount < 4)
            {
                return cCount++;
            }
            else
            {
                return 129;
            }
        }

        public static void free_C_reg(ulong reg)
        {
            cCount--;
        }

        public static void free_P_reg(ulong reg)
        {
            pCount--;
        }
    }
}
