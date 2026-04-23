using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project.BLL.Interfaces;
using project.Models;
using project.Models.DTO;
using System.Data;
using Microsoft.Extensions.Logging;

namespace project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RaffleController : ControllerBase
    {
        private readonly IRaffleBLL _raffleBLL;
        private readonly IMapper _mapper;
        private readonly ILogger<RaffleController> _logger;

        public RaffleController(IRaffleBLL raffleBLL, IMapper mapper, ILogger<RaffleController> logger)
        {
            _raffleBLL = raffleBLL;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("draw")]
        public async Task<IActionResult> Draw([FromBody] RaffleDTO raffleDTO)
        {
            _logger.LogWarning("Admin initiated raffle draw for GiftId: {GiftId}", raffleDTO.GiftId);

            try
            {
                var raffle = _mapper.Map<Raffle>(raffleDTO);

                await _raffleBLL.AddRaffle(raffle);

                _logger.LogInformation("Raffle completed successfully for GiftId: {GiftId}", raffleDTO.GiftId);

                return Ok("Raffle completed successfully");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument during raffle draw. GiftId: {GiftId}", raffleDTO.GiftId);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation during raffle draw. GiftId: {GiftId}", raffleDTO.GiftId);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Entity not found during raffle draw. GiftId: {GiftId}", raffleDTO.GiftId);
                return NotFound(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error during raffle draw. GiftId: {GiftId}", raffleDTO.GiftId);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
