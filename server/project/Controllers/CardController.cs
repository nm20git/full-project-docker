using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project.BLL;
using project.BLL.Interfaces;
using project.Models;
using project.Models.DTO;
using System.Data;
using Microsoft.Extensions.Logging;

namespace project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ICardBLL _cardBLL;
        private readonly IMapper _mapper;
        private readonly ILogger<CardController> _logger;

        public CardController(ICardBLL cardBLL, IMapper mapper, ILogger<CardController> logger)
        {
            _cardBLL = cardBLL;
            _mapper = mapper;
            _logger = logger;
        }

        // POST api/<CardController>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post(CardDTO cardDTO)
        {
            _logger.LogInformation("Card purchase attempt. UserId: {UserId}, GiftId: {GiftId}, Quantity: {Quantity}",
                cardDTO.UserId, cardDTO.GiftId, cardDTO.Quantity);

            var quantity = cardDTO.Quantity;
            var card = _mapper.Map<Card>(cardDTO);

            try
            {
                await _cardBLL.Add(card, quantity);

                _logger.LogInformation("Card purchase successful. UserId: {UserId}, GiftId: {GiftId}, Quantity: {Quantity}",
                    cardDTO.UserId, cardDTO.GiftId, quantity);

                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex,
                    "Card purchase failed - entity not found. UserId: {UserId}, GiftId: {GiftId}",
                    cardDTO.UserId, cardDTO.GiftId);

                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex,
                    "Card purchase failed - invalid argument. UserId: {UserId}, GiftId: {GiftId}",
                    cardDTO.UserId, cardDTO.GiftId);

                return NotFound(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex,
                    "Database error during card purchase. UserId: {UserId}, GiftId: {GiftId}",
                    cardDTO.UserId, cardDTO.GiftId);

                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-total")]
        public async Task<IActionResult> GetTotal()
        {
            _logger.LogInformation("Admin requested total cards count");

            try
            {
                var total = await _cardBLL.GetTotal();

                _logger.LogInformation("Total cards returned successfully. Total: {Total}", total);

                return Ok(total);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while getting total cards count");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
