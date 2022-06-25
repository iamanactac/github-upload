using System;
namespace Компилятор
{
    class LexicalAnalyzer
    {
        public const byte
            star = 21, // *
            slash = 60, // /
            equal = 16, // =
            comma = 20, // ,
            semicolon = 14, // ;
            colon = 5, // :
            point = 61,	// .
            arrow = 62,	// ^
            leftpar = 9,	// (
            rightpar = 4,	// )
            lbracket = 11,	// [
            rbracket = 12,	// ]
            flpar = 63,	// {
            frpar = 64,	// }
            later = 65,	// <
            greater = 66,	// >
            laterequal = 67,	//  <=
            greaterequal = 68,	//  >=
            latergreater = 69,	//  <>
            plus = 70,	// +
            minus = 71,	// –
            lcomment = 72,	//  (*
            rcomment = 73,	//  *)
            assign = 51,	//  :=
            twopoints = 74,	//  ..
            ident = 2,	// идентификатор
            floatc = 82,	// вещественная константа
            intc = 15,	// целая константа
            casesy = 31,
            elsesy = 32,
            filesy = 57,
            gotosy = 33,
            thensy = 52,
            typesy = 34,
            untilsy = 53,
            dosy = 54,
            withsy = 37,
            ifsy = 56,
            insy = 100,
            ofsy = 101,
            orsy = 102,
            tosy = 103,
            endsy = 104,
            varsy = 105,
            divsy = 106,
            andsy = 107,
            notsy = 108,
            forsy = 109,
            modsy = 110,
            nilsy = 111,
            setsy = 112,
            beginsy = 113,
            whilesy = 114,
            arraysy = 115,
            constsy = 116,
            labelsy = 117,
            downtosy = 118,
            packedsy = 119,
            recordsy = 120,
            repeatsy = 121,
            programsy = 122,
            functionsy = 123,
            procedurensy = 124,
            integersy = 125,
            quoteOne = 126, // '
            quoteTwo = 127, // "
            comment = 128, // comment
            stringc = 129,
            writelnsy = 130, // строковая константа 
            stringsy = 131, // string
            floatsy = 132, // float
            equalStar = 133, // *=
            equalSlash = 134, // /=
            equalMinus = 135, // -=
            equalPlus = 136; // +=

        static public byte symbol; // код символа
        static public TextPosition token; // позиция символа
        static string addrName; // адрес идентификатора в таблице имен
        static int nmb_int; // значение целой константы
        static float nmb_float; // значение вещественной константы
        static char one_symbol; // значение символьной константы
        static Keywords keywords = new Keywords();

        static public void NextSym()
        {
            while (InputOutput.Ch == ' ') InputOutput.NextCh();
            token.lineNumber = InputOutput.positionNow.lineNumber;
            token.charNumber = InputOutput.positionNow.charNumber;

            if (Char.IsDigit(InputOutput.Ch)) {
                byte digit;
                Int16 maxint = Int16.MaxValue;
                nmb_int = 0;
                while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9')
                {
                    digit = (byte)(InputOutput.Ch - '0');
                    if (nmb_int < maxint / 10 ||
                    (nmb_int == maxint / 10 &&
                    digit <= maxint % 10))
                        nmb_int = 10 * nmb_int + digit;
                    else
                    {
                        // константа превышает предел
                        InputOutput.Error(203, InputOutput.positionNow);
                        nmb_int = 0;
                        while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9') InputOutput.NextCh();
                    }
                    InputOutput.NextCh();
                }
                if (InputOutput.Ch == '.') 
                {
                    InputOutput.NextCh();
                    while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9')
                    {
                        InputOutput.NextCh();
                        symbol = floatc;
                    }
                }
                else symbol = intc;
            }
            else if (Char.IsLetter(InputOutput.Ch)) {
                string name = "";
                while (((InputOutput.Ch >= 'a' && InputOutput.Ch <= 'z') ||
                        (InputOutput.Ch >= 'A' && InputOutput.Ch <= 'Z') ||
                        (InputOutput.Ch >= '0' && InputOutput.Ch <= '9') ))
                {
                    name += InputOutput.Ch;
                    InputOutput.NextCh();
                    if (InputOutput.positionNow.charNumber == 1) break;
                }
                //symbol = код идентификатора или код ключевого слова
                try
                {
                    symbol = keywords.Kw[(byte)name.Length][name];
                }
                catch (Exception) {
                    SemanticAnalyzer.name = name.GetHashCode();
                    symbol = ident;
                }
            }
            else if (InputOutput.Ch == '<') {
                InputOutput.NextCh();
                if (InputOutput.Ch == '=')
                {
                    symbol = laterequal; InputOutput.NextCh();
                }
                else
                    if (InputOutput.Ch == '>')
                {
                    symbol = latergreater; InputOutput.NextCh();
                }
                else
                    symbol = later;
            }
            else if (InputOutput.Ch == '>') {
                InputOutput.NextCh();
                if (InputOutput.Ch == '=')
                {
                    symbol = greaterequal; InputOutput.NextCh();
                }
                else
                    symbol = greater;
            }
            else if (InputOutput.Ch == ':') {
                InputOutput.NextCh();
                if (InputOutput.Ch == '=')
                {
                    symbol = assign; InputOutput.NextCh();
                }
                else
                    symbol = colon;
            }
            else if (InputOutput.Ch == ';') {
                symbol = semicolon;
                InputOutput.NextCh();
            }
            else if (InputOutput.Ch == '.')
            {
                InputOutput.NextCh();
                if (InputOutput.Ch == '.' && InputOutput.positionNow.charNumber != InputOutput.lastInLine+1)
                {
                    symbol = twopoints; 
                    InputOutput.NextCh();
                }
                else symbol = point;
            }
            else if (InputOutput.Ch == '/')
            {
                symbol = slash; InputOutput.NextCh();
            }
            else if (InputOutput.Ch == ',')
            {
                symbol = comma; InputOutput.NextCh();
            }
            else if (InputOutput.Ch == '=')
            {
                InputOutput.NextCh();
                if (InputOutput.Ch == '+')
                {
                    symbol = equalPlus;
                    InputOutput.NextCh();
                }
                else if (InputOutput.Ch == '-')
                {
                    symbol = equalMinus;
                    InputOutput.NextCh();
                }
                else if (InputOutput.Ch == '/')
                {
                    symbol = equalSlash;
                    InputOutput.NextCh();
                }
                else if (InputOutput.Ch == '*')
                {
                    symbol = equalStar;
                    InputOutput.NextCh();
                }
                else symbol = equal;
            }
            else if (InputOutput.Ch == '^')
            {
                symbol = arrow; InputOutput.NextCh();
            }
            else if (InputOutput.Ch == '(')
            {
                InputOutput.NextCh();
                if (InputOutput.Ch == '*' && InputOutput.positionNow.charNumber != InputOutput.lastInLine)
                {
                    symbol = lcomment; InputOutput.NextCh();
                    while(InputOutput.positionNow.charNumber != InputOutput.lastInLine+1)
                    {
                        if(InputOutput.Ch == '*')
                        {
                            InputOutput.NextCh();
                            if (InputOutput.Ch == ')' && InputOutput.positionNow.charNumber != InputOutput.lastInLine+1)
                            {
                                InputOutput.symbolString += Convert.ToString(lcomment) + " ";
                                InputOutput.symbolString += Convert.ToString(comment) + " ";
                                symbol = rcomment; 
                                InputOutput.NextCh();
                                break;
                            }
                        }
                        else InputOutput.NextCh();
                    }
                    if(symbol != rcomment)
                    {
                        InputOutput.symbolString += Convert.ToString(lcomment) + " ";
                        symbol = comment;
                    }
                }
                else symbol = leftpar;
            }
            else if (InputOutput.Ch == ')')
            {
                symbol = rightpar; InputOutput.NextCh();
            }
            else if (InputOutput.Ch == '[')
            {
                symbol = lbracket; InputOutput.NextCh();
            }
            else if (InputOutput.Ch == ']')
            {
                symbol = rbracket; InputOutput.NextCh();
            }
            else if (InputOutput.Ch == '{')
            {
                symbol = flpar; InputOutput.NextCh();
            }
            else if (InputOutput.Ch == '}')
            {
                symbol = frpar; InputOutput.NextCh();
            }
            else if (InputOutput.Ch == '+')
            {
                symbol = plus; InputOutput.NextCh();
            }
            else if (InputOutput.Ch == '-')
            {
                symbol = minus; InputOutput.NextCh();
            }
            else if (InputOutput.Ch == '\'')
            {
                symbol = stringc;
                InputOutput.NextCh();
                while (InputOutput.positionNow.charNumber != InputOutput.lastInLine + 1)
                {
                    if (InputOutput.Ch == '\'')
                    {
                        InputOutput.symbolString += Convert.ToString(quoteOne) + " ";
                        InputOutput.symbolString += Convert.ToString(stringc) + " ";
                        symbol = quoteOne;
                        InputOutput.NextCh();
                        break;
                    }
                    else InputOutput.NextCh();
                }
                if (symbol != quoteOne)
                {
                    InputOutput.symbolString += Convert.ToString(quoteOne) + " ";
                    symbol = stringc;
                }
            }
            else if (InputOutput.Ch == '\"')
            {
                symbol = stringc;
                InputOutput.NextCh();
                while (InputOutput.positionNow.charNumber != InputOutput.lastInLine + 1)
                {
                    if (InputOutput.Ch == '\"')
                    {
                        InputOutput.symbolString += Convert.ToString(quoteTwo) + " ";
                        InputOutput.symbolString += Convert.ToString(stringc) + " ";
                        symbol = quoteTwo;
                        InputOutput.NextCh();
                        break;
                    }
                    else InputOutput.NextCh();
                }
                if (symbol != quoteTwo)
                {
                    InputOutput.symbolString += Convert.ToString(quoteTwo) + " ";
                    symbol = stringc;
                }
            }
            else if (InputOutput.Ch == '*')
            {
                if (InputOutput.Ch == ')' && InputOutput.positionNow.charNumber != InputOutput.lastInLine+1)
                {
                    symbol = rcomment; InputOutput.NextCh();
                }
                else symbol = star; 
            }
            InputOutput.symbolString += Convert.ToString(symbol) + " ";
        }
    }
}
