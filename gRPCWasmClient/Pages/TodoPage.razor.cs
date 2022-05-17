using gRPCWasmClient.Services;
using Microsoft.AspNetCore.Components;

using gRPCProto;

namespace gRPCWasmClient.Pages;

public partial class TodoPage
{
    [Inject]
    protected ITodoService TodoService { get; set; }

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