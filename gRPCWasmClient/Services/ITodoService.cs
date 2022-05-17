using gRPCProto;

namespace gRPCWasmClient.Services;

public interface ITodoService
{
    Task<List<TodoItem>> GetTodoList();
    Task<TodoItem> AddTodo(TodoItem item);
}