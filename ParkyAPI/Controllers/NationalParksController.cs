using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Model;
using ParkyAPI.Model.Dtos;
using ParkyAPI.Repository.IRepository;
using System.Collections.Generic;

namespace ParkyAPI.Controllers
{
    /// <summary>
    /// Nation park controller
    /// </summary>
    [Route("api/v{version:apiVersion}/nationalparks")]
    //[Route("api/[controller]")]
    //[ApiExplorerSettings(GroupName = "ParkyOpenApiSpecNP")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParksController : ControllerBase
    {
        private readonly INationalParkRepository _npRepository;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository npRepo, IMapper mapper)
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
            return Ok(parksDto);
        }

        /// <summary>
        ///  Endpoint to get individual National Park
        /// </summary>
        /// <param name="nationalParkId">Id of the individual National Park </param>
        /// <returns></returns>
        [HttpGet("{nationalParkId:int}", Name = "GetNationalPark")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var park = _npRepository.GetNationalPark(nationalParkId);

            if(park == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<NationalParkDto>(park));
        }

        /// <summary>
        ///     Endpoint to create National Park
        /// </summary>
        /// <param name="nationalParkDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto)
        { 
            if(nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }

            if(_npRepository.NationalParkExists(nationalParkDto.Name))
            {
                ModelState.AddModelError("", $"National park having name {nationalParkDto.Name} is alraedy exists!");
                return StatusCode(404, ModelState);
            }

            var nationalPark = _mapper.Map<NationalPark>(nationalParkDto);

            if(!_npRepository.CreateNationalPark(nationalPark))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {nationalParkDto.Name}.");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetNationalPark", new { Version = HttpContext.GetRequestedApiVersion().ToString(),
                                                nationalParkId = nationalPark.Id}, nationalPark);
        }

        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateNationalPark(int nationalParkId, [FromBody] NationalParkDto nationalParkDto)
        {
            if(nationalParkDto == null)
            {
                return BadRequest();
            }

            if (!_npRepository.NationalParkExists(nationalParkId))
            {
                ModelState.AddModelError("", $"National park with id {nationalParkId} does not exists!");
                return StatusCode(404, ModelState);
            }

            var nationalPark = _mapper.Map<NationalPark>(nationalParkDto);
            nationalPark.Id = nationalParkId;

            if (!_npRepository.UpdateNationalPark(nationalPark))
            {
                ModelState.AddModelError("", $"Something went wrong while updating the record {nationalParkDto.Name}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{nationalParkId:int}", Name = "DeleteNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteNationalPark(int nationalParkId)
        {
            if (!_npRepository.NationalParkExists(nationalParkId))
            {
                ModelState.AddModelError("", $"National park with id {nationalParkId} does not exists!");
                return StatusCode(404, ModelState);
            }

            var nationalPark = _npRepository.GetNationalPark(nationalParkId);

            if (!_npRepository.DeleteNationalPark(nationalPark))
            {
                ModelState.AddModelError("", $"Something went wrong while deleting the record with id {nationalParkId}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
