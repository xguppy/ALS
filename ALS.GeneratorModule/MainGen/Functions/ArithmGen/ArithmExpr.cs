using System;
using System.Collections.Generic;
using System.Text;

namespace Generator
{
    public class ArithmExpr
    {
        private Random _rnd = new Random();
        // выражение на C++
        public string Expression { get; set; } = default;
        // выражение в формате для KaTeX
        public string ExpressionMD { get; set; } = default;
        // код для вычисления выражения на C++ (вместе с условием)
        public string Code { get; set; } = default;
        // условие для вычисления выражения (на языке C++)
        public string Conditions { get; set; } = default;

        private double _rangeOfConst; // диапазон для констант
        private bool _constType; // тип констант целые или дробные
        private int _finesse = 3; // количество знаков после точки
        private bool _frac = false; // создавать дробь?

        private int _numberOfVars = 1; // зависимость от переменных
        private int  _countOfVars = 0;

        private Elems _elems; // доступные математические знаки и функции

        public ArithmExpr(Elems e)
        {
            _elems = e;
        }

        // перейти к самому левому листу
        private Tree GetLeftLeaf(Tree tree)
        {
            while (tree.State != State.Var && tree.State != State.DoneNode) tree = tree.Left;
            return tree;
        }

        // странные проблемы с вещественными, в разных локализациях ОС
        // вещественные генерируются по разному: то 123,321 то 123.321
        private string MakeDoubleGreatAgain(double val)
        {
            string temp = val.ToString();
            return temp.Substring(0, temp.IndexOf(',') + _finesse + 1).Replace(',', '.');
        }

        // превращение одной из переменных в константу
        private void AEConst(Tree tree)
        {
            if (_countOfVars > 2 && _numberOfVars < _countOfVars) { MakeNeededVars(tree); return; }

            Tree t = (_rnd.Next(0,100) > 50 && tree.Right.State == State.Var) ? tree.Right : tree.Left;

            if (t.State == State.Var)
            {
                t.ValueMD = t.Value = _constType ?
                    MakeDoubleGreatAgain(RndWrapper.NextD(-_rangeOfConst, _rangeOfConst, _rnd)) 
                    : 
                    RndWrapper.NextI((int)-_rangeOfConst, (int)_rangeOfConst, _rnd).ToString();
            }
        }
        // Убеждаемся что в выражении присутствует нужное кол-во переменных
        private void MakeNeededVars(Tree tree)
        {
            CheckTree(tree.Left);
            CheckTree(tree.Right);
        }

        // если необходимо создаем нужные переменные
        private void CheckTree(Tree tree)
        {
            if (_numberOfVars < _countOfVars)
            {
                if (tree.State == State.Var)
                {
                     tree.Value = tree.ValueMD = $"x{_numberOfVars}";
                    _numberOfVars++; 
                }
            }
        }
        // создаем готовый узел
        private void AEChangeNode(Tree tree, List<AEElem> ls)
        {
            int number = _rnd.Next(0, ls.Count);
            AEElem elem = _frac && tree.Parent.Value == "ROOT" ? _elems.Frac : ls[number];
            var localConstr = elem.MakeConstr(tree);
            if (localConstr != default) Code = $"{Code} && {localConstr}";
            tree.Value = elem.Makeorig(tree);
            tree.ValueMD = elem.MakeMD(tree);
            tree.State = State.DoneNode;
        }
        /*private void AESignMake(Tree tree)
        {
            AEConst(tree); // заменяем переменные на константы
            AEChangeNode(tree, _elems.Signs); // выбираем случайный знак
        }*/


        // выбираем случайную функцию
        private void AEFunc(Tree tree) => AEChangeNode(tree, _elems.Funcs);

        private void AESign(Tree tree)
        {
            if (tree.Right.State == State.Var || tree.Right.State == State.DoneNode)
            {
                AEConst(tree); // заменяем переменные на константы
                AEChangeNode(tree, _elems.Signs); // выбираем случайный знак
            }
            else
            {
                // обработка части дерева рекурсивно
                var t = tree.Parent.State;
                tree.Parent.State = State.Root;
                DoForNode(tree.Right);
                tree.Parent.State = t;
            }
        }
        // обработка части дерева рекурсивно
        private void DoForNode(Tree tree)
        {
            tree = GetLeftLeaf(tree);
            if (tree.Parent.State != State.Root)
            {
                tree = tree.Parent;
                switch (tree.State)
                {
                    case State.Sign:
                        AESign(tree);
                        break;
                    case State.Func:
                        AEFunc(tree);
                        break;
                }
                DoForNode(tree);
            }
        }

        public void MakeAE(int hard, double range, bool isDouble, int countOfVars, int finesse, bool frac)
        {
            if (hard < 1) throw new Exception("сложность выражения должна быть больше 1");

            Code = Expression = ExpressionMD = default;

            _finesse = finesse;
            _rangeOfConst = range;
            _constType = isDouble;
            _countOfVars = countOfVars+1;
            _frac = frac;

            var tree = new GenTree(_elems).Run(hard, _frac);
            DoForNode(tree);
            Expression = tree.Left.Value;
            ExpressionMD = tree.Left.ValueMD;//$"$${tree.Left.ValueMD}$$";
            if (Code != default) {
                Conditions = Code.Remove(0, 3);
                Code = $"if ({Conditions}) y = {Expression}; else y = 0.0;";
            }
            else
            {
                Conditions = "true";
                Code = $"y = {Expression};";
            };
        }
    }
}