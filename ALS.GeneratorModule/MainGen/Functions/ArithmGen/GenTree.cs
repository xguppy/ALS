using System;

namespace Generator
{
    // создание дерева (скелета) для генерации полноценного арифм.выражения
    public class GenTree
    {
        private Random _rnd = new Random();
        private Elems _elems; // доступные знаки и математические функции
        private bool _frac = false; // генерировать дробь?

        public GenTree(Elems e)
        {
            _elems = e;
        }

        // создание узла со знаком
        private void SignNode(Tree tree)
        {
            tree.State = State.Sign;
            tree.Left = new Tree(tree);
            tree.Right = new Tree(tree);
        }

        // создание узла с мат. функцией
        private Tree FunctionNode(Tree tree)
        {
            tree.State = State.Func;
            tree.Left = new Tree(tree);
            return tree.Left;
        }

        // распространение генерации по узлам рекурсивно
        private void ContiniousGen(Tree tree, int depth)
        {
            if (depth < 1) return;

            var left = _rnd.Next(1, depth);
            var right = depth - left;
            GenNewTree(tree.Left, left, 0);
            GenNewTree(tree.Right, right, 0);
        }
        // создание нового дерева для арифм. выражения
        private void GenNewTree(Tree tree, int hard, int curr)
        {
            if (curr < hard)
            {
                curr++;
                if ((tree.Parent.State == State.Root && _frac) || 
                    (_elems.Signs.Count > 0 && _rnd.Next(0, 101) < 70))
                {
                    SignNode(tree);
                    ContiniousGen(tree, hard - curr);
                }
                else
                    GenNewTree(FunctionNode(tree), hard, curr);
            }
        }

        public Tree Run(int hard, bool frac = false)
        {
            _frac = frac;
            var tree = new Tree(null);
            tree.Left = new Tree(tree);
            tree.State = State.Root;
            tree.Value = "ROOT";
            GenNewTree(tree.Left, hard, 0);
            return tree;
        }
    }
}