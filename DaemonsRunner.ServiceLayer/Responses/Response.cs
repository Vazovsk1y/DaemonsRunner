namespace DaemonsRunner.BuisnessLayer.Responses;

public record Response 
{
	private const string SuccessMessage = "Operation successfully completed.";
	private const string FailMessage = "Operation wasn't successfully completed.";

	public string Description { get; }

	public StatusCode OperationStatus { get; }

	protected Response(string description, StatusCode operationStatus)
	{
		Description = description;
		OperationStatus = operationStatus;
	}

	public static Response Success(string operationDescription = SuccessMessage) => new(operationDescription, StatusCode.Success);

	public static Response Fail(string operationDescription = FailMessage) => new(operationDescription, StatusCode.Fail);

	public static DataResponse<T> Success<T>(T value, string operationDescription = SuccessMessage) => new(value, operationDescription, StatusCode.Success);

	public static DataResponse<T> Fail<T>(string operationDescription = FailMessage) => new(default, operationDescription, StatusCode.Fail);
}

public enum StatusCode
{
	Success,
	Fail,
}
