using Clinical.Application.DTOs.DoctorSchedule;
using Clinical.Application.DTOs.Pagination;
using Clinical.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Clinical.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorSchedulesController : ControllerBase
    {
        private readonly IDoctorScheduleService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorSchedulesController"/> class.
        /// </summary>
        /// <param name="service">The doctor schedule service.</param>
        public DoctorSchedulesController(IDoctorScheduleService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets a paged list of doctor schedules, optionally filtered and sorted.
        /// </summary>
        /// <param name="query">Pagination, search, and sorting parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paged result of <see cref="DoctorScheduleDto"/>.</returns>
        /// <response code="200">Returns the paged list of doctor schedules.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<DoctorScheduleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<DoctorScheduleDto>>> GetAll(
            [FromQuery] QueryParams query,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetAllAsync(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets a single doctor schedule by its identifier.
        /// </summary>
        /// <param name="id">The doctor schedule identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The matching <see cref="DoctorScheduleDto"/>.</returns>
        /// <response code="200">Returns the requested doctor schedule.</response>
        /// <response code="404">No doctor schedule was found with the given id.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DoctorScheduleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorScheduleDto>> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Creates a new doctor schedule entry.
        /// </summary>
        /// <param name="dto">The doctor schedule data to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The newly created <see cref="DoctorScheduleDto"/>.</returns>
        /// <response code="201">The doctor schedule was created successfully.</response>
        /// <response code="400">The request data was invalid (e.g. StartTime not before EndTime).</response>
        /// <response code="409">The schedule overlaps with an existing one for the doctor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(DoctorScheduleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DoctorScheduleDto>> Create(
            [FromBody] CreateDoctorScheduleDto dto,
            CancellationToken cancellationToken)
        {
            var created = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing doctor schedule entry.
        /// </summary>
        /// <param name="id">The doctor schedule identifier.</param>
        /// <param name="dto">The updated schedule data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">The doctor schedule was updated successfully.</response>
        /// <response code="400">The request data was invalid (e.g. StartTime not before EndTime).</response>
        /// <response code="404">No doctor schedule was found with the given id.</response>
        /// <response code="409">The schedule overlaps with an existing one for the doctor.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateDoctorScheduleDto dto,
            CancellationToken cancellationToken)
        {
            var updated = await _service.UpdateAsync(id, dto, cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Soft-deletes a doctor schedule entry.
        /// </summary>
        /// <param name="id">The doctor schedule identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">The doctor schedule was deleted successfully.</response>
        /// <response code="404">No doctor schedule was found with the given id.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            var deleted = await _service.DeleteAsync(id, cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
