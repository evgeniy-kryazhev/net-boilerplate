namespace NetBoilerplate.Shared;

public class PagedInputDto(int skip, int take)
{
    public int Skip { get; set; } = skip;
    public int Take { get; set; } = take;
}