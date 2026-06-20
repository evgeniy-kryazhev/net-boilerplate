using NetBoilerplate.Domain;
using NetBoilerplate.Shared;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace NetBoilerplate.Infrastructure;

public sealed class MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minio;
    private readonly MinioOptions _options;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    public MinioFileStorageService(IMinioClient minio, IOptions<MinioOptions> options)
    {
        _minio = minio;
        _options = options.Value;
    }

    public async Task PutAsync(
        string bucket,
        string objectName,
        Stream data,
        string contentType,
        CancellationToken token = default)
    {
        await EnsureBucketExistsAsync(bucket, token);

        var args = new PutObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectName)
            .WithStreamData(data)
            .WithObjectSize(data.Length)
            .WithContentType(contentType);

        await _minio.PutObjectAsync(args, token);
    }

    public async Task<Stream> GetAsync(
        string bucket,
        string objectName,
        CancellationToken token = default)
    {
        var memoryStream = new MemoryStream();

        var args = new GetObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectName)
            .WithCallbackStream(stream => stream.CopyTo(memoryStream));

        await _minio.GetObjectAsync(args, token);

        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task DeleteAsync(
        string bucket,
        string objectName,
        CancellationToken token = default)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectName);

        await _minio.RemoveObjectAsync(args, token);
    }

    public async Task<IReadOnlyList<string>> ListObjectNamesAsync(
        string bucket,
        string prefix,
        CancellationToken token = default)
    {
        var objectNames = new List<string>();
        var args = new ListObjectsArgs()
            .WithBucket(bucket)
            .WithPrefix(prefix)
            .WithRecursive(true);

        await foreach (Item item in _minio.ListObjectsEnumAsync(args, token))
        {
            objectNames.Add(item.Key);
        }

        return objectNames;
    }

    public async Task<string> GetPresignedUrlAsync(
        string bucket,
        string objectName,
        int expiryInSeconds = 3600)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectName)
            .WithExpiry(expiryInSeconds);

        return await _minio.PresignedGetObjectAsync(args);
    }

    public async Task<bool> ExistsAsync(
        string bucket,
        string objectName,
        CancellationToken token = default)
    {
        try
        {
            var args = new StatObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectName);

            await _minio.StatObjectAsync(args, token);

            return true;
        }
        catch (ObjectNotFoundException)
        {
            return false;
        }
    }

    private async Task EnsureBucketExistsAsync(string bucket, CancellationToken token)
    {
        if (_initialized)
        {
            return;
        }

        await _initLock.WaitAsync(token);

        try
        {
            if (_initialized)
            {
                return;
            }

            var exists = await _minio.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(bucket), token);

            if (!exists)
            {
                await _minio.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(bucket), token);
            }

            _initialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }
}
