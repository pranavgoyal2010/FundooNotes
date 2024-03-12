namespace ModelLayer.Response;

public class FundooResponseModel<T>
{
    public bool Success { get; set; } = true;
    public string? Message { get; set; }
    public T? Data { get; set; }

    public string? Token { get; set; }
}
