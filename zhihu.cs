using System.Collections.Generic;


public class Question
{
    /// <summary>
    /// 
    /// </summary>
    public string type { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int id { get; set; }
    /// <summary>
    /// 山东的你，择偶的标准是怎样的？
    /// </summary>
    public string title { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string question_type { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int created { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int updated_time { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string url { get; set; }
}


public class Author
{
    /// <summary>
    /// 
    /// </summary>
    public string id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string url_token { get; set; }
    /// <summary>
    /// 韩阿P
    /// </summary>
    public string name { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string avatar_url { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string avatar_url_template { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string is_org { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string type { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string url { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string user_type { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string headline { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int gender { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string is_advertiser { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int follower_count { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string is_followed { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string is_privacy { get; set; }
}

public class DataItem
{
    /// <summary>
    /// 
    /// </summary>
    public int id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string type { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string answer_type { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Question question { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Author author { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string url { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string is_collapsed { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int created_time { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int updated_time { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string extras { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string is_copyable { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string is_normal { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int voteup_count { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int comment_count { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string is_sticky { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string admin_closed_comment { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string comment_permission { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string reshipment_settings { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string content { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string editable_content { get; set; }
    /// <summary>
    /// 你的意中人是一个山东公务员，有一天，他会在驾驶台摆着国旗和党旗，驾着黑色奥迪A6来娶你。 车后备箱装满单位发的米和油，还有六个核桃外加两条中华烟，几瓶茅台干红和全套渔具。
    /// </summary>
    public string excerpt { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string collapsed_by { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string collapse_reason { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string annotation_action { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<string> mark_infos { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string ad_answer { get; set; }
}

public class Paging
{
    /// <summary>
    /// 
    /// </summary>
    public string is_end { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string is_start { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string next { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string previous { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int totals { get; set; }
}

public class zhihu
{
    /// <summary>
    /// 
    /// </summary>
    public List<DataItem> data { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Paging paging { get; set; }
}