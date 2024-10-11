using Models;

namespace Api.DTOs;

public class UserDTO(User user)
{
    public int Id { get; set; } = user.Id;
    public string Login { get; set; } = user.Login;
}