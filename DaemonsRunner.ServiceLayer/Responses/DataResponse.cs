namespace DaemonsRunner.BuisnessLayer.Responses;

public record DataResponse<T> : Response
{
	private readonly T? _data;
	public T Data
	{
		get
		{
			if (OperationStatus == StatusCode.Fail)
			{
				throw new InvalidOperationException("Cannot acessed data from failed response.");
			}

			return _data!;
		}
	}

	internal DataResponse(T? data, string description, StatusCode operationStatus) : base(description, operationStatus) => _data = data;
}

