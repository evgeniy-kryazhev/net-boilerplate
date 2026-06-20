namespace NetBoilerplate.Application.Dto;

public class ErrorResultDto(int code, string message)
{
    public int Code { get; set; } = code;
    public string Message { get; set; } = message;
}