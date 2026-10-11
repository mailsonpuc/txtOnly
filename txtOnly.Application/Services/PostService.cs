using txtOnly.Application.DTOs;
using txtOnly.Application.Interfaces;
using txtOnly.Application.Mappings;
using txtOnly.Domain.Entities;
using txtOnly.Domain.Interfaces;
using txtOnly.Domain.Pagination;
using txtOnly.Application.Validation;

namespace txtOnly.Application.Services;

public class PostService : IPostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;

    public PostService(IUnitOfWork unitOfWork, IPostRepository postRepository, IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _postRepository = postRepository;
        _userRepository = userRepository;
    }

    public async Task<List<PostDTO>> GetPostsAsync()
    {
        var posts = await _postRepository.GetPostsAsync();
        return posts.Select(post => post.ToPostDTO()).ToList();
    }

    public async Task<PagedList<PostDTO>> GetPostsPagedAsync(int pageNumber, int pageSize)
    {
        var posts = await _postRepository.GetPostsPagedAsync(pageNumber, pageSize);
        var postDtos = posts.Select(post => post.ToPostDTO()).ToList();
        return new PagedList<PostDTO>(postDtos, posts.TotalCount, posts.CurrentPage, posts.PageSize);
    }

    public async Task<PagedList<PostDTO>> GetPostsByUserPagedAsync(Guid userId, int pageNumber, int pageSize)
    {
        var posts = await _postRepository.GetPostsByUserPagedAsync(userId, pageNumber, pageSize);
        var postDtos = posts.Select(post => post.ToPostDTO()).ToList();
        return new PagedList<PostDTO>(postDtos, posts.TotalCount, posts.CurrentPage, posts.PageSize);
    }

    public async Task<PostDTO?> GetByIdAsync(Guid? id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        return post?.ToPostDTO();
    }

    public async Task<PostDTO> CreateAsync(PostCreateDTO post)
    {
        ArgumentNullException.ThrowIfNull(post);
        await EntityValidation.EnsureUserExistsAsync(_userRepository, post.UserId);
        var createdPost = await _postRepository.CreateAsync(post.ToPost());
        await _unitOfWork.CommitAsync();
        return createdPost.ToPostDTO();
    }

    public async Task<PostDTO> UpdateAsync(PostDTO post)
    {
        ArgumentNullException.ThrowIfNull(post);
        var postEntity = await GetEntityByIdAsync(post.PostId);
        if (postEntity.IsDeleted)
            throw new InvalidOperationException("Não é possível editar uma publicação removida.");
        post.ApplyTo(postEntity);
        var updatedPost = await _postRepository.UpdateAsync(postEntity);
        await _unitOfWork.CommitAsync();
        return updatedPost.ToPostDTO();
    }

    public async Task<PostDTO> RemoveAsync(PostDTO post)
    {
        ArgumentNullException.ThrowIfNull(post);
        var postEntity = await GetEntityByIdAsync(post.PostId);
        if (!postEntity.IsDeleted)
            postEntity.Delete();
        var removedPost = await _postRepository.UpdateAsync(postEntity);
        await _unitOfWork.CommitAsync();
        return removedPost.ToPostDTO();
    }

    public async Task<PostDTO> RestoreAsync(Guid id)
    {
        var postEntity = await GetEntityByIdAsync(id);
        if (postEntity.IsDeleted)
            postEntity.Restore();
        var restoredPost = await _postRepository.UpdateAsync(postEntity);
        await _unitOfWork.CommitAsync();
        return restoredPost.ToPostDTO();
    }

    private async Task<Post> GetEntityByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("O ID da publicação é obrigatório.", nameof(id));
        return await _postRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Publicação não encontrada.");
    }

}