using Clinical.Application.DTOs.MedicalRecord;
using Clinical.Application.DTOs.Pagination;
using Clinical.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Clinical.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController(IMedicalRecordService medicalRecordService)
    : ControllerBase
    {
        /// <summary>
        /// Gets a paginated list of medical records with optional search and sorting.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<MedicalRecordDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<MedicalRecordDto>>> GetAll(
            [FromQuery] QueryParams query,
            CancellationToken ct)
        {
            var result = await medicalRecordService.GetAllAsync(query, ct);

            return Ok(result);
        }

        /// <summary>
        /// Gets a medical record by ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MedicalRecordDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MedicalRecordDto>> GetById(
            int id,
            CancellationToken ct)
        {
            var record = await medicalRecordService.GetByIdAsync(id, ct);

            if (record is null)
                return NotFound();

            return Ok(record);
        }

        /// <summary>
        /// Creates a new medical record.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(MedicalRecordDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MedicalRecordDto>> Create(
            [FromBody] CreateMedicalRecordRequest dto,
            CancellationToken ct)
        {
            var result = await medicalRecordService.CreateAsync(dto, ct);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Value!.Id },
                result.Value);
        }

        /// <summary>
        /// Updates an existing medical record.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateMedicalRecordDto dto,
            CancellationToken ct)
        {
            var existing = await medicalRecordService.GetByIdAsync(id, ct);

            if (existing is null)
                return NotFound();

            await medicalRecordService.UpdateAsync(id, dto, ct);

            return NoContent();
        }

        /// <summary>
        /// Deletes a medical record.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken ct)
        {
            var existing = await medicalRecordService.GetByIdAsync(id, ct);

            if (existing is null)
                return NotFound();

            await medicalRecordService.DeleteAsync(id, ct);

            return NoContent();
        }
    }
}