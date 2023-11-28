namespace EntityGuardian.Entities.Dtos;

public class ResponseData<T>(T resultObject, int dataCount)
{
    public T ResultObject { get; set; } = resultObject;
    public int DataCount { get; set; } = dataCount;
}