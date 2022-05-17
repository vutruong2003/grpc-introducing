using gRPCProto;

namespace gRPCWebClient.Data;
public class TodoService
{
    private const string BaseAddress = "https://localhost:7242";
    private readonly Todo.TodoClient _client;
    public TodoService(Todo.TodoClient client)
    {
        _client = client;
    }

    public async Task<List<TodoItem>> GetTodoList()
    {
        var response = await _client.GetListsAsync(new Empty());

        return response.Items.ToList();
    }

    public async Task<TodoItem> AddTodo(TodoItem item)
    {
        var response = await _client.AddTodoAsync(new AddTodoRequest
        {
            Item = item
        });

        return response.Item;
    }
}