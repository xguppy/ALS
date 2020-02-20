using System;
using System.Collections.Generic;

namespace Generator
{
    public class GenTree
    {
        private Random _random =  new Random();
        private List<Tree> _trees;
        private bool _powismorethenone = false;
        private int _numberOfVar = 1;
        private int _countOfVars = 0;

        public Tree Next(int itCount, int countOfVars = 1)
        {
            int curr = 0;
            Tree t = new Tree(null);
            _trees = new List<Tree>() { t };
            _countOfVars = countOfVars;
            GenArithmExprVer3(itCount, 0, ref curr);
            return t;
        }

        public string GetNewVar()
        {
            string res = $"x{_numberOfVar}";
            _numberOfVar = ((_numberOfVar + 1) % (_countOfVars+1));
            if (_numberOfVar == 0) _numberOfVar++;

            return res;
        }

        // golden middle
        private void GenArithmExprVer3(int itCount, int num, ref int curr)
        {
            if (curr == itCount) return;

            var res = _random.Next(0, 100);
            var tree = _trees[num];
            if (Elems.Funcs.Length > 0 && res < 25)
            {
                tree.Value = $"{Elems.Funcs[_random.Next(0, Elems.Funcs.Length)]}";
                tree.Left = new Tree(tree);
                curr++;
                _trees.Add(tree.Left);
                tree.State = State.Func;
            }
            else if (Elems.Signs.Length > 0)
            {
                int gran = _powismorethenone ? 1 : 0;
                tree.Value = Elems.Signs[_random.Next(0, Elems.Signs.Length - gran)];
                tree.State = State.Sign;
                tree.Left = new Tree(tree, GetNewVar());
                tree.Right = new Tree(tree, GetNewVar());

                if (!_powismorethenone) _powismorethenone = tree.Value == "^";

                Tree newTree = tree.Right;
                if (_random.Next(0, 100) < 50)
                {
                    newTree = tree.Left;
                }

                curr++;
                _trees.Add(newTree);
            }

            if (curr % 15 == 0) _trees.RemoveAll(x => !x.IsLeaf());

            int nextTree = -1;
            while (true)
            {
                nextTree = _random.Next(0, _trees.Count);
                if (_trees[nextTree].IsLeaf())
                {
                    break;
                }
            }

            GenArithmExprVer3(itCount, nextTree, ref curr);
        }

        // old and slow
        private void GenArithmExprVer2(int itCount, int num, ref int curr)
        {
            if (curr == itCount) return;

            var res = _random.Next(0, 100);
            var tree = _trees[num];

            if (res < 25)
            {
                tree.Value = $"{Elems.Funcs[_random.Next(0, Elems.Funcs.Length)]}";
                tree.Left = new Tree(tree);
                curr++;
                _trees.Add(tree.Left);
                tree.State = State.Func;
                //GenArithmExpr(itCount, tree.Left, ref curr);
            }
            else
            {
                tree.Value = Elems.Signs[_random.Next(0, Elems.Signs.Length)];
                tree.State = State.Sign;
                tree.Left = new Tree(tree);
                tree.Right = new Tree(tree);

                Tree newTree = tree.Right;
                if (_random.Next(0, 100) < 50)
                {
                    newTree = tree.Left;
                }

                curr++;
                _trees.Add(newTree);
            }

            int nextTree = -1;
            while (true)
            {
                nextTree = _random.Next(0, _trees.Count);
                if (_trees[nextTree].IsLeaf())
                {
                    break;
                }
            }

            GenArithmExprVer2(itCount, nextTree, ref curr);
        }

        // old, fast but is boring
        private void GenArithmExpr(int itCount, Tree tree, ref int curr)
        {
            if (curr == itCount) return;

            var res = _random.Next(0, 100);

            if (res < 25)
            {
                tree.Value = $"{Elems.Funcs[_random.Next(0, Elems.Funcs.Length)]}";
                tree.Left = new Tree(tree);
                curr++;
                tree.State = State.Func;
                GenArithmExpr(itCount, tree.Left, ref curr);
            }
            else
            {
                tree.Value = Elems.Signs[_random.Next(0, Elems.Signs.Length)];
                tree.State = State.Sign;
                tree.Left  = new Tree(tree);
                tree.Right = new Tree(tree);
                
                Tree newTree = tree.Right;
                if (_random.Next(0, 100) < 50)
                {
                    newTree = tree.Left;
                }

                curr++;
                GenArithmExpr(itCount, newTree, ref curr);
            }

        }
    }
}