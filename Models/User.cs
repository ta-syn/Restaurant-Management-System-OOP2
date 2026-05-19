using System;
namespace RestaurantManagementSystem.Models;

public abstract class User
{
    private int _id;
    private string _name = string.Empty;
    private string _password = string.Empty;
    private string _role = string.Empty;

    public int Id
    {
        get => _id;
        set
        {
            if (value <= 0) throw new ArgumentException("Id must be a positive number.");
            _id = value;
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Name cannot be empty.");
            _name = value.Trim();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Password cannot be empty.");
            _password = value;
        }
    }

    public string Role
    {
        get => _role;
        protected set => _role = value;
    }

    protected User(int id, string name, string password, string role)
    {
        Id = id;
        Name = name;
        Password = password;
        Role = role;
    }

    public abstract void ShowDashboard();

    public virtual bool Login(string inputPassword)
    {
        return Password == inputPassword;
    }

    public override string ToString() => $"[{Role}] {Name} (ID: {Id})";
}
