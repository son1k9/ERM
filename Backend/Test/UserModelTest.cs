using Models;

namespace Test;

public class UserModelTest
{
    [Fact]
    public void Insert()
    {
        var factory = Utils.GetConnectionFactory();
        var usersModel = new UserModel(factory);

        var user = new User
        {
            Email = "test@test.test",
            Login = "test_login",
            Phone = "89209004534",
            Password = "password"
        };
        var id = usersModel.Insert(user);
        Assert.True(id > 0);
    }

    [Fact]
    public void Get()
    {
        var factory = Utils.GetConnectionFactory();
        var usersModel = new UserModel(factory);

        var user = new User
        {
            Email = "test@test.test",
            Login = "test_login",
            Phone = "89209004534",
            Password = "password"
        };
        var id = usersModel.Insert(user);
        var u = usersModel.Get(id);
        Assert.NotNull(u);
    }
}