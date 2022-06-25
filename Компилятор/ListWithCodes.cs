using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Компилятор
{
    class ListWithCodes
    {
        public HashSet<byte> 
            Types,
            Values,
            Expressions,
            OperatorEquals,
            begpart,
            st_typepart,
            st_varpart,
            st_procfuncpart,
            id_starters,
            after_var,
            st_expression,
            st_program,
            all_positions_for_skip;
        
        public ListWithCodes()
        {
            Values = new HashSet<byte>();                   // значения
            Values.Add(LexicalAnalyzer.floatc);             // я вещественное число
            Values.Add(LexicalAnalyzer.intc);               // я число 
            Values.Add(LexicalAnalyzer.stringc);            // "я строка"
            Values.Add(LexicalAnalyzer.ident);              // я что?

            Types = new HashSet<byte>();                    // типы
            Types.Add(LexicalAnalyzer.integersy);           // integer
            Types.Add(LexicalAnalyzer.floatsy);             // float
            Types.Add(LexicalAnalyzer.stringsy);            // string

            Expressions = new HashSet<byte>();              // выражения
            Expressions.Add(LexicalAnalyzer.star);          // *
            Expressions.Add(LexicalAnalyzer.slash);         // /
            Expressions.Add(LexicalAnalyzer.equal);         // =
            Expressions.Add(LexicalAnalyzer.plus);          // +
            Expressions.Add(LexicalAnalyzer.minus);         // -
            Expressions.Add(LexicalAnalyzer.latergreater);  // <>
            Expressions.Add(LexicalAnalyzer.greaterequal);  // >=
            Expressions.Add(LexicalAnalyzer.laterequal);    // <=
            Expressions.Add(LexicalAnalyzer.greater);       // >
            Expressions.Add(LexicalAnalyzer.later);         // <
            Expressions.Add(LexicalAnalyzer.arrow);         // ^
            Expressions.Add(LexicalAnalyzer.notsy);         // not
            Expressions.Add(LexicalAnalyzer.andsy);         // and
            Expressions.Add(LexicalAnalyzer.orsy);          // or
            Expressions.Add(LexicalAnalyzer.divsy);         // div
            Expressions.Add(LexicalAnalyzer.modsy);         // mod

            OperatorEquals = new HashSet<byte>();           // присваивание
            OperatorEquals.Add(LexicalAnalyzer.assign);     // := 
            OperatorEquals.Add(LexicalAnalyzer.equalStar);  // *=
            OperatorEquals.Add(LexicalAnalyzer.equalSlash); // /=
            OperatorEquals.Add(LexicalAnalyzer.equalPlus);  // +=
            OperatorEquals.Add(LexicalAnalyzer.equalMinus); // -=

            ///////////////////////////////////////////////////////////////////////////нейтролизатор
            
            begpart = new HashSet<byte>();
            begpart.Add(LexicalAnalyzer.labelsy);
            begpart.Add(LexicalAnalyzer.constsy);
            begpart.Add(LexicalAnalyzer.typesy);
            begpart.Add(LexicalAnalyzer.varsy);
            begpart.Add(LexicalAnalyzer.functionsy);
            begpart.Add(LexicalAnalyzer.procedurensy);
            begpart.Add(LexicalAnalyzer.beginsy);

            //st_typepart = new HashSet<byte>();
            //st_typepart.Add(LexicalAnalyzer.typesy);
            //st_typepart.Add(LexicalAnalyzer.varsy);
            //st_typepart.Add(LexicalAnalyzer.functionsy);
            //st_typepart.Add(LexicalAnalyzer.procedurensy);
            //st_typepart.Add(LexicalAnalyzer.beginsy);

            //st_varpart = new HashSet<byte>();
            //st_varpart.Add(LexicalAnalyzer.varsy);
            //st_varpart.Add(LexicalAnalyzer.functionsy);
            //st_varpart.Add(LexicalAnalyzer.procedurensy);
            //st_varpart.Add(LexicalAnalyzer.beginsy);

            //st_procfuncpart = new HashSet<byte>();
            //st_procfuncpart.Add(LexicalAnalyzer.functionsy);
            //st_procfuncpart.Add(LexicalAnalyzer.procedurensy);
            //st_procfuncpart.Add(LexicalAnalyzer.beginsy);

            id_starters = new HashSet<byte>();
            id_starters.Add(LexicalAnalyzer.ident);

            st_program = new HashSet<byte>();
            st_program.Add(LexicalAnalyzer.programsy);
            st_program.Add(LexicalAnalyzer.ident);
            st_program.Add(LexicalAnalyzer.semicolon);

            st_expression = new HashSet<byte>();
            st_expression.Add(LexicalAnalyzer.floatc);             // я вещественное число
            st_expression.Add(LexicalAnalyzer.intc);               // я число 
            st_expression.Add(LexicalAnalyzer.stringc);            // "я строка"
            st_expression.Add(LexicalAnalyzer.ident);              // я что?
            st_expression.Add(LexicalAnalyzer.notsy);              // not

            all_positions_for_skip = new HashSet<byte>();
            all_positions_for_skip.Add(LexicalAnalyzer.whilesy);
            all_positions_for_skip.Add(LexicalAnalyzer.elsesy);
            all_positions_for_skip.Add(LexicalAnalyzer.casesy);
            all_positions_for_skip.Add(LexicalAnalyzer.ifsy);
            all_positions_for_skip.Add(LexicalAnalyzer.ident);
            all_positions_for_skip.Add(LexicalAnalyzer.endsy);
        }
    }
}
