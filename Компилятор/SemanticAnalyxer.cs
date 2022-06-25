using System;
using System.Collections.Generic;
using System.Collections;
using System.Numerics;

namespace Компилятор
{
    class SemanticAnalyzer
    { 
        static public Stack<TreeNode> heads = new Stack<TreeNode>();
        static public int name;
        static public List<int> names = new List<int>(); // это вообше песня запоминаем все идентификаторы в строке (их хеш коды)
        static public UInt16 tempIdType; // тип испольозования идентификаторов списка выше
        static public UInt16 tempIdTypeArray; // тип испольозования параметров
        public const UInt16 // способ использования идентификатора
            progs = 300,
            types = 301,
            consts = 302,
            vars = 303,
            procs = 304,
            funcs = 305,

            scalars = 401, /* cтандартный скалярный тип */
            limiteds = 402, /* ограниченный тип */
            enums = 403, /* перечислимый тип */
            arrays = 404, /* регулярный тип (массив) */
            references = 405, /* ссылочный тип */
            sets = 406, /* множественный тип */
            files = 407, /* файловый тип */
            records = 408; /* комбинированный тип (запись)*/
        public static int globalOfSet = 0;

        public class IdParam // информация о параметрах
        {
            public UInt16 typeParam; /* информация о типе параметра */
            IdParam linkParam; /* указатель на информацию о следующем параметре */
        }

        public class TreeNode
        {
            public int hashValue = 0;  // значение hash-функции
            public UInt16 klass; // способ использования идентификатора
            public UInt16 idType; // информация о типе
            public int ofSet = 0;
            //public ConstVal constValue; // значение константы
            // для процедур (функций)
            public IdParam param; // указатель на информацию о параметрах

            public TreeNode leftLink = null, rightLink = null, preLink = null;
        }
        static TreeNode searchPlaceAndBalance(int hashfunc, TreeNode nowNode)
        {
            if (!(nowNode is null)) balance(nowNode);
            else
            {
                nowNode = new TreeNode();
                return null; // дерево пустое
            }
            if (hashfunc > nowNode.hashValue && nowNode.rightLink != null)
                return searchPlaceAndBalance(hashfunc, nowNode.rightLink); // идем вправо 
            else if (hashfunc < nowNode.hashValue && nowNode.leftLink != null)
                return searchPlaceAndBalance(hashfunc, nowNode.leftLink); // идем влево            
            return nowNode; // нашли место в дереве
        }
        static public TreeNode newIdent(int hashfunc, // значение hash-функции для идентификатора
                          UInt16 klassUsed = vars, /* способ использования идентификатора */
                          UInt16 idTypeUsed = 0 /* информация о типе */)
        {
            idTypeUsed = tempIdTypeArray == arrays ? arrays : tempIdType;
            TreeNode nowNode = new TreeNode();
            if (!(searchIdent(hashfunc) is null))
            {
                InputOutput.Error(0, LexicalAnalyzer.token, "Ошибка! Идентификатор был уже определен ранее");
                return null;
            }
            TreeNode temp = searchPlaceAndBalance(hashfunc, heads.Peek());
            if (temp is null)
            {
                heads.Pop();
                nowNode = new TreeNode();
                heads.Push(nowNode); // запихнули новую голову
            }
            else if (temp.hashValue < hashfunc) temp.rightLink = nowNode;
            else if (temp.hashValue > hashfunc) temp.leftLink = nowNode;

            nowNode.preLink = temp;
            nowNode.hashValue = hashfunc;
            nowNode.klass = klassUsed;
            nowNode.idType = idTypeUsed;

            if (idTypeUsed == arrays)
            {
                nowNode.param = new IdParam();
                nowNode.param.typeParam = tempIdType;
            }
            nowNode.ofSet = globalOfSet;
            globalOfSet++;
            return nowNode;
        }

        static public TreeNode searchIdent (int hashfunc, /* значение hash-функции для идентификатора */
                         TreeNode nowNode = null)
        {
            if (nowNode is null) // для поиска по дереву
            {
                nowNode = heads.Peek(); // начинаем с головы
                if (nowNode is null)
                {
                    return null; // дерево пустое
                }
            }
            if (hashfunc > nowNode.hashValue && nowNode.rightLink != null)
               return searchIdent(hashfunc, nowNode.rightLink); // идем вправо 
            else if (hashfunc < nowNode.hashValue && nowNode.leftLink != null)
                return searchIdent(hashfunc, nowNode.leftLink); // идем влево            
            else if(hashfunc == nowNode.hashValue) return nowNode; // нашли в дереве
            return null; // не нашли 
        }
        static int findHeight(TreeNode p)
        {
            if (p is null) return 0;
            return Math.Max(findHeight(p.leftLink), findHeight(p.rightLink)) + 1;
        }
        static TreeNode rotateRight(TreeNode p)
        {
            TreeNode q = p.leftLink;
            if (!(p.preLink is null))
            {
                if (p.preLink.leftLink.hashValue == p.hashValue)
                    p.preLink.leftLink = q;
                else p.preLink.rightLink = q;
            }
            else
            {
                heads.Pop();
                heads.Push(q);
            }
            p.leftLink = q.rightLink;
            q.rightLink = p;

            return q;
        }
        static TreeNode rotateLeft(TreeNode p)
        {
            TreeNode q = p.rightLink;
            q.preLink = p.preLink;
            if (!(p.preLink is null))
            {
                if (p.preLink.leftLink.hashValue == p.hashValue)
                    p.preLink.leftLink = q;
                else p.preLink.rightLink = q;
            }
            else
            {
                heads.Pop();
                heads.Push(q);
            }
            p.rightLink = q.leftLink;
            q.leftLink = p;

            return q;
        }
        static int bfactor(TreeNode p)
        {
            return findHeight(p.rightLink) - findHeight(p.leftLink);
        }
        static TreeNode balance (TreeNode p)
        {
            if(bfactor(p) == 2)
            {
                if (bfactor(p.rightLink) < 0)
                    p.rightLink = rotateRight(p.rightLink);
                return rotateLeft(p);
            }
            if(bfactor(p) == -2)
            {
                if (bfactor(p.leftLink) > 0)
                    p.leftLink = rotateLeft(p.leftLink);
                return rotateRight(p);
            }
            return p;
        }
    }
}