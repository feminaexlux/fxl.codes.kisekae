namespace fxl.codes.kisekae.Models;

public class ErrorViewModel
{
    public string RequestId { get; init; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}