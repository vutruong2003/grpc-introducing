using Microsoft.AspNetCore.Components;
using gRPCWebClient.Data;
using gRPCProto;

namespace gRPCWebClient.Pages;

public partial class Todo
{
    [Inject]
    public TodoService TodoService { get; set; }

    private TodoItem _item;
    private List<TodoItem> _items;

    protected override async Task OnInitializedAsync()
    {
        _item = new TodoItem();
        _items = await TodoService.GetTodoList();

        await base.OnInitializedAsync();
    }

    private async Task AddTodo()
    {
        if (string.IsNullOrEmpty(_item.Content)) return;

        var item = await TodoService.AddTodo(_item);

        _items.Add(item);
        _item.Content = "";
    }
}

