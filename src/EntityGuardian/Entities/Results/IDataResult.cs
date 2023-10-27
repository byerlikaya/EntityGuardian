namespace EntityGuardian.Entities.Results
{
    internal interface IDataResult<out T>
    {
        T ResultObject { get; }

        int DataCount { get; }
    }
}