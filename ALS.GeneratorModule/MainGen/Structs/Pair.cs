namespace Generator.MainGen.Structs
{
    public sealed class Pair<T1, T2>
    {
        public T1 First { get; set; } = default(T1);
        public T2 Second { get; set; } = default(T2);
        
        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }
}