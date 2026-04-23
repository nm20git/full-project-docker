using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project.BLL;
using project.BLL.Interfaces;
using project.DAL;
using project.Models;
using project.Models.DTO;
using System;
using System.Data;
using Microsoft.Extensions.Logging;
using project.DAL.Intefaces;

namespace project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly IGiftBLL _giftBLL;
        private readonly IMapper _mapper;
        private readonly ILogger<GiftController> _logger;

        public GiftController(IGiftBLL giftBLL, IMapper mapper, ILogger<GiftController> logger)
        {
            _giftBLL = giftBLL;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/<GiftController>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var gifts = await _giftBLL.Get();
            var dto = _mapper.Map<List<GiftDTO>>(gifts);
            return Ok(dto);
        }


        // GET api/<GiftController>/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("Get gift by id called. GiftId: {GiftId}", id);

            try
            {
                var gift = await _giftBLL.Get(id);

                if (gift == null)
                    return NotFound("Gift not found");

                var dto = _mapper.Map<GiftDTO>(gift);
                return Ok(dto);

            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while getting gift {GiftId}", id);
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{id}/buyers")]
        public async Task<ActionResult<List<User>>> GetBuyers(int id)
        {
            _logger.LogInformation("Get buyers called for GiftId: {GiftId}", id);

            try
            {
                var buyers = await _giftBLL.GetBuyers(id);
                _logger.LogInformation("Returned {Count} buyers for GiftId {GiftId}", buyers.Count(), id);
                return Ok(buyers);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Gift not found while getting buyers. GiftId: {GiftId}", id);
                return NotFound(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while getting buyers for GiftId {GiftId}", id);
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{giftId}/winners")]
        public async Task<List<User>> GetWinners(int giftId)
        { 
            _logger.LogInformation("BLL: Getting winners for GiftId: {GiftId}", giftId);

            var winners = await  _giftBLL.GetWinners(giftId); 

            _logger.LogInformation("BLL: Retrieved {Count} winners for GiftId: {GiftId}", winners.Count, giftId);
           
            return winners; 
        }
        [Authorize]
        [HttpGet("all-winners")]
        public async Task<ActionResult<List<User>>> GetWinners()
        {
            _logger.LogInformation("Admin requested winners for Gifts");

            try
            {
                var winners = await _giftBLL.GetAllWinners();
                _logger.LogInformation("Returned {Count} winners for Gifts", winners.Count());
                return Ok(winners);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Database error while getting winners for Gifts");
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while getting winners");
                return BadRequest(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while getting winners for Gifts");
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] GiftCreateDTO dto)
        {
            _logger.LogInformation("Admin creating new gift. GiftName: {GiftName}", dto.Name);

            try
            {
                if (dto == null)
                    return BadRequest("DTO NULL");

                if (dto.Image == null)
                    return BadRequest("Image missing");
                //שמירת התמונה בשרת
                string? imagePath = null;

                if (dto.Image != null)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    var fullPath = Path.Combine(folder, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await dto.Image.CopyToAsync(stream);

                    imagePath = "/uploads/" + fileName;
                }
                var gift = _mapper.Map<Gift>(dto);

                if (imagePath != null)
                    gift.ImageUrl = imagePath;
                await _giftBLL.Add(gift);

                _logger.LogInformation("Gift created successfully. GiftName: {GiftName}", dto.Name);

                return Ok();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data while creating gift. GiftName: {GiftName}", dto.Name);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Related entity not found while creating gift. GiftName: {GiftName}", dto.Name);
                return NotFound(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while creating gift. GiftName: {GiftName}", dto.Name);
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] GiftCreateDTO dto)
        {
            _logger.LogInformation("Admin updating gift. GiftId: {GiftId}", id);

            try
            {
                //שמירת התמונה בשרת
                string? imagePath = null;

                if (dto.Image != null)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    var fullPath = Path.Combine(folder, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await dto.Image.CopyToAsync(stream);

                    imagePath = "/uploads/" + fileName;
                }
                Console.WriteLine($"DTO received: {dto.Name} {dto.Price} {dto.SponsorId}");
                var gift = await _giftBLL.Get(id);
                _mapper.Map(dto, gift);
                //var gift = _mapper.Map<Gift>(dto);

                if (imagePath != null)
                    gift.ImageUrl = imagePath;

                gift.Id = id;
                await _giftBLL.Update(gift);

                _logger.LogInformation("Gift updated successfully. GiftId: {GiftId}", id);

                return Ok();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data while updating gift. GiftId: {GiftId}", id);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while updating gift. GiftId: {GiftId}", id);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Gift not found while updating. GiftId: {GiftId}", id);
                return NotFound(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while updating gift. GiftId: {GiftId}", id);
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Admin deleting gift. GiftId: {GiftId}", id);

            try
            {
                await _giftBLL.Delete(id);

                _logger.LogInformation("Gift deleted successfully. GiftId: {GiftId}", id);

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while deleting gift. GiftId: {GiftId}", id);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Gift not found while deleting. GiftId: {GiftId}", id);
                return NotFound(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while deleting gift. GiftId: {GiftId}", id);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("filter")]
        public async Task<ActionResult<List<Gift>>> Search([FromBody] GiftFilterDTO filter)
        {
            _logger.LogInformation("Gift filter called");

            try
            {
                var gifts = await _giftBLL.FilterGifts(filter);
                var giftsDTO = _mapper.Map<List<GiftDTO>>(gifts);

                _logger.LogInformation("Filter returned {Count} gifts", giftsDTO.Count);

                return Ok(giftsDTO);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid filter parameters");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Filter failed - entity not found");
                return NotFound(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while filtering gifts");
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("reset-system")]
        public async Task<IActionResult> ResetSystem()
        {
            _logger.LogWarning("Admin triggered system reset");

            try
            {
                await _giftBLL.ResetSystem();

                _logger.LogInformation("System reset successfully");

                return Ok("System reset successfully");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation during system reset");
                return BadRequest(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error during system reset");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("sorted/by-buyers")]
        public async Task<IActionResult> GetSortedByBuyers()
        {
            try
            {

                var gifts = await _giftBLL.GetSortedByBuyers();
                var dto = _mapper.Map<List<GiftDTO>>(gifts);
                return Ok(dto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DataException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("sorted/by-price")]
        public async Task<IActionResult> GetSortedByPrice()
        {
            try
            {
                var gifts = await _giftBLL.GetSortedByPrice();
                var dto = _mapper.Map<List<GiftDTO>>(gifts);
                return Ok(dto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DataException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
