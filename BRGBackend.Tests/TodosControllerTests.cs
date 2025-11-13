using Xunit;
using BRGBackend.Controllers;
using BRGBackend.Data;
using BRGBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BRGBackend.Tests
{
    public class TodosControllerTests
    {
        private AppDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            // Dados iniciais
            for (int i = 1; i <= 8; i++)
            {
                context.Todos.Add(new Todo
                {
                    UserId = 1,
                    Id = i,
                    Title = $"Tarefa {i}",
                    Completed = i % 2 == 0
                });
            }
            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task GetTodos_Pagination_Works()
        {
            var context = GetContext();
            var controller = new TodosController(context);

            var result = await controller.GetTodos(page: 1, pageSize: 3, null, "id", "asc") as OkObjectResult;

            var json = System.Text.Json.JsonSerializer.Serialize(result!.Value);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            int total = root.GetProperty("total").GetInt32();
            var todos = root.GetProperty("todos").EnumerateArray().ToList();

            Assert.Equal(3, todos.Count);
            Assert.Equal(8, total);
        }

        [Fact]
        public async Task PutTodo_Rejects_More_Than_5_Incomplete()
        {
            var context = GetContext();
            var controller = new TodosController(context);


            // Marca 5 tarefas como incompletas
            var allTasks = context.Todos.Where(t => t.UserId == 1).ToList();
            foreach (var t in allTasks.Take(5))
                t.Completed = false; // incompletas

            foreach (var t in allTasks.Skip(5))
                t.Completed = true; // completas

            context.SaveChanges();

            var sixth = allTasks.Skip(5).First();


            var result = await controller.UpdateTodoCompleted(sixth.Id, false);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Contains("5 tarefas incompletas", badRequest.Value.ToString());
        }

        [Fact]
        public async Task PutTodo_Allows_Completed_To_Incomplete_If_Less_Than_5()
        {
            var context = GetContext();
            var controller = new TodosController(context);

            var todo = context.Todos.First(t => t.Completed);
            var result = await controller.UpdateTodoCompleted(todo.Id, false);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}
