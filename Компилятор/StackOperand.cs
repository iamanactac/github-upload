using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Компилятор
{
    class StackOperand
    {
        public const byte
            REFERENCE = 1,//локальная переменная или параметр-значение
            REGISTER = 2,//регистр
            CONSTANT = 3,//непосредственное данное
            REF_VAR = 4;// переменная, переданная по ссылке

        public ulong loctype;//класс операнда
        public byte optype;//тип операнда
        public ulong opsize;//размер операнда в словах
        //Регистр
        public class Opreg
        {
            public string whatreg; //тип регистра
            public ulong regnum; //номер регистра
        }
        public Opreg opreg = new Opreg();
        // Локальная переменная или параметр-значение
        public class Memloc
        {
            public ulong dreg;//номер Д-регистра
            public ulong ireg;//номер И-регистра
            public ulong memoff;// смещение в области данных
        }
        public Memloc memloc = new Memloc();
        //Переменная, переданная по ссылке
        public ulong regnum;//номер Р-регистра, содержащего адрес фактического параметра
    }
}
