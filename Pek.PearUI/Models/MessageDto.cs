namespace Pek.PearUI.Models;

/// <summary>
/// 
/// </summary>
public class MessageDto
{
    /// <summary>
    /// 
    /// </summary>
    public Int32 id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public String? title { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public IList<MessageChild>? children { get; set; }
}

/// <summary>
/// 
/// </summary>
public class MessageChild
{
    /// <summary>
    /// 
    /// </summary>
    public Int32 id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public String? avatar { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public String? title { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public String? context { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public String? form { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public String? time { get; set; }
}