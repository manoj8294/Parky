using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Model;
using ParkyAPI.Model.Dtos;
using ParkyAPI.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;

namespace ParkyAPI.Controllers
{
    /// <summary>
    /// Nation park controller
    /// </summary>
    [Route("api/v{version:apiVersion}/nationalparks")]
    [ApiVersion("2.0")]
    //[Route("api/[controller]")]
    //[ApiExplorerSettings(GroupName = "ParkyOpenApiSpecNP")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParksV2Controller : ControllerBase
    {
        private readonly INationalParkRepository _npRepository;
        private readonly IMapper _mapper;

        public NationalParksV2Controller(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepository = npRepo;
            _mapper = mapper;
        }

        /// <summary>
        ///  Endpoint to get list of all the National Parks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<NationalParkDto>))]
        public IActionResult GetNationalParks()
        {
            var parksList = _npRepository.GetNationalParks();
            var parksDto = new List<NationalParkDto>();
            foreach (var park in parksList)
            {
                parksDto.Add(_mapper.Map<NationalParkDto>(park));
            }
            return Ok(parksDto.FirstOrDefault());
        }
    }
}
