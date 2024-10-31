using DepoQuick.Backend.Models;
using DepoQuick.Backend.Providers;

namespace DepoQuick.Backend.Repos;

public class UserRepo : IRepo<User>
{
    private InMemoryDatabase _db;

    public UserRepo(InMemoryDatabase database)
    {
        _db = database;
    }
    
    public void Add(User user)
    {
        _db.Users.Add(user);
    }
    
    public List<User> GetAll()
    {
        return _db.Users;
    }
    
    public User? Get(int id)
    {
        return _db.Users.Find(u => u.Id == id);
    }

    public User? GetByEmail(string email)
    {
        return _db.Users.Find(u => u.Email == email);
    }
}