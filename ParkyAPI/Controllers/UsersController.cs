using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Infrastructure.Interface;
using ParkyAPI.Model;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILog _logger;

        public UsersController(IUserRepository userRepository, ILog logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticationModel model)
        {
            _logger.Debug("Enter Authentication");
            var user = _userRepository.Authenticate(model.Username, model.Password);

            if(user == null)
            {
                _logger.Debug("Username or Password is incorrect.");
                return BadRequest(new {Message = "Username or Password is incorrect."});
            }
            _logger.Debug("Exit Authentication");
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthenticationModel model)
        {
            bool isUniqueUser = _userRepository.IsUniqueUser(model.Username);
            if(!isUniqueUser)
            {
                return BadRequest(new { Message = "Username already exist" });
            }
            var user = _userRepository.Register(model.Username, model.Password);
            if(user == null)
            {
                return StatusCode(500, new { Message = "Error while registering the user" });
            }
            return Ok(user);
        }
    }
}
