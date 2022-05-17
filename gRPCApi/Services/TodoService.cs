using Grpc.Core;
using gRPCProto;
using StackExchange.Redis;
using System.Text.Json;

namespace gRPCApi.Services;

public class TodoService : Todo.TodoBase
{
    private readonly ILogger<TodoService> _logger;
    private readonly static TodoList _itemList = new TodoList();

    private readonly IConnectionMultiplexer _redis;

    public TodoService(ILogger<TodoService> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis;
    }

    public override async Task<TodoList> GetLists(Empty request, ServerCallContext context)
    {
        var db = _redis.GetDatabase();
        var todoItems = await db.ListRangeAsync("gRPCApi:todo");
        var _itemList = new TodoList();
        _itemList.Items.AddRange(todoItems
            .Select(x => JsonSerializer
                .Deserialize<TodoItem>(x.ToString())
            )
            .ToList()
        );

        return _itemList;
    }

    public override async Task<AddTodoResponse> AddTodo(AddTodoRequest request, ServerCallContext context)
    {
        var todoItem = new TodoItem
        {
            Content = request.Item.Content,
            Id = Guid.NewGuid().ToString(),
            Status = request.Item.Status         
        };

        var db = _redis.GetDatabase();
        await db.ListRightPushAsync("gRPCApi:todo", new RedisValue[] { JsonSerializer.Serialize(todoItem) });

        return new AddTodoResponse
        {
            Item = todoItem
        };
    }

    public override Task<Empty> UpdateTodo(UpdateTodoRequest request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> RemoveTodo(RemoveTodoRequest request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }
}
