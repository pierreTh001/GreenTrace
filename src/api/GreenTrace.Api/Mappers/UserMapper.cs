using GreenTrace.Api.Infrastructure.Entities;
using GreenTrace.Api.ViewModels.Users;

namespace GreenTrace.Api.Mappers;

public static class UserMapper
{
    public static UserViewModel ToViewModel(this User user) =>
        new(user.Id, user.Email, user.FirstName, user.LastName);

    public static void MapTo(this UpdateUserViewModel vm, User user)
    {
        user.FirstName = vm.FirstName;
        user.LastName = vm.LastName;
    }
}
