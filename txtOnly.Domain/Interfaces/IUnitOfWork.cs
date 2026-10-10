

namespace txtOnly.Domain.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository UserRepository { get; }
    IPostRepository PostRepository { get; }
    ICommentRepository CommentRepository { get; }
    ILikeRepository LikeRepository { get; }
    IFollowRepository FollowRepository { get; }
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
