using Base.Contracts.BLL;
using Base.Contracts.DAL;
using Microsoft.EntityFrameworkCore;

namespace Base.BLL;

public abstract class BaseBll<TAppDbContext> : IBll
    where TAppDbContext : DbContext
{
    protected readonly IUnitOfWork UoW;

    protected BaseBll(IUnitOfWork uoW)
    {
        UoW = uoW;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await UoW.SaveChangesAsync();
    }
}