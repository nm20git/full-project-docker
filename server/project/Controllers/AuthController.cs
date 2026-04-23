using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project.BLL.Interfaces;
using project.Models;
using project.Models.DTO;
using System.Data;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserBLL _userBLL;
        private readonly IMapper _mapper;
        private readonly ITokenBLL _tokenBLL;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserBLL userBLL, IMapper mapper, ITokenBLL tokenBLL, ILogger<AuthController> logger)
        {
            _userBLL = userBLL;
            _mapper = mapper;
            _tokenBLL = tokenBLL;
            _logger = logger;
        }

        // POST api/<AuthController>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            _logger.LogInformation("Register attempt for email: {Email}", userDTO.Email);

            try
            {
                var user = _mapper.Map<User>(userDTO);
                var result = await _userBLL.Register(user, userDTO.Password);

                if (!result.Success)
                {
                    _logger.LogWarning("Register failed for email: {Email}. Reason: {Reason}",
                        userDTO.Email, result.Message);

                    return BadRequest(new { message = result.Message });
                }

                _logger.LogInformation("User registered successfully: {Email}", userDTO.Email);

                return Ok(new { message = result.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error during register for {Email}", userDTO.Email);
                return BadRequest(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error during register for {Email}", userDTO.Email);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            _logger.LogInformation("Login attempt for email: {Email}", userLogin.Email);

            try
            {
                var user = await _userBLL.Login(userLogin.Email, userLogin.Password);

                if (user == null)
                {
                    _logger.LogWarning("Login failed for email: {Email}", userLogin.Email);
                    return Unauthorized("Email or password is incorrect");
                }

                var token = _tokenBLL.CreateToken(user);

                _logger.LogInformation("User logged in successfully: {UserId}", user.Id);

                return Ok(new
                {
                    token,
                    user = new
                    {
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        role = user.Role
                    }
                });
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error during login for {Email}", userLogin.Email);
                return StatusCode(500, ex.Message);
            }
        }


        [Authorize]
        [HttpGet("{id}/gifts")]
        public async Task<ActionResult<List<Gift>>> GetUserGifts(int id)
        {
            _logger.LogInformation("GetUserGifts called for userId: {UserId}", id);

            try
            {
                var userIdFromToken = int.Parse(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!
                );

                if (userIdFromToken != id)
                {
                    _logger.LogWarning(
                        "Unauthorized access attempt. TokenUserId: {TokenId}, RequestedId: {RequestedId}",
                        userIdFromToken, id);

                    return Forbid();
                }

                var gifts = await _userBLL.GetUserGifts(id);

                _logger.LogInformation("Returned {Count} gifts for user {UserId}",
                    gifts.Count, id);

                return Ok(gifts);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found: {UserId}", id);
                return NotFound(ex.Message);
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Database error while getting gifts for user {UserId}", id);
                return StatusCode(500, ex.Message);
            }
        }
    }
}