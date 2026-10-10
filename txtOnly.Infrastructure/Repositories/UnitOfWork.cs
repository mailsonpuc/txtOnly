
using txtOnly.Domain.Interfaces;
using txtOnly.Infrastructure.Context;

namespace txtOnly.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private IUserRepository? _userRepository;
    private IPostRepository? _postRepository;
    private ICommentRepository? _commentRepository;
    private ILikeRepository? _likeRepository;
    private IFollowRepository? _followRepository;
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

    public IPostRepository PostRepository => _postRepository ??= new PostRepository(_context);

    public ICommentRepository CommentRepository => _commentRepository ??= new CommentRepository(_context);

    public ILikeRepository LikeRepository => _likeRepository ??= new LikeRepository(_context);

    public IFollowRepository FollowRepository => _followRepository ??= new FollowRepository(_context);

    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }
}
