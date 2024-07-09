namespace Patterns.ObjectPool.Interfaces
{
    public interface IPooleableObject : IPrototype
    {
        public bool Active
        {
            get;
            set;
        }
        public IObjectPool elementPool
        {
            get;
            set;
        }
        public void Reset();
    }
}