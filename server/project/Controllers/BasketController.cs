using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project.BLL.Interfaces;
using project.Models;
using System.Data;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketItemBLL _basketItemBLL;
        private readonly ILogger<BasketController> _logger;

        public BasketController(IBasketItemBLL basketItemBLL, ILogger<BasketController> logger)
        {
            _basketItemBLL = basketItemBLL;
            _logger = logger;
        }

        // GET api/<BasketController>/5
        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            _logger.LogInformation("Get basket called for userId: {UserId}", userId);

            try
            {
                var userIdFromToken = int.Parse(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!
                );

                if (userIdFromToken != userId)
                {
                    _logger.LogWarning("Unauthorized basket access attempt. TokenUserId: {TokenId}, RequestedUserId: {RequestedId}",
                        userIdFromToken, userId);
                    return Forbid();
                }

                var items = await _basketItemBLL.Get(userId);

                _logger.LogInformation("Returned {Count} basket items for user {UserId}",
                    items.Count(), userId);

                return Ok(items);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while getting basket for user {UserId}", userId);
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<BasketController>
        [Authorize]
        [HttpPost("{userId}/{giftId}")]
        public async Task<IActionResult> Post(int userId, int giftId)
        {
            _logger.LogInformation("AddToBasket called. UserId: {UserId}, GiftId: {GiftId}", userId, giftId);

            try
            {
                var userIdFromToken = int.Parse(
                   User.FindFirstValue(ClaimTypes.NameIdentifier)!
               );

                if (userIdFromToken != userId)
                {
                    _logger.LogWarning("Unauthorized add-to-basket attempt. TokenUserId: {TokenId}, RequestedUserId: {RequestedId}",
                        userIdFromToken, userId);
                    return Forbid();
                }

                await _basketItemBLL.AddToBasket(userId, giftId);

                _logger.LogInformation("Gift {GiftId} added to basket for user {UserId}", giftId, userId);

                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Gift not found while adding to basket. UserId: {UserId}, GiftId: {GiftId}",
                    userId, giftId);
                return NotFound(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while adding to basket. UserId: {UserId}, GiftId: {GiftId}",
                    userId, giftId);
                return StatusCode(500, ex.Message);
            }

        }

        // PUT api/<BasketController>/5
        [Authorize]
        [HttpPut("{userId}/{giftId}/{myAction}")]
        public async Task<IActionResult> Put(int userId, int giftId, string myAction)
        {
            _logger.LogInformation("UpdateQuantity called. UserId: {UserId}, GiftId: {GiftId}, Action: {Action}",
                userId, giftId, myAction);

            try
            {
                var userIdFromToken = int.Parse(
                   User.FindFirstValue(ClaimTypes.NameIdentifier)!
               );

                if (userIdFromToken != userId)
                {
                    _logger.LogWarning("Unauthorized quantity update attempt. TokenUserId: {TokenId}, RequestedUserId: {RequestedId}",
                        userIdFromToken, userId);
                    return Forbid();
                }

                await _basketItemBLL.UpdateQuantity(userId, giftId, myAction);

                _logger.LogInformation("Quantity updated successfully. UserId: {UserId}, GiftId: {GiftId}, Action: {Action}",
                    userId, giftId, myAction);

                return Ok();
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while updating quantity. UserId: {UserId}, GiftId: {GiftId}",
                    userId, giftId);
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Invalid action while updating quantity. UserId: {UserId}, GiftId: {GiftId}, Action: {Action}",
                    userId, giftId, myAction);
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<BasketController>/5
        [Authorize]
        [HttpDelete("{userId}/{giftId}")]
        public async Task<IActionResult> Delete(int userId, int giftId)
        {
            _logger.LogInformation("Delete basket item called. UserId: {UserId}, GiftId: {GiftId}",
                userId, giftId);

            try
            {
                var userIdFromToken = int.Parse(
                   User.FindFirstValue(ClaimTypes.NameIdentifier)!
               );

                if (userIdFromToken != userId)
                {
                    _logger.LogWarning("Unauthorized delete item attempt. TokenUserId: {TokenId}, RequestedUserId: {RequestedId}",
                        userIdFromToken, userId);
                    return Forbid();
                }

                await _basketItemBLL.DeleteItem(userId, giftId);

                _logger.LogInformation("Basket item deleted successfully. UserId: {UserId}, GiftId: {GiftId}",
                    userId, giftId);

                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Basket item not found for deletion. UserId: {UserId}, GiftId: {GiftId}",
                    userId, giftId);
                return NotFound(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while deleting basket item. UserId: {UserId}, GiftId: {GiftId}",
                    userId, giftId);
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<BasketController>/5
        [Authorize]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(int userId)
        {
            _logger.LogInformation("ClearBasket called for UserId: {UserId}", userId);

            try
            {
                var userIdFromToken = int.Parse(
                   User.FindFirstValue(ClaimTypes.NameIdentifier)!
               );

                if (userIdFromToken != userId)
                {
                    _logger.LogWarning("Unauthorized clear basket attempt. TokenUserId: {TokenId}, RequestedUserId: {RequestedId}",
                        userIdFromToken, userId);
                    return Forbid();
                }

                await _basketItemBLL.ClearBasket(userId);

                _logger.LogInformation("Basket cleared successfully for UserId: {UserId}", userId);

                return Ok();
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while clearing basket for UserId: {UserId}", userId);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
