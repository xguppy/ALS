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

        private List<string> _uniqueVars = new List<string>();

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
        private void GenConst(List<string> result, State s, bool isFirst, double range = 100.0, bool isDouble = true, int finesse = 3)
        {
            var pos = result.Count - 1;
            double rndConst = 0.0;
            if (s == State.Sign)
            {
                while (Math.Abs(rndConst) < 1.0e-12)
                {
                    rndConst = isDouble ? RndWrapper.NextD(-range, range, _random) : RndWrapper.NextI((int)-range, (int)range, _random);
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
                    // убеждаемся что необходимое кол-во переменных присутсвует в выражении
                    if (result[pos - 2].Length == 2 && !_uniqueVars.Contains(result[pos - 2]))
                    {
                        _uniqueVars.Add(result[pos - 2]);
                    }
                    else if (result[pos - 1].Length == 2 && !_uniqueVars.Contains(result[pos - 1]))
                    {
                        _uniqueVars.Add(result[pos - 1]);
                    }
                    // далее случайным образом меняем переменную на значение
                    else if (result[pos - 1].Length != 2 || (result[pos - 2].Length == 2 && _random.Next(0, 100) < 50))
                    {
                        result[pos - 2] = val;
                    }
                    else if (result[pos - 1].Length == 2 && _random.Next(0, 100) < 50)
                    {
                        if (result[pos] == "^")
                            result[pos - 1] = $"{(int)(rndConst * 10 / range)}";
                        else
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
                CodeOnC = $"if ({_condition}) y = {Expression}; else y = 0.0;";
            }
            else
            {
                CodeOnC = $"y = {Expression};";
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
        public void Run(int itCount = 5, double range = 100.0, bool isDouble = true, int countOfVars = 1)
        {
            _condition = "";
            Tree tree = _genTree.Next(itCount, countOfVars);
            List<string> result = new List<string>();
            bool isRight = false, isFirst = true;
            
            while (tree != null)
            {
                if (tree.State != State.Var)
                {   // если узел стал листом
                    if (tree.IsLeaf())
                    {
                        result.Add(tree.Value);
                        GenConst(result, tree.State, isFirst, range, isDouble);
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
                    //result.Add(tree.Value);
                    result.Add(_genTree.GetNewVar());
                    tree = tree.Parent;
                    if (tree == null) continue;
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