namespace EntityGuardian.Entities.Results
{
    public interface IDataResult<out T>
    {
        T ResultObject { get; }

        int DataCount { get; }
    }
}