using Models;

namespace Test;

public class EventModelTest
{
    //private static int InsertEvent(ISqliteConnectionFactory factory)
    //{
    //    var userModel = new UserModel(factory);
    //    var userId = userModel.Insert(Utils.CreateUser());
    //    Assert.True(userId > 0);

    //    var eventModel = new EventModel(factory);
    //    var _event = Utils.CreateEvent();
    //    _event.OraganizerId = userId;

    //    return eventModel.Insert(_event);
    //}

    //[Fact]
    //public void Insert()
    //{
    //    var factory = Utils.GetConnectionFactory();

    //    var eventId = InsertEvent(factory);
    //    Assert.True(eventId > 0);
    //}

    //[Fact]
    //public void Get()
    //{
    //    var factory = Utils.GetConnectionFactory();
    //    var eventModel = new EventModel(factory);

    //    var eventId = InsertEvent(factory);
    //    var e = eventModel.Get(eventId);
    //    Assert.NotNull(e);
    //}
}
