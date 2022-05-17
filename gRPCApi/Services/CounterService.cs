using Grpc.Core;
using gRPCProto;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace gRPCApi.Services
{
    public class CounterService : Counter.CounterBase
    {
        private const string Redis_Counter = "gRPCCounter";

        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly static ConcurrentDictionary<string, IServerStreamWriter<CounterResponse>> _clientStreams = new ConcurrentDictionary<string, IServerStreamWriter<CounterResponse>>();

        public CounterService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = _redis.GetDatabase();
        }

        public override async Task<CounterResponse> DoCount(CountRequest request, ServerCallContext context)
        {
            var amount = request.Amount;

            var count = await _database.StringIncrementAsync(Redis_Counter, amount);

            await SendCounting((int)count);

            return new CounterResponse
            {
                Count = (int)count
            };
        }

        public override async Task StartCounter(CounterRequest request, IServerStreamWriter<CounterResponse> responseStream, ServerCallContext context)
        {
            var count = await _database.StringGetAsync(Redis_Counter);

            var id = Guid.NewGuid().ToString();
            _clientStreams.GetOrAdd(id, responseStream);
            
            await responseStream.WriteAsync(new CounterResponse
            {
                Count = ((int)count)
            });

            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(300));
            }

            _clientStreams.Remove(id, out var stream);
        }

        public override async Task<CounterResponse> ResetCounter(CounterRequest request, ServerCallContext context)
        {
            var count = await _database.StringSetAsync(Redis_Counter, request.Start);

            if (count)
            {
                await SendCounting(request.Start);
            }

            return new CounterResponse
            {
                Count = request.Start
            };
        }

        private async Task SendCounting(int count)
        {
            foreach (var stream in _clientStreams.Values)
            {
                try
                {
                    await stream.WriteAsync(new CounterResponse
                    {
                        Count = count
                    });
                }
                catch (Exception ex)
                {
                    // ingore
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
