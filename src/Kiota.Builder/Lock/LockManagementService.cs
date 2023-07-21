using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Zio;

namespace Kiota.Builder.Lock;

/// <summary>
/// A service that manages the lock file for a Kiota project implemented using the file system.
/// </summary>
public class LockManagementService : ILockManagementService
{
    private readonly IFileSystem fs;

    public LockManagementService(IFileSystem fs)
    {
        ArgumentNullException.ThrowIfNull(fs);
        this.fs = fs;
    }
    private const string LockFileName = "kiota-lock.json";
    /// <inheritdoc/>
    public IEnumerable<string> GetDirectoriesContainingLockFile(string searchDirectory)
    {
        ArgumentException.ThrowIfNullOrEmpty(searchDirectory);
        var files = fs.EnumeratePaths(searchDirectory, LockFileName, SearchOption.AllDirectories, SearchTarget.File);
        return files.Select((path) => path.GetName()).Where(x => !string.IsNullOrEmpty(x)).OfType<string>();
    }
    /// <inheritdoc/>
    public Task<KiotaLock?> GetLockFromDirectoryAsync(string directoryPath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(directoryPath);
        return GetLockFromDirectoryInternalAsync(directoryPath, cancellationToken);
    }
    private async Task<KiotaLock?> GetLockFromDirectoryInternalAsync(string directoryPath, CancellationToken cancellationToken)
    {
        var lockFile = Path.Combine(directoryPath, LockFileName);
        if (fs.FileExists(lockFile))
        {
#pragma warning disable CA2007
            await using var fileStream = fs.OpenFile(lockFile, FileMode.Open, FileAccess.ReadWrite);
#pragma warning restore CA2007
            return await GetLockFromStreamInternalAsync(fileStream, cancellationToken).ConfigureAwait(false);
        }
        return null;
    }
    /// <inheritdoc/>
    public Task<KiotaLock?> GetLockFromStreamAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        return GetLockFromStreamInternalAsync(stream, cancellationToken);
    }
    private static async Task<KiotaLock?> GetLockFromStreamInternalAsync(Stream stream, CancellationToken cancellationToken)
    {
        return await JsonSerializer.DeserializeAsync(stream, context.KiotaLock, cancellationToken).ConfigureAwait(false);
    }
    /// <inheritdoc/>
    public Task WriteLockFileAsync(string directoryPath, KiotaLock lockInfo, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(directoryPath);
        ArgumentNullException.ThrowIfNull(lockInfo);
        return WriteLockFileInternalAsync(directoryPath, lockInfo, cancellationToken);
    }
    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };
    private static readonly KiotaLockGenerationContext context = new(options);
    private async Task WriteLockFileInternalAsync(string directoryPath, KiotaLock lockInfo, CancellationToken cancellationToken)
    {
        var lockFilePath = Path.Combine(directoryPath, LockFileName);
#pragma warning disable CA2007
        await using var fileStream = fs.OpenFile(lockFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
#pragma warning restore CA2007
        // TODO: not working on WASI
        // await JsonSerializer.SerializeAsync(fileStream, lockInfo, context.KiotaLock, cancellationToken).ConfigureAwait(false);
        JsonSerializer.Serialize(fileStream, lockInfo, context.KiotaLock);
    }
    /// <inheritdoc/>
    public Task BackupLockFileAsync(string directoryPath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(directoryPath);
        return BackupLockFileInternalAsync(directoryPath);
    }
    private Task BackupLockFileInternalAsync(string directoryPath)
    {
        var lockFilePath = Path.Combine(directoryPath, LockFileName);
        if (fs.FileExists(lockFilePath))
        {
            var backupFilePath = GetBackupFilePath(directoryPath);
            var targetDirectory = Path.GetDirectoryName(backupFilePath);
            if (string.IsNullOrEmpty(targetDirectory)) return Task.CompletedTask;
            if (!fs.DirectoryExists(targetDirectory))
                fs.CreateDirectory(targetDirectory);
            fs.CopyFile(lockFilePath, backupFilePath, true);
        }
        return Task.CompletedTask;
    }
    /// <inheritdoc/>
    public Task RestoreLockFileAsync(string directoryPath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(directoryPath);
        return RestoreLockFileInternalAsync(directoryPath);
    }
    private Task RestoreLockFileInternalAsync(string directoryPath)
    {
        var lockFilePath = Path.Combine(directoryPath, LockFileName);
        var targetDirectory = Path.GetDirectoryName(lockFilePath);
        if (string.IsNullOrEmpty(targetDirectory)) return Task.CompletedTask;
        if (!fs.DirectoryExists(targetDirectory))
            fs.CreateDirectory(targetDirectory);
        var backupFilePath = GetBackupFilePath(directoryPath);
        if (fs.FileExists(backupFilePath))
        {
            fs.CopyFile(backupFilePath, lockFilePath, true);
        }
        return Task.CompletedTask;
    }
    // private static readonly ThreadLocal<HashAlgorithm> HashAlgorithm = new(SHA256.Create);
    private static string GetBackupFilePath(string outputPath)
    {
        // TODO System.Security.Cryptography is not supported on this platform. we have to pass it through CDI
        // var hashedPath = BitConverter.ToString((HashAlgorithm.Value ?? throw new InvalidOperationException("unable to get hash algorithm")).ComputeHash(Encoding.UTF8.GetBytes(outputPath))).Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
        // Quick hack to see how far we can go :-)
        var hashedPath = outputPath + "-backup";
        return Path.Combine(Path.GetTempPath(), Constants.TempDirectoryName, "backup", hashedPath, LockFileName);
    }
}
