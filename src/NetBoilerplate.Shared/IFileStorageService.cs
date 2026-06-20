namespace NetBoilerplate.Shared;

public interface IFileStorageService
{
    Task PutAsync(
        string bucket,
        string objectName,
        Stream data,
        string contentType,
        CancellationToken token = default);

    Task<Stream> GetAsync(
        string bucket,
        string objectName,
        CancellationToken token = default);

    Task DeleteAsync(
        string bucket,
        string objectName,
        CancellationToken token = default);

    Task<IReadOnlyList<string>> ListObjectNamesAsync(
        string bucket,
        string prefix,
        CancellationToken token = default);

    Task<string> GetPresignedUrlAsync(
        string bucket,
        string objectName,
        int expiryInSeconds = 3600);

    Task<bool> ExistsAsync(
        string bucket,
        string objectName,
        CancellationToken token = default);
}
