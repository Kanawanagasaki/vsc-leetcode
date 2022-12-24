namespace Kanawanagasaki.VSCode.LeetCode.WebView.Components;

using Microsoft.AspNetCore.Components;

public partial class Dropdown : ComponentBase
{
    [Parameter, EditorRequired] public required string Title { get; init; }
    [Parameter, EditorRequired] public required RenderFragment ChildContent { get; init; }
    [Parameter] public EventCallback Unselected { get; set; }
    [Parameter] public bool IsMultiSelect { get; set; } = false;
    [Parameter] public bool IsAbsolute { get; set; } = false;

    public bool IsCollapsed { get; private set; } = true;

    private DropdownItem? _activeItem = null;
    private List<DropdownItem> _items = new();
    private List<DropdownItem> _multiselectedItems = new();

    public void AddItem(DropdownItem item)
    {
        if (item.IsSelected)
        {
            if (IsMultiSelect)
                _multiselectedItems.Add(item);
            else
                _activeItem = item;
        }
        _items.Add(item);
        StateHasChanged();
    }

    private async Task OnItemClick(DropdownItem item)
    {
        if (IsMultiSelect)
        {
            if (_multiselectedItems.Contains(item))
            {
                _multiselectedItems.Remove(item);
                await item.Unselected.InvokeAsync();
            }
            else
            {
                _multiselectedItems.Add(item);
                await item.Selected.InvokeAsync();
            }
        }
        else
        {
            if (_activeItem == item)
            {
                _activeItem = null;
                await item.Unselected.InvokeAsync();
                await Unselected.InvokeAsync();
            }
            else
            {
                _activeItem = item;
                await item.Selected.InvokeAsync();
            }
        }
    }

    public void Collapse()
    {
        IsCollapsed = true;
        StateHasChanged();
    }

    public void Show()
    {
        IsCollapsed = false;
        StateHasChanged();
    }
}

public class DropdownItem : ComponentBase
{
    [CascadingParameter] public required Dropdown Parent { get; init; }
    [Parameter, EditorRequired] public required RenderFragment ChildContent { get; init; }
    [Parameter] public bool IsSelected { get; set; } = false;
    [Parameter] public EventCallback Selected { get; set; }
    [Parameter] public EventCallback Unselected { get; set; }

    protected override void OnInitialized()
    {
        Parent.AddItem(this);
    }
}
