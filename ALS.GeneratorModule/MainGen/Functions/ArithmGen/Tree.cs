namespace Generator
{
    public enum State
    {
        Sign,
        Func,
        Var,
        DoneNode,
        Root
    }
    public class Tree
    {
        public string Value { get; set; }
        public string ValueMD { get; set; }
        public Tree Left { get; set; } = null;
        public Tree Right { get; set; } = null;
        public Tree Parent { get; set; } = null;
        public State State = State.Var;


        public Tree(Tree parent, string value = "x1")
        {
            Value = value;
            ValueMD = value;
            Parent = parent;
        }

    }
}