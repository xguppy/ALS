using System;
using System.Collections.Generic;

namespace Generator
{
    public class ArithmExpr
    {
        private Random _random = new Random();
        public string Expression { get; set; }
        public string CodeOnC { get; set; }
        private string _condition { get; set; }
        
        private GenTree _genTree = new GenTree();

        
        // проход по дереву до самого крайнего левого листа
        private Tree GoLeft(Tree tree)
        {
            while (!tree.IsLeaf())
            {
                tree = tree.Left;
            }
            return tree;
        }
        
        //генерация констант (изменение листьев дерева на случайные значения)
        private void GenConst(List<string> result, State s, bool isFirst, double range = 100.0, int finesse = 3)
        {
            var pos = result.Count - 1;
            double rndConst = 0.0;
            if (s == State.Sign)
            {
                while (Math.Abs(rndConst) < 1.0e-12)
                {
                    rndConst = RndWrapper.NextD(-range, range, _random);
                }

                string val = $"{rndConst}";
                if (val.IndexOf(',') != -1)
                {
                    val = val.Substring(0, val.IndexOf(',') + finesse).Replace(',', '.');
                }
                if (isFirst)
                {
                    result[pos - 2] = val;
                }
                else
                {
                    if (_random.Next(0, 100) < 50 && result[pos - 2].Length == 1)
                    {
                        result[pos - 2] = val;
                        return;
                    }
                    if (_random.Next(0, 100) < 50 && result[pos - 1].Length == 1)
                    {
                        if (result[pos] == "^")
                        {
                            result[pos - 1] = $"{(int)(rndConst*10/range)}";
                            return;
                        }
                        result[pos - 1] = val;
                    }
                }
            }
        }
        
        // генерация кода на c++, именно условия для выполнения выражения
        private void GenCode()
        {
            if (_condition.Length > 0)
            {
                _condition = _condition.Remove(_condition.Length - 3, 3);
                CodeOnC = $"if ({_condition})\nvar = {Expression};";
            }
            else
            {
                CodeOnC = $"var = {Expression}";
            }
        }
        // генерация выражения в виде строки
        private void GenExpr(List<string> result, State s)
        {
            var pos = result.Count - 1;
            AddCond(s, result[pos], result[pos - 1]);
            if (s == State.Sign)
            {
                if (result[pos] == "^")
                {
                    result[pos] = $"pow({result[pos-2]}, {result[pos-1]})";
                }
                else
                {
                    result[pos] = $"({result[pos - 2]} {result[pos]} {result[pos - 1]})";
                }
                result.RemoveAt(pos-1);
                result.RemoveAt(pos-2);
            }
            else if (s == State.Func)
            {
                if (result[pos - 1][0] != '(')
                {
                    result[pos] = $"{result[pos]}({result[pos-1]})";
                }
                else
                {
                    result[pos] = $"{result[pos]}{result[pos-1]}";
                }
                result.RemoveAt(pos-1);
            }
        }

        private void AddCond(State s, string condEl, string arg)
        {
            if (s == State.Sign)
            {
                if (condEl == "/")
                {
                    _condition += $"{arg} != 0 && ";
                }
            }
            else
            {
                if (condEl == "sqrt")
                {
                    _condition += $"({arg} > 0) && ";
                }
                if (condEl == "log" || condEl == "ln" || condEl == "log10")
                {
                    _condition += $"({arg} > 0 && {arg} != 1) && ";
                }
            }
        }

        // перевод дерева в выражение       
        public void Run(int itCount = 5)
        {
            _condition = "";
            Tree tree = _genTree.Next(itCount);
            List<string> result = new List<string>();
            bool isRight = false, isFirst = true;
            
            while (tree != null)
            {
                if (tree.State != State.Var)
                {   // если узел стал листом
                    if (tree.IsLeaf())
                    {
                        result.Add(tree.Value);
                        GenConst(result, tree.State, isFirst);
                        GenExpr(result, tree.State);
                        isFirst = false;
                        var tmp = tree;
                        tree = tree.Parent;
                        if (tree == null) continue;
                        // удаление использованного узла-листа
                        if (tree.Left == tmp)
                        {
                            tree.Left = null;
                        }
                        else
                        {
                            tree.Right = null;
                        }
                    } // переход к самому левому листу
                    else if (tree.Left != null)
                    {
                        tree = GoLeft(tree);
                        isRight = false;
                    } // переход к правому узлу/листу
                    else
                    {
                        tree = tree.Right;
                        isRight = true;
                    }
                    continue;
                }
                // если лист
                if (tree.State == State.Var)
                {
                    result.Add(tree.Value);
                    tree = tree.Parent;
                    // удаление использованного листа
                    if (isRight)
                    {
                        tree.Right = null;
                    }
                    else
                    {
                        tree.Left = null;
                    }
                }
            }
            
            Expression = result[0];
            if (Expression[0] == '(')
            {
                Expression = Expression.Remove(0, 1).Remove(Expression.Length-2);
            }
            GenCode();
        }
        
        
    }
}