using Grpc.Core;
using GrpcService.Data;
using GrpcService.Models;
using GrpcService.Protos;
using Microsoft.EntityFrameworkCore;

namespace GrpcService.Services
{
    public class ToDoService : ToDoIt.ToDoItBase
    {
        private readonly AppDbContext _dbContext;

        public ToDoService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Create
        public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
        {
            if (request.Title == string.Empty || request.Description == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

            var toDoItem = new ToDoItem
            {
                Title = request.Title,
                Description = request.Description
            };

            await _dbContext.AddAsync(toDoItem);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new CreateToDoResponse
            {
                Id = toDoItem.Id
            });
        }
        #endregion

        #region ReadById
        public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "resource index must be greater than 0"));

            var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);

            if (toDoItem != null)
            {
                return await Task.FromResult(new ReadToDoResponse
                {
                    Id = toDoItem.Id,
                    Title = toDoItem.Title,
                    Description = toDoItem.Description,
                    ToDoStatus = toDoItem.ToDoStatus
                });

            }

            throw new RpcException(new Status(StatusCode.NotFound, $"No task with id {request.Id}"));
        }
        #endregion

        #region ListToDo
        public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
        {
            var response = new GetAllResponse();
            var toDoItems = await _dbContext.ToDoItems.ToListAsync();

            foreach (var toDoItem in toDoItems)
            {
                response.ToDo.Add(new ReadToDoResponse
                {
                    Id = toDoItem.Id,
                    Title = toDoItem.Title,
                    Description = toDoItem.Description,
                    ToDoStatus = toDoItem.ToDoStatus
                });
            }

            return await Task.FromResult(response);
        }
        #endregion

        #region Update
        public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0 || request.Title == string.Empty || request.Description == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply valid object"));

            var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);

            if (toDoItem is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"No task with id {request.Id}"));

            toDoItem.Title = request.Title;
            toDoItem.Description = request.Description;
            toDoItem.ToDoStatus = request.ToDoStatus;

            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new UpdateToDoResponse
            {
                Id = request.Id,
            });
        }
        #endregion

        public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
        {
            if (request.Id.Equals(0))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "resource index must be graeater than 0"));

            var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);

            if (toDoItem is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"No task with Id {request.Id}"));

            _dbContext.Remove(toDoItem);

            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new DeleteToDoResponse
            {
                Id = request.Id
            });
        }
    }
}
