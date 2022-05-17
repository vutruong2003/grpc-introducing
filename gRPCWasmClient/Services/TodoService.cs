using gRPCProto;

namespace gRPCWasmClient.Services;

public class TodoService : ITodoService
{
    private readonly Todo.TodoClient _todoClient;
    public TodoService(Todo.TodoClient todoClient)
    {
        _todoClient = todoClient;
    }

    public async Task<List<TodoItem>> GetTodoList()
    {
        var response = await _todoClient.GetListsAsync(new Empty());

        return response.Items.ToList();
    }

    public async Task<TodoItem> AddTodo(TodoItem item)
    {
        var response = await _todoClient.AddTodoAsync(new AddTodoRequest
        {
            Item = item
        });

        return response.Item;
    }
}