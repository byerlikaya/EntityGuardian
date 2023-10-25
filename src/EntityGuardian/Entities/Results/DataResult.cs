namespace EntityGuardian.Entities.Results
{
    public class DataResult<T> : IDataResult<T>
    {
        public DataResult(T resultObject, int dataCount)
        {
            ResultObject = resultObject;
            DataCount = dataCount;
        }

        public T ResultObject { get; set; }
        public int DataCount { get; set; }
    }
}