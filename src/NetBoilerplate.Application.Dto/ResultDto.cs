namespace NetBoilerplate.Application.Dto;

public class ResultDto<T>(int count = 0)
{
    public List<T> Items { get; set; } = [];
    public int Count { get; set; } = count;
}