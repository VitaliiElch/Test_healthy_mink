using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTestProject.Data;
using MyTestProject.Models;

namespace MyTestProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HRController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HRController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees([FromQuery] string position)
        {
            if (string.IsNullOrEmpty(position))
            {
                return Ok(await _context.Employees.Include(e => e.Shifts).ToListAsync());
            }

            var employees = await _context.Employees.Include(e => e.Shifts).Where(e => e.Position == position).ToListAsync();
            if (!employees.Any())
            {
                return BadRequest("Position not found");
            }

            return Ok(employees);
        }

        [HttpGet("positions")]
        public IActionResult GetPositions()
        {
            var positions = new List<string> { "Менеджер", "Инженер", "Тестировщик свечей" };
            return Ok(positions);
        }

        // Метод которым добавляем сотрудника
        [HttpPost("add")]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            if (string.IsNullOrEmpty(employee.FirstName) || string.IsNullOrEmpty(employee.LastName) || string.IsNullOrEmpty(employee.Position))
        {
                return BadRequest("Required fields are missing");
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return Ok(employee);
        }

        // изменяем сотрудника 
        [HttpPut("update")]
        public async Task<IActionResult> UpdateEmployee([FromBody] Employee employee)
        {
            if (employee.Id == 0 || !_context.Employees.Any(e => e.Id == employee.Id))
            {
                return BadRequest("Employee not found");
            }

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return Ok(employee);
        }

        // удаляем сотрудника
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return BadRequest("Employee not found");
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
