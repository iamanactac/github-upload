using System;
using System.Collections.Generic;
using System.IO;

namespace Компилятор
{
    struct TextPosition
    {
        public uint lineNumber; // номер строки
        public byte charNumber; // номер позиции в строке

        public TextPosition(uint ln = 0, byte c = 0)
        {
            lineNumber = ln;
            charNumber = c;
        }
    }

    struct Err
    {
        public TextPosition errorPosition;
        public byte errorCode;
        public string errorMassage;
        public Err(TextPosition errorPosition, byte errorCode)
        {
            this.errorPosition = errorPosition;
            this.errorCode = errorCode;
            errorMassage = "";
        }
    }


    class InputOutput
    {
        const byte ERRMAX = 9;
        public static char Ch { get; set; }
        static bool endFlag = true;
        public static TextPosition positionNow = new TextPosition();
        static string line = "";
        static string errStr = "0";
        public static byte lastInLine = 0;
        public static List<Err> err;
        static public StreamReader File { get; set; }
        static public StreamReader errFile { get; set; }
        static uint errCount = 0;
        public static List<Err> allErr = new List<Err>();
        public static string symbolString = "";

        public InputOutput()
        {
            File = new StreamReader("D:\\ВУЗ\\8трим\\ЯП\\Компилятор\\code.txt");
            //errFile = new StreamReader("D:\\ВУЗ\\8трим\\ЯП\\Компилятор\\err.txt");
        }
        public void AllCh()
        {
            ReadNextLine();
            NextCh();
            while (endFlag)
            {
                SyntaxisAnalyzer.StartSyntaxisAnalyzer(); // запуск синаксического анализатора от строки
            }
            SyntaxisAnalyzer.endEnd();
            if (err.Count > 0)
                ListErrors();
            Console.WriteLine($"Компиляция завершена: : ошибок — {errCount}!");
            line = " ";
            positionNow.charNumber = 0;
        }
        static void ifEndLine()
        {
            if (positionNow.charNumber == lastInLine)
            {
                ListThisLine();
                if (err.Count > 0)
                    ListErrors();
                symbolString = "";
                positionNow.lineNumber++;
                ReadNextLine();
                positionNow.charNumber = 0;

            }
        }
        public static void NextCh()
        {
            ifEndLine();
            if (positionNow.charNumber < lastInLine)
            {
                Ch = line[positionNow.charNumber];
                ++positionNow.charNumber;
            }
        }
        private static void ListThisLine()
        {
            Console.WriteLine(line);
        }
        private static void ReadNextLine()
        {
            err = new List<Err>();
            if (!File.EndOfStream)
            {
                line = File.ReadLine();
                lastInLine = (byte)(line.Length);
                if (lastInLine == 1) ReadNextLine();
            }
            else
            {
                End();
            }
        }
        static public void End()
        {
            endFlag = false;
        }
        static void ListErrors()
        {
            int pos = 0;
            string s;
            foreach (Err item in err)
            {
                pos = item.errorPosition.charNumber - 1;
                ++errCount;
                s = "";
                //while (s.Length < pos) s += " ";
                for (int i = 0; i < 5; i++) s += "_";
                if (item.errorMassage == "") s += $"ошибка код {item.errorCode}";
                else s += item.errorMassage;
                s += " **";
                if (errCount < 10) s += "0";
                s += $"{errCount}**";
                for (int i = 0; i < 5; i++) s += "_";
                Console.WriteLine(s);
            }
        }
        static public void Error(byte errorCode, TextPosition position, String errMassage = "")
        {
            Err e;
            if (err.Count <= ERRMAX)
            {
                e = new Err(position, errorCode);
                if (errMassage != "") e.errorMassage = errMassage;
                err.Add(e);
            }
        }
    }
}
