

namespace txtOnly.Domain.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{

    Task<int> CommitAsync(CancellationToken cancellationToken = default);

}
