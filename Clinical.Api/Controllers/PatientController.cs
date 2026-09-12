using Clinical.Application.DTOs.Pagination;

using Clinical.Application.DTOs.Patients;
using Clinical.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clinical.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientController(IPatientService patientService)
    : ControllerBase
{
    /// <summary>
    /// Gets a paginated list of patients with optional search and sorting.
    /// </summary>
    /// <param name="query">
    /// Pagination, search, and sorting parameters.
    /// </param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated list of patients.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PatientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PatientDto>>> GetAll(
        [FromQuery] QueryParams query,
        CancellationToken ct)
    {
        var result = await patientService.GetAllAsync(query, ct);

        return Ok(result);
    }

    /// <summary>
    /// Gets a patient by ID.
    /// </summary>
    /// <param name="id">The patient ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The patient if found.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> GetById(
        int id,
        CancellationToken ct)
    {
        var patient = await patientService.GetByIdAsync(id, ct);

        if (patient is null)
            return NotFound();

        return Ok(patient);
    }

    /// <summary>
    /// Updates an existing patient's profile information.
    /// </summary>
    /// <param name="id">The patient ID.</param>
    /// <param name="dto">The updated patient data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content if the update succeeded.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdatePatientDto dto,
        CancellationToken ct)
    {
        var existing =
            await patientService.GetByIdAsync(id, ct);

        if (existing is null)
            return NotFound();

        await patientService.UpdateAsync(id, dto, ct);

        return NoContent();
    }

    /// <summary>
    /// Deletes a patient.
    /// </summary>
    /// <param name="id">The patient ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content if the deletion succeeded.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken ct)
    {
        var existing =
            await patientService.GetByIdAsync(id, ct);

        if (existing is null)
            return NotFound();

        await patientService.DeleteAsync(id, ct);

        return NoContent();
    }
}