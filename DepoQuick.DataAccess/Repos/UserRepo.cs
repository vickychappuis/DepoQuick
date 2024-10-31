using DepoQuick.Models;
using Microsoft.EntityFrameworkCore;

namespace DepoQuick.DataAccess.Repos;

public class UserRepo : IRepo<User, int>, IRepo<User, string>
{
    private readonly IDbContextFactory<Context> _contextFactory;

    public UserRepo(IDbContextFactory<Context> context)
    {
        _contextFactory = context;
    }

    public User Add(User user)
    {
        using var context = _contextFactory.CreateDbContext();
        var userEntry = context.Users.Add(user);
        context.SaveChanges();

        return userEntry.Entity;
    }
    
    public List<User> GetAll()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Users.ToList();
    }
    
    public User? Get(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Users.Find(id);
    }

    public void Delete(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var user = context.Users.Find(id);
        
        if (user is null)
            return;
        
        context.Users.Remove(user);
        context.SaveChanges();
    }

    public User Update(User entity)
    {
        using var context = _contextFactory.CreateDbContext();
        var userEntry = context.Users.Update(entity);
        context.SaveChanges();
        
        return userEntry.Entity;
    }

    public User? Get(string email)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Users.FirstOrDefault(u => u.Email == email);
    }
    
    public void Delete(string email)
    {
        using var context = _contextFactory.CreateDbContext();
        var user = context.Users.FirstOrDefault(u => u.Email == email);
        
        if (user is null)
            return;
        
        context.Users.Remove(user);
        context.SaveChanges();
    }
}