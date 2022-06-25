using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Компилятор
{
    class StFoll
    {
        public HashSet<byte>[] sf = new HashSet<byte>[30];

        public const byte
            begpart = 0,
            st_typepart = 1,
            st_varpart = 2,
            st_procfuncpart = 3,
            id_starters = 4,
            after_var = 5;
        public StFoll()
        {
            sf[begpart] = new HashSet<byte>();
            sf[begpart].Add(LexicalAnalyzer.labelsy);
            sf[begpart].Add(LexicalAnalyzer.constsy);
            sf[begpart].Add(LexicalAnalyzer.typesy);
            sf[begpart].Add(LexicalAnalyzer.varsy);
            sf[begpart].Add(LexicalAnalyzer.functionsy);
            sf[begpart].Add(LexicalAnalyzer.procedurensy);
            sf[begpart].Add(LexicalAnalyzer.beginsy);

            sf[st_typepart] = new HashSet<byte>();
            sf[st_typepart].Add(LexicalAnalyzer.typesy);
            sf[st_typepart].Add(LexicalAnalyzer.varsy);
            sf[st_typepart].Add(LexicalAnalyzer.functionsy);
            sf[st_typepart].Add(LexicalAnalyzer.procedurensy);
            sf[st_typepart].Add(LexicalAnalyzer.beginsy);

            sf[st_varpart] = new HashSet<byte>();
            sf[st_varpart].Add(LexicalAnalyzer.varsy);
            sf[st_varpart].Add(LexicalAnalyzer.functionsy);
            sf[st_varpart].Add(LexicalAnalyzer.procedurensy);
            sf[st_varpart].Add(LexicalAnalyzer.beginsy);

            sf[st_procfuncpart] = new HashSet<byte>();
            sf[st_procfuncpart].Add(LexicalAnalyzer.functionsy);
            sf[st_procfuncpart].Add(LexicalAnalyzer.procedurensy);
            sf[st_procfuncpart].Add(LexicalAnalyzer.beginsy);

            sf[id_starters] = new HashSet<byte>();
            sf[id_starters].Add(LexicalAnalyzer.ident);

            sf[after_var] = new HashSet<byte>();
            sf[after_var].Add(LexicalAnalyzer.semicolon);
        }
    }
}