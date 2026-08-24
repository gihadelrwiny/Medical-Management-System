using Clinical.Application.DTOs.Department;
using Clinical.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Clinical.API.Controllers
{
    /// <summary>
    /// Manages clinical department operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        /// <summary>
        /// Retrieves all departments along with their doctor count.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A list of departments.</returns>
        /// <response code="200">Returns the list of departments.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DepartmentDto>))]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll(CancellationToken cancellationToken)
        {
            var departments = await _departmentService.GetAllAsync(cancellationToken);
            return Ok(departments);
        }

        /// <summary>
        /// Retrieves a specific department by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the department.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The department details if found.</returns>
        /// <response code="200">Returns the requested department.</response>
        /// <response code="404">If the department was not found.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DepartmentDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var department = await _departmentService.GetByIdAsync(id, cancellationToken);
            if (department is null) return NotFound();

            return Ok(department);
        }

        /// <summary>
        /// Creates a new department.
        /// </summary>
        /// <param name="dto">The department creation details.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The newly created department.</returns>
        /// <response code="201">Returns the created department.</response>
        /// <response code="400">If the input validation fails.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(DepartmentDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DepartmentDto>> Create([FromBody] CreateDepartmentDto dto, CancellationToken cancellationToken)
        {
            var result = await _departmentService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing department.
        /// </summary>
        /// <param name="id">The unique identifier of the department to update.</param>
        /// <param name="dto">The updated department details.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <response code="204">If the update was successful.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="404">If the department was not found.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentDto dto, CancellationToken cancellationToken)
        {
            var updated = await _departmentService.UpdateAsync(id, dto, cancellationToken);
            if (!updated) return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Deletes a department by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the department to delete.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <response code="204">If the department was successfully deleted.</response>
        /// <response code="404">If the department was not found.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await _departmentService.DeleteAsync(id, cancellationToken);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}