namespace GPool.src
{
    public interface IPoolable
    {
        public bool IsActive { get; protected set; }

        void OnReset();

        void Take() { IsActive = true; }

        void Reset() { IsActive = false; OnReset(); }
    }
}
