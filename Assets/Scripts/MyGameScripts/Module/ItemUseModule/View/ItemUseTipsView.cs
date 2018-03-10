public partial interface IItemUseTipsView
{
//在这里添加自定义接口
    UIGrid ItemGrid { get; }

    void SetTitle(string title);
    void SetContent(string content);
}

public sealed partial class ItemUseTipsView
{
    protected override void OnDispose()
    {

    }

    public void SetTitle(string title)
    {
        TitleLabel_UILabel.text = title;
    }
    public void SetContent(string content)
    {
        ContentLabel_UILabel.text = content;
    }

    public UIGrid ItemGrid {
        get { return itemGrid_UIGrid; }
    }
}
