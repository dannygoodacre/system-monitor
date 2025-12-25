namespace SystemMonitor.Application.Resources;

public interface IResource
{
    /// <summary>
    /// The unique name of the resource.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// How often to query the status of the resource.
    /// </summary>
    public int CheckFrequencyInSeconds { get; }

    /// <summary>
    /// How often to send warning emails.
    /// </summary>
    public int EmailFrequencyInSeconds { get; }

    /// <summary>
    /// Get the status of the resource.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while performing the operation.</param>
    /// <returns>The current status.</returns>
    Task<string> GetStatusInformationAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Return true if the resource is okay; else, return false.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while performing the operation.</param>
    /// <returns>A <see cref="bool"/> indicating the status of the resource.</returns>
    Task<bool> IsOkayAsync(CancellationToken cancellationToken = default);
}
