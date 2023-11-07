namespace EntityGuardian.Entities.Dtos;

public class ResponseData<T>
{
    public ResponseData(T resultObject, int dataCount)
    {
        ResultObject = resultObject;
        DataCount = dataCount;
    }

    public T ResultObject { get; set; }
    public int DataCount { get; set; }
}