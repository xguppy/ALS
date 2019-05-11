using System;

namespace Generator
{
    public class GenTree
    {
        private Random _random =  new Random();

        public Tree Next(int itCount)
        {
            int curr = 0;
            Tree t = new Tree(null);
            GenArithmExpr(itCount, t, ref curr);
            return t;
        }
        
        private void GenArithmExpr(int itCount, Tree tree, ref int curr)
        {
            if (curr == itCount) return;

            var res = _random.Next(0, 100);

            if (res < 35)
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