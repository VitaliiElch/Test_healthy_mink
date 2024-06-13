using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTestProject.Data;
using MyTestProject.Models;

namespace MyTestProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckpointController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CheckpointController(ApplicationDbContext context)
        {
            _context = context;
        }

        // сотрудник зашел на завод и вышел не отметившись

        [HttpPost("startShift")]
        public async Task<IActionResult> StartShift(int employeeId, DateTime startTime)
        {
            var employee = await _context.Employees.Include(e => e.Shifts).FirstOrDefaultAsync(e => e.Id == employeeId);
            if (employee == null)
            {
                return BadRequest("Employee not found");
            }

            if (employee.Shifts.Any(s => s.EndTime == DateTime.MinValue))
            {
                return BadRequest("Previous shift not closed");
            }

            var shift = new Shift
            {
                StartTime = startTime,
                EmployeeId = employeeId
            };
            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();

            return Ok(shift);
        }

        // сотрудник попал на завод не пробив пропуск

        [HttpPost("endShift")]
        public async Task<IActionResult> EndShift(int employeeId, DateTime endTime)
        {
            var employee = await _context.Employees.Include(e => e.Shifts).FirstOrDefaultAsync(e => e.Id == employeeId);
            if (employee == null)
            {
                return BadRequest("Employee not found");
            }

            var shift = employee.Shifts.FirstOrDefault(s => s.EndTime == DateTime.MinValue);
            if (shift == null)
            {
                return BadRequest("No open shift found");
            }

            shift.EndTime = endTime;
            shift.HoursWorked = (endTime - shift.StartTime).TotalHours;
            await _context.SaveChangesAsync();

            return Ok(shift);
        }
    }
}
