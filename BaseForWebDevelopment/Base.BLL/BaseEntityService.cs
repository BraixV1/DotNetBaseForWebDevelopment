using System.Linq.Expressions;
using Base.Contracts.BLL;
using Base.Contracts.DAL;
using Base.Contracts.Domain;

namespace Base.BLL;

public class BaseEntityService<TDalEntity, TBllEntity, TRepository> :
    BaseEntityService<TDalEntity, TBllEntity, TRepository, Guid>,
    IEntityService<TBllEntity>
    where TBllEntity : class, IDomainEntityId
    where TRepository : IEntityRepository<TDalEntity, Guid>
    where TDalEntity : class, IDomainEntityId<Guid>
{
    public BaseEntityService(IUnitOfWork uoW, TRepository repository, IBllMapper<TDalEntity, TBllEntity> mapper) : base(uoW,
        repository, mapper)
    {
    }
}

public class BaseEntityService<TDalEntity, TBllEntity, TRepository, TKey> : IEntityService<TBllEntity, TKey>
    where TRepository : IEntityRepository<TDalEntity, TKey>
    where TKey : IEquatable<TKey>
    where TBllEntity : class, IDomainEntityId<TKey>
    where TDalEntity : class, IDomainEntityId<TKey>
{
    private readonly IUnitOfWork _uoW;
    protected readonly TRepository Repository;
    protected readonly IBllMapper<TDalEntity, TBllEntity> Mapper;

    public BaseEntityService(IUnitOfWork uoW, TRepository repository, IBllMapper<TDalEntity, TBllEntity> mapper)
    {
        _uoW = uoW;
        Repository = repository;
        Mapper = mapper;
    }

    public TBllEntity Add(TBllEntity entity)
    {
        return Mapper.Map(Repository.Add(Mapper.Map(entity)))!;
    }

    public TBllEntity Update(TBllEntity entity)
    {
        return Mapper.Map(Repository.Update(Mapper.Map(entity)))!;
    }

    public int Remove(TBllEntity entity, TKey? userId = default)
    {
        return Repository.Remove(Mapper.Map(entity), userId);
    }

    public int Remove(TKey id, TKey? userId = default)
    {
        return Repository.Remove(id, userId);
    }

    public TBllEntity? FirstOrDefault(TKey id, TKey? userId = default, bool noTracking = true)
    {
        return Mapper.Map(Repository.FirstOrDefault(id, userId, noTracking));
    }

    public IEnumerable<TBllEntity> GetAll(TKey? userId = default, bool noTracking = true)
    {
        return Repository.GetAll(userId, noTracking).Select(e => Mapper.Map(e));
    }

    public bool Exists(TKey id, TKey? userId = default)
    {
        return Repository.Exists(id, userId);
    }

    public async Task<TBllEntity?> FirstOrDefaultAsync(TKey id, TKey? userId = default, bool noTracking = true)
    {
        return Mapper.Map(await Repository.FirstOrDefaultAsync(id, userId, noTracking));
    }

    public async Task<IEnumerable<TBllEntity>> GetAllAsync(TKey? userId = default, bool noTracking = true)
    {
        return (await Repository.GetAllAsync(userId, noTracking)).Select(e => Mapper.Map(e));
    }

    public async Task<bool> ExistsAsync(TKey id, TKey? userId = default)
    {
        return await Repository.ExistsAsync(id, userId);
    }

    public async Task<int> RemoveAsync(TBllEntity entity, TKey? userId = default)
    {
        return await Repository.RemoveAsync(Mapper.Map(entity), userId);
    }

    public async Task<int> RemoveAsync(TKey id, TKey? userId = default)
    {
        return await Repository.RemoveAsync(id, userId);
    }

    public async Task<TBllEntity?> FirstOrDefaultAsyncInclude(TKey id, TKey? userId = default, bool noTracking = true,
        params Expression<Func<TBllEntity, object>>[] includeProperties)
    {
        var dalIncludeProperties = includeProperties.Select(prop =>
            Expression.Lambda<Func<TDalEntity, object>>(prop.Body, prop.Parameters[0]));

        var entity = await Repository.FirstOrDefaultAsyncInclude(id, userId, noTracking, dalIncludeProperties.ToArray());
        return Mapper.Map(entity);
    }


    public async Task<IEnumerable<TBllEntity>> GetAllIncludingAsync(params Expression<Func<TBllEntity, object>>[] includeProperties)
    {
        var dalIncludeProperties = includeProperties.Select(prop =>
            Expression.Lambda<Func<TDalEntity, object>>(prop.Body, prop.Parameters)).ToArray();

        var entities = await Repository.GetAllIncludingAsync(includeProperties: dalIncludeProperties);
        return entities.Select(e => Mapper.Map(e));
    }


    public async Task<IEnumerable<TBllEntity>> GetAllIncludingAsync<TProperty>(Expression<Func<TBllEntity, TProperty>>
        includeExpression, TKey? userId = default, bool noTracking = true)
    {
        var includeProperties = new List<Expression<Func<TDalEntity, object>>>();
        includeProperties.Add(Expression.Lambda<Func<TDalEntity, object>>(includeExpression.Body, includeExpression.Parameters));

        var entities = await Repository.GetAllIncludingAsync(includeProperties.ToArray());
        return entities.Select(e => Mapper.Map(e));
    }

}