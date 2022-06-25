using System;
using System.Linq;
using System.Collections.Generic;
//Описание простых переменных(+). Описание массивов(+). Синтаксический анализ
//выражения(+). Операторы: условный(+), выбора(+), присваивания(+), составной(+).
namespace Компилятор
{
    class SyntaxisAnalyzer
    {
        static ListWithCodes allCodes = new ListWithCodes();
        static HashSet<byte> followers = new HashSet<byte>();
        static bool ifStart = true;
        public static int count = 0;
        static bool ifFlag = false;
        static int beginEndCount = 0;
        static public bool belong(byte symbol, HashSet<byte> st)
        {
            return st.Contains(symbol);
        }
        static void union(HashSet<byte> st1, HashSet<byte> st2)
        {
            st1.UnionWith(st2); // объединение 2 и 1
        }
        static void accept(byte symbolexpected, string errMassage = "") // соответсвие символу
        { 
            if (LexicalAnalyzer.symbol == symbolexpected) LexicalAnalyzer.NextSym(); // если символ существует и равен
            else if (errMassage != "") InputOutput.Error(symbolexpected, LexicalAnalyzer.token, errMassage);
            else InputOutput.Error(symbolexpected, LexicalAnalyzer.token);
        }
        static void maybeSkip(HashSet<byte> starters, HashSet<byte> finisher, string errMassage)
        {
            if (!belong(LexicalAnalyzer.symbol, starters))
            {
                InputOutput.Error(LexicalAnalyzer.symbol, LexicalAnalyzer.token, errMassage);
                skipto2(starters, finisher);
            }
        }
        static void skipto2(HashSet<byte> startes, HashSet<byte> followers)
        {
            while (!belong(LexicalAnalyzer.symbol, startes) && !belong(LexicalAnalyzer.symbol, followers)) // скипаем пока не нашелся в старте или в внешних символах
            {
                LexicalAnalyzer.NextSym();
            }
        }
        static public void StartSyntaxisAnalyzer() // на старт, внимание, умер 
        {
            if (ifStart) // проверка правельности начала 
            {
                SemanticAnalyzer.TreeNode temp = null; // добавляем новую обасть видимости новую голову
                SemanticAnalyzer.heads.Push(temp);
                LexicalAnalyzer.NextSym();
                HashSet<byte> ptra = new HashSet<byte>();
                union(followers, allCodes.begpart);                // add "var", "function" and the rest in followers
                followers.Add(LexicalAnalyzer.point);                   // add point in followers
                
                ptra.Add(LexicalAnalyzer.programsy);                    // add "program" in ptra
                maybeSkip(ptra, followers, "Ошибка! Ожидался Program"); // try find "program" else go to var or end with point 
                if (belong(LexicalAnalyzer.symbol, ptra)){              // if find "program"
                    accept(LexicalAnalyzer.programsy, "if you see this, then you are sweet"); // just next char, if you see error then this is not good
                    accept(LexicalAnalyzer.ident, "Ошибка! Ожидался идентификатор");
                    accept(LexicalAnalyzer.semicolon, "Ошибка! Ожидался символ \";\"");
                }
                ifStart = false;    // we dont return to this block
            }
            else                    // все, что не начало
            {
                block();
                if(!ifFlag) LexicalAnalyzer.NextSym();
                ifFlag = false;
            }
        }
        static public void block()
        {
            varpart();
            followers.Add(LexicalAnalyzer.thensy);      // обновили followers \ содержит на что заканчивается выражения в if
            followers.Add(LexicalAnalyzer.semicolon);   // обновили followers \ содержит на что заканчивается выражения (;)
            union(followers, allCodes.all_positions_for_skip); // add all for live
            ifstatement();
            equal();
            beginEnd();
            caseStatmant();
        }
        static void varpart() // описсание простых переменных 
        {
            if (LexicalAnalyzer.symbol == LexicalAnalyzer.varsy) // если var
            {
                ifFlag = true;
                HashSet<byte> ptra = new HashSet<byte>();
                ptra.Add(LexicalAnalyzer.ident);// add ident in ptra

                accept(LexicalAnalyzer.varsy);  // некст символ
                maybeSkip(ptra, followers, "Ошибка! в блоке описания"); // try find ident else go to begin or point 
                while (LexicalAnalyzer.symbol == LexicalAnalyzer.ident) // пока индитификатор
                {
                    vardeclaration();
                    foreach (int hashCod in SemanticAnalyzer.names) // все идентификаторы добавляются в свою обасть видимости
                    {
                        SemanticAnalyzer.newIdent(hashCod);
                    }
                    SemanticAnalyzer.names.Clear(); // все идентификаторы в строке обработаны
                    accept(LexicalAnalyzer.semicolon, "Ошибка, ожидался символ \";\""); // ;
                }
            }
        }
        static void vardeclaration() // перебор всех переменных однотипных
        {
            accept(LexicalAnalyzer.ident); // некст символ
            SemanticAnalyzer.names.Add(SemanticAnalyzer.name);
            while (LexicalAnalyzer.symbol == LexicalAnalyzer.comma) // пока запятушка
            {
                LexicalAnalyzer.NextSym(); // некст символ
                SemanticAnalyzer.names.Add(SemanticAnalyzer.name); // добавляем новый илентификатор для добавления в дерево
                accept(LexicalAnalyzer.ident, "Ошибка! Ожидался идентификатор"); // если индентификатор
            }
            accept(LexicalAnalyzer.colon, "Ошибка! Ожидался символ \":\""); // :
            if (LexicalAnalyzer.symbol == LexicalAnalyzer.arraysy)
            {
                SemanticAnalyzer.tempIdTypeArray = SemanticAnalyzer.arrays; // тип идентификаторов массив
                arraytype(); // массив?
            }
            else type(); // не массив?
        }
        static void type() // простые типы
        {
            if (!allCodes.Types.Contains(LexicalAnalyzer.symbol))
                accept(LexicalAnalyzer.integersy, "Ошибка! Ожидался Тип"); // ew dont find type
            else
            {
                SemanticAnalyzer.tempIdType = SemanticAnalyzer.scalars;
                LexicalAnalyzer.NextSym();
            }
        }
        static void arraytype() // массивы
        {
            accept(LexicalAnalyzer.arraysy); // некст сивол
            accept(LexicalAnalyzer.lbracket, "Ошибка! Ожидался символ \"[\""); // [
            simpletype();
            while (LexicalAnalyzer.symbol == LexicalAnalyzer.comma) // пока запятая
            {
                accept(LexicalAnalyzer.comma); // некст символ
                simpletype();
            }
            accept(LexicalAnalyzer.rbracket, "Ошибка! Ожидался символ \"]\""); // ]
            accept(LexicalAnalyzer.ofsy, "Ошибка! Ожидался \"of\""); // of
            type();
        }
        static void simpletype() // значения
        {
            if (!allCodes.Values.Contains(LexicalAnalyzer.symbol)) accept(LexicalAnalyzer.intc, "Ошибка! Ожидалось значение");
            else LexicalAnalyzer.NextSym();
        }
        static void ifstatement() // условный оператор
        {
            if (LexicalAnalyzer.symbol == LexicalAnalyzer.ifsy)
            {
                ifFlag = true;
                accept(LexicalAnalyzer.ifsy);
                expression();

                accept(LexicalAnalyzer.thensy, "Ошибка! Ожидался \"then\"");
                equal();

                if (LexicalAnalyzer.symbol == LexicalAnalyzer.elsesy)
                {
                    accept(LexicalAnalyzer.elsesy);
                    ifstatement();
                    equal();
                }
                else block(); // если это не else, то чтобы не потерять строку рекурсивно запускаем
            }
        }
        static void expression() // выражение
        {
            maybeSkip(allCodes.st_expression, followers, "Ошибка! В выражении"); // идем до then, ; или до чего-то еще

            if (belong(LexicalAnalyzer.symbol, allCodes.st_expression))
            {
                if ( LexicalAnalyzer.symbol == LexicalAnalyzer.ident 
                    || allCodes.Values.Contains(LexicalAnalyzer.symbol) ) // это идентификатор или значение
                {
                    if(LexicalAnalyzer.symbol == LexicalAnalyzer.ident)
                    {
                        if(SemanticAnalyzer.searchIdent(SemanticAnalyzer.name) is null)
                            accept(LexicalAnalyzer.varsy, "Ошибка! Идентификатор не определен");
                        GeneratorCodes.push_reference(LexicalAnalyzer.integersy, 0, 017,
                            (ulong)(SemanticAnalyzer.searchIdent(SemanticAnalyzer.name)).ofSet);
                    }
                    count++;
                    LexicalAnalyzer.NextSym(); // некст символ
                    
                    while (allCodes.Expressions.Contains(LexicalAnalyzer.symbol)) // пока опреатор
                    {
                        LexicalAnalyzer.NextSym();
                        if (LexicalAnalyzer.symbol == LexicalAnalyzer.ident
                            || allCodes.Values.Contains(LexicalAnalyzer.symbol))
                        { // это идентификатор или значение
                            if (LexicalAnalyzer.symbol == LexicalAnalyzer.ident)
                            {
                                if (SemanticAnalyzer.searchIdent(SemanticAnalyzer.name) is null)
                                    accept(LexicalAnalyzer.varsy, "Ошибка! Идентификатор не определен");
                                GeneratorCodes.push_reference(LexicalAnalyzer.integersy, 0, 017,
                                (ulong)(SemanticAnalyzer.searchIdent(SemanticAnalyzer.name)).ofSet);
                                GeneratorCodes.multop((ulong)LexicalAnalyzer.star, LexicalAnalyzer.integersy);
                            }
                            LexicalAnalyzer.NextSym();
                        }
                        else
                        {
                            accept(LexicalAnalyzer.ident, "Ошибка! Ожидался идентификатор или значение");
                            break;
                        }
                    }
                }
            }
        }
        static void equal() // присваивание
        {
            if (LexicalAnalyzer.symbol == LexicalAnalyzer.ident) // это идентификатор
            {
                if (SemanticAnalyzer.searchIdent(SemanticAnalyzer.name) is null)
                        accept(LexicalAnalyzer.varsy, "Ошибка! Идентификатор не определен");

                ifFlag = true;
                LexicalAnalyzer.NextSym(); // некст символ
                if (allCodes.OperatorEquals.Contains(LexicalAnalyzer.symbol))
                {
                    LexicalAnalyzer.NextSym(); // некст символ
                    expression();
                    accept(LexicalAnalyzer.semicolon, "Ошибка! Ожидался сисвол \";\"");
                }
                else
                {
                    InputOutput.Error(LexicalAnalyzer.symbol, LexicalAnalyzer.token, "Ошибка! Ожидалось присваивание");
                }
            }
        }
        static void beginEnd() // составной 
        {
            if (LexicalAnalyzer.beginsy == LexicalAnalyzer.symbol)
            {
                ifFlag = true;
                beginEndCount++;
                LexicalAnalyzer.NextSym();
            }
            else if (LexicalAnalyzer.endsy == LexicalAnalyzer.symbol)
            {
                ifFlag = true;
                beginEndCount--;
                LexicalAnalyzer.NextSym();
                if (beginEndCount != 0) accept(LexicalAnalyzer.semicolon, "Ошибка! Ожидался символ \";\"");
                else accept(LexicalAnalyzer.point, "Ошибка! Ожидался символ \".\"");
            }
        }
        static public void endEnd() // проверка на последний end
        {
            if (beginEndCount != 0) accept(0, "Ошибка! Ожидался end");
        } 
        static void caseStatmant() // case
        {
            if (LexicalAnalyzer.symbol == LexicalAnalyzer.casesy)
            {
                ifFlag = true;
                beginEndCount++; // у кейса должен быть end;
                LexicalAnalyzer.NextSym(); // некст символ
                expression();
                accept(LexicalAnalyzer.ofsy, "Ошибка! Ожидался \"of\"");

                do
                {
                    if (LexicalAnalyzer.symbol == LexicalAnalyzer.ident
                        || allCodes.Values.Contains(LexicalAnalyzer.symbol))
                    { // это идентификатор или значение
                        if (LexicalAnalyzer.symbol == LexicalAnalyzer.ident)
                        {
                            if (SemanticAnalyzer.searchIdent(SemanticAnalyzer.name) is null)
                                accept(LexicalAnalyzer.varsy, "Ошибка! Идентификатор не определен");
                        }
                        LexicalAnalyzer.NextSym(); // некст символ
                    }
                    else
                    {
                        accept(LexicalAnalyzer.ident, "Ошибка! Ожидался идентификатор или значение");
                        break;
                    }
                    accept(LexicalAnalyzer.colon, "Ошибка! Ожидался символ \":\"");
                    equal();

                } while (!(LexicalAnalyzer.symbol == LexicalAnalyzer.elsesy 
                            || LexicalAnalyzer.symbol == LexicalAnalyzer.endsy)); // пока не else или end

                if (LexicalAnalyzer.symbol == LexicalAnalyzer.elsesy) // может быть else
                {
                    accept(LexicalAnalyzer.elsesy);
                    equal();
                }
            }
        }
    }
}
