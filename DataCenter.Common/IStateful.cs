namespace DataCenter.Common
{
    public interface IStateful { }

    public interface IStateful<in TKey, in TState> : IStateful
    {
        bool IsValid(TKey key);
    }
}