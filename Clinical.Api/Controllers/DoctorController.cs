
using Clinical.Application.DTOs.Doctor;
using Clinical.Application.DTOs.Pagination;
using Clinical.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clinical.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorController(IDoctorService doctorService)
    : ControllerBase
{
    /// <summary>
    /// Gets a paginated list of doctors with optional search and sorting.
    /// </summary>
    /// <param name="query">
    /// Pagination, search, and sorting parameters.
    /// </param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated list of doctors.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<DoctorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<DoctorDto>>> GetAll(
        [FromQuery] QueryParams query,
        CancellationToken ct)
    {
        var result = await doctorService.GetAllAsync(query, ct);

        return Ok(result);
    }

    /// <summary>
    /// Gets a doctor by ID.
    /// </summary>
    /// <param name="id">The doctor ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The doctor if found.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorDto>> GetById(
        int id,
        CancellationToken ct)
    {
        var doctor = await doctorService.GetByIdAsync(id, ct);

        if (doctor is null)
            return NotFound();

        return Ok(doctor);
    }

    /// <summary>
    /// Creates a new doctor.
    /// </summary>
    /// <param name="dto">The doctor creation data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created doctor.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DoctorDto>> Create(
      [FromBody] CreateDoctorRequest dto,
      CancellationToken ct)
    {
        var result = await doctorService.CreateAsync(dto, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            result.Value);
    }

    /// <summary>
    /// Updates an existing doctor.
    /// </summary>
    /// <param name="id">The doctor ID.</param>
    /// <param name="dto">The updated doctor data.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateDoctorDto dto,
        CancellationToken ct)
    {
        var existing =
            await doctorService.GetByIdAsync(id, ct);

        if (existing is null)
            return NotFound();

        await doctorService.UpdateAsync(id, dto, ct);

        return NoContent();
    }

    /// <summary>
    /// Deletes a doctor.
    /// </summary>
    /// <param name="id">The doctor ID.</param>
    /// <param name="ct">Cancellation token.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken ct)
    {
        var existing =
            await doctorService.GetByIdAsync(id, ct);

        if (existing is null)
            return NotFound();

        await doctorService.DeleteAsync(id, ct);

        return NoContent();
    }
}