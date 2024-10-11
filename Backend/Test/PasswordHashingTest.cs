using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test;
public class PasswordHashingTest
{
    [Fact]
    public void Test()
    {
        var password = "1234567890";
        var hash = User.HashPassword(password);
        var hash2 = User.HashPassword(password);
        Assert.Equal(hash, hash2);
    }
}
