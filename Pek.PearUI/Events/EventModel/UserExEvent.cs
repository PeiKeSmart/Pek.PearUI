namespace DH.Cube.Events.EventModel;

/// <summary>
/// 扩展用户消费者事件
/// </summary>
public class UserExEvent
{

    public UserExEvent(int uId)
    {
        UId = uId;
    }

    public int? UId { get; set; }
}
