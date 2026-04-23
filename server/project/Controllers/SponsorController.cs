using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project.BLL;
using project.BLL.Interface;
using project.DAL;
using project.Models;
using project.Models.DTO;
using System.Data;
using Microsoft.Extensions.Logging;

namespace project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class SponsorController : ControllerBase
    {
        private readonly ISponsorBLL _sponsorBLL;
        private readonly IMapper _mapper;
        private readonly ILogger<SponsorController> _logger;

        public SponsorController(ISponsorBLL sponsorBLL, IMapper mapper, ILogger<SponsorController> logger)
        {
            _sponsorBLL = sponsorBLL;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/<SponsorController>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Admin requested all sponsors");

            try
            {
                var sponsors = await _sponsorBLL.Get();
                _logger.LogInformation("Returned {Count} sponsors", sponsors.Count());
                return Ok(sponsors);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while getting all sponsors");
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<SponsorController>/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("Admin requested sponsor by id. SponsorId: {SponsorId}", id);

            try
            {
                var sponsor = await _sponsorBLL.Get(id);
                return Ok(sponsor);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Sponsor not found. SponsorId: {SponsorId}", id);
                return NotFound(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while getting sponsor {SponsorId}", id);
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/gifts")]
        public async Task<ActionResult<List<Gift>>> GetGifts(int id)
        {
            _logger.LogInformation("Admin requested gifts for SponsorId: {SponsorId}", id);

            try
            {
                var gifts = await _sponsorBLL.GetGifts(id);
                _logger.LogInformation("Returned {Count} gifts for SponsorId {SponsorId}", gifts.Count(), id);
                return Ok(gifts);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Sponsor not found while getting gifts. SponsorId: {SponsorId}", id);
                return NotFound(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while getting gifts for SponsorId {SponsorId}", id);
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<SponsorController>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SponsorDTO sponsorDTO)
        {
            _logger.LogInformation("Admin creating sponsor. SponsorName: {SponsorName}", sponsorDTO.FirstName);

            try
            {
                var sponsor = _mapper.Map<Sponsor>(sponsorDTO);
                await _sponsorBLL.Add(sponsor);

                _logger.LogInformation("Sponsor created successfully. SponsorName: {SponsorName}", sponsorDTO.FirstName);

                return Ok();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data while creating sponsor. SponsorName: {SponsorName}", sponsorDTO.FirstName);
                return BadRequest(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while creating sponsor. SponsorName: {SponsorName}", sponsorDTO.FirstName);
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/<SponsorController>/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SponsorDTO sponsorDTO)
        {
            _logger.LogInformation("Admin updating sponsor. SponsorId: {SponsorId}", id);

            try
            {
                var sponsor = _mapper.Map<Sponsor>(sponsorDTO);
                sponsor.Id = id;
                await _sponsorBLL.Update(sponsor);

                _logger.LogInformation("Sponsor updated successfully. SponsorId: {SponsorId}", id);

                return Ok();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data while updating sponsor. SponsorId: {SponsorId}", id);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Sponsor not found while updating. SponsorId: {SponsorId}", id);
                return NotFound(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while updating sponsor. SponsorId: {SponsorId}", id);
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<SponsorController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Admin deleting sponsor. SponsorId: {SponsorId}", id);

            try
            {
                await _sponsorBLL.Delete(id);

                _logger.LogInformation("Sponsor deleted successfully. SponsorId: {SponsorId}", id);

                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Sponsor not found while deleting. SponsorId: {SponsorId}", id);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while deleting sponsor. SponsorId: {SponsorId}", id);
                return BadRequest(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while deleting sponsor. SponsorId: {SponsorId}", id);
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("filter")]
        public async Task<ActionResult<List<Sponsor>>> FilterSponsors([FromBody] SponsorFilterDTO filter)
        {
            _logger.LogInformation("Admin filtering sponsors");

            try
            {
                var sponsors = await _sponsorBLL.FilterSponsors(filter);
                var sponsorsDTO = _mapper.Map<List<SponsorDTO>>(sponsors);

                _logger.LogInformation("Filter returned {Count} sponsors", sponsorsDTO.Count);

                return Ok(sponsorsDTO);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid filter parameters for sponsors");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Filter failed - entity not found");
                return NotFound(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while filtering sponsors");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
