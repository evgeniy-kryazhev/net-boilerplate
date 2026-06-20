namespace NetBoilerplate.Shared;

public record PagedResultDto<T>(List<T> Hits, int TotalHits);