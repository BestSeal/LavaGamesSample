namespace Sources.Interfaces
{
    public interface IFillable<in T> where T : class
    {
        public bool IsFilled { get; }

        public void Fill(T placeable);
    }
}