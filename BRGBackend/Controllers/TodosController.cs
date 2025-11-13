using BRGBackend.Data;
using BRGBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BRGBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public TodosController(AppDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        // GET /todos?page=1&pageSize=10&title=expedita&sort=title&order=asc
        [HttpGet]
        public async Task<IActionResult> GetTodos(
            int page = 1,
            int pageSize = 10,
            string? title = null,
            string sort = "id",
            string order = "asc")
        {
            var query = _context.Todos.AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(t => t.Title!.Contains(title));

            // Ordenação
            query = (sort.ToLower(), order.ToLower()) switch
            {
                ("title", "asc") => query.OrderBy(t => t.Title),
                ("title", "desc") => query.OrderByDescending(t => t.Title),
                ("completed", "asc") => query.OrderBy(t => t.Completed),
                ("completed", "desc") => query.OrderByDescending(t => t.Completed),
                _ => query.OrderBy(t => t.Id)
            };

            var total = await query.CountAsync();
            var todos = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new { total, page, pageSize, todos });
        }

        // GET /todos/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoById(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null) return NotFound();
            return Ok(todo);
        }

        // PUT /todos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoCompleted(int id, [FromBody] bool completed)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
                return NotFound();

            if (!completed)
            {
                var incompleteCount = await _context.Todos
                    .CountAsync(t => t.UserId == todo.UserId && !t.Completed && t.Id != id);

                if (incompleteCount >= 5)
                {
                    return BadRequest(new
                    {
                        message = $"O usuário {todo.UserId} já possui 5 tarefas incompletas."
                    });
                }
            }

            todo.Completed = completed;
            await _context.SaveChangesAsync();

            return Ok(todo);
        }


        // POST /sync
        [HttpPost("sync")]
        public async Task<IActionResult> SyncTodos()
        {
            var url = "https://jsonplaceholder.typicode.com/todos";
            var todos = await _httpClient.GetFromJsonAsync<List<Todo>>(url);

            if (todos == null) return BadRequest("Erro ao obter dados da API externa.");

            // Limpa e insere
            _context.Todos.RemoveRange(_context.Todos);
            await _context.SaveChangesAsync();

            await _context.Todos.AddRangeAsync(todos);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sincronização concluída", count = todos.Count });
        }
    }
}
