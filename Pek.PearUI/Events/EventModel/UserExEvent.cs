namespace Pek.PearUI.Events.EventModel;

/// <summary>
/// 扩展用户消费者事件
/// </summary>
public class UserExEvent
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="uId"></param>
    public UserExEvent(Int32 uId)
    {
        UId = uId;
    }

    /// <summary>
    /// 用户ID
    /// </summary>
    public Int32 UId { get; set; }
}
