using Models;

namespace Test;

public class UserModelTest
{
    [Fact]
    public void Insert()
    {
        var factory = Utils.GetConnectionFactory();
        var userModel = new UserModel(factory);

        var id = userModel.Insert(Utils.CreateUser());
        Assert.True(id > 0);
    }

    [Fact]
    public void Get()
    {
        var factory = Utils.GetConnectionFactory();
        var userModel = new UserModel(factory);

        var id = userModel.Insert(Utils.CreateUser());
        Assert.True(id > 0);

        var u = userModel.Get(id);
        Assert.NotNull(u);
    }
}