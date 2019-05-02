namespace TechStore.DAL.Interfaces
{
    public interface IUpdater<in T>
    {
        void Update(T item);
    }
}
