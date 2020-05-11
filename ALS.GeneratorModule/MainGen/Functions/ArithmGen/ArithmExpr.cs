using System;
using System.Collections.Generic;
using System.Text;

namespace Generator
{
    public class ArithmExpr
    {
        private Random _rnd = new Random();
        public string Expression { get; set; } = default;
        public string ExpressionMD { get; set; } = default;
        public string Code { get; set; } = default;

        private double _rangeOfConst;
        private bool _constType;
        private int _finesse = 3;

        private int _numberOfVars = 1;
        private int  _countOfVars = 0;

        private Tree GetLeftLeaf(Tree tree)
        {
            while (tree.State != State.Var && tree.State != State.DoneNode) tree = tree.Left;
            return tree;
        }

        private string MakeDoubleGreatAgain(double val)
        {
            string temp = val.ToString();
            return temp.Substring(0, temp.IndexOf(',') + _finesse + 1).Replace(',', '.');
        }

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

        private void AEChangeNode(Tree tree, List<AEElem> ls)
        {
            int number = _rnd.Next(0, ls.Count);
            var localConstr = ls[number].MakeConstr(tree);
            if (localConstr != default) Code = $"{Code} && {localConstr}";
            tree.Value = ls[number].MakeOrig(tree);
            tree.ValueMD = ls[number].MakeMD(tree);
            tree.State = State.DoneNode;
        }

        private void AESignMake(Tree tree)
        {
            AEConst(tree);
            AEChangeNode(tree, Elems.Signs);
        }

        private void AEFunc(Tree tree) => AEChangeNode(tree, Elems.Funcs);

        private void AESign(Tree tree)
        {
            if (tree.Right.State == State.Var || tree.Right.State == State.DoneNode)
                AESignMake(tree);
            else
            {
                var t = tree.Parent.State;
                tree.Parent.State = State.Root;
                DoForNode(tree.Right);
                tree.Parent.State = t;
            }
        }
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

        public void MakeAE(int hard, double range, bool isDouble, int countOfVars, int finesse)
        {
            if (hard < 1) throw new Exception("сложность выражения должна быть больше 1");

            Code = Expression = ExpressionMD = default;

            _finesse = finesse;
            _rangeOfConst = range;
            _constType = isDouble;
            _countOfVars = countOfVars+1;

            var tree = new GenTree().Run(hard+1);
            DoForNode(tree);
            Expression = tree.Left.Value;
            ExpressionMD = tree.Left.ValueMD;//$"$${tree.Left.ValueMD}$$";
            if (Code != default)  Code = $"if ({Code.Remove(0, 3)}) y = {Expression}; else y = 0.0;";
            else Code = $"y = {Expression};";
        }
    }
}