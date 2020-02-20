namespace Generator
{
    public enum State
    {
        Sign,
        Func,
        Var
    }
    public class Tree
    {
        public string Value { get; set; }
        public Tree Left { get; set; } = null;
        public Tree Right { get; set; } = null;
        public Tree Parent { get; set; } = null;
        public State State = State.Var;
        

        public Tree(Tree parent, string value = "x1")
        {
            Value = value;
            Parent = parent;
        }

        public bool IsLeaf() => Left == null && Right == null;
        public bool IsStart() => Parent == null;
    }
}