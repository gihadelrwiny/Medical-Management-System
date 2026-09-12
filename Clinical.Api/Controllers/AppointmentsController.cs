using Clinical.Application.DTOs.Appoinment;
using Clinical.Application.DTOs.Pagination;
using Clinical.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Clinical.API.Controllers
{
    /// <summary>
    /// Manages patient appointment operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        /// <summary>
        /// Retrieves a paged, filtered, and sorted list of appointments.
        /// </summary>
        /// <param name="query">Pagination, search, and sort parameters.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A paged list of appointments.</returns>
        /// <response code="200">Returns the paged list of appointments.</response>
        [HttpGet]
        public async Task<ActionResult<PagedResult<AppointmentDto>>> GetAll(
            [FromQuery] QueryParams query,
            CancellationToken cancellationToken)
        {
            var result = await _appointmentService.GetAllAsync(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific appointment by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the appointment.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The appointment details if found.</returns>
        /// <response code="200">Returns the requested appointment.</response>
        /// <response code="404">If the appointment was not found.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AppointmentDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppointmentDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentService.GetByIdAsync(id, cancellationToken);
            if (appointment is null) return NotFound();

            return Ok(appointment);
        }

        /// <summary>
        /// Books a new appointment for a patient with a doctor.
        /// </summary>
        /// <param name="dto">The appointment creation details.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The newly created appointment.</returns>
        /// <response code="201">Returns the created appointment.</response>
        /// <response code="400">If the input validation fails.</response>
        /// <response code="409">If the doctor already has an appointment at that time.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AppointmentDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AppointmentDto>> Create([FromBody] CreateAppointmentDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _appointmentService.CreateAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing appointment.
        /// </summary>
        /// <param name="id">The unique identifier of the appointment to update.</param>
        /// <param name="dto">The updated appointment details.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <response code="204">If the update was successful.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="404">If the appointment was not found.</response>
        /// <response code="409">If the doctor already has an appointment at that time.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var updated = await _appointmentService.UpdateAsync(id, dto, cancellationToken);
                if (!updated) return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Deletes an appointment by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the appointment to delete.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <response code="204">If the appointment was successfully deleted.</response>
        /// <response code="404">If the appointment was not found.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await _appointmentService.DeleteAsync(id, cancellationToken);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}