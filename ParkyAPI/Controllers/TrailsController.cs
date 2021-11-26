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
    /// Trail controller
    /// </summary>
    [Route("api/v{version:apiVersion}/Trails")]
    //[Route("api/Trails")]
    //[ApiExplorerSettings(GroupName = "ParkyOpenApiSpecTrails")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailsController : ControllerBase
    {
        private readonly ITrailRepository _trailRepository;
        private readonly IMapper _mapper;

        public TrailsController(ITrailRepository trailRepo, IMapper mapper)
        {
            _trailRepository = trailRepo;
            _mapper = mapper;
        }

        /// <summary>
        ///  Endpoint to get list of all the Trails
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TrailDto>))]
        public IActionResult GetTrails()
        {
            var trailsList = _trailRepository.GetTrails();
            var trailsDto = new List<TrailDto>();
            foreach (var trail in trailsList)
            {
                trailsDto.Add(_mapper.Map<TrailDto>(trail));
            }
            return Ok(trailsDto);
        }

        /// <summary>
        ///  Endpoint to get individual Trail
        /// </summary>
        /// <param name="trailId">Id of the individual Trail </param>
        /// <returns></returns>
        [HttpGet("{trailId:int}", Name = "GetTrail")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = "Admin")]
        public IActionResult GetTrail(int trailId)
        {
            var trail = _trailRepository.GetTrail(trailId);

            if(trail == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<TrailDto>(trail));
        }

        /// <summary>
        ///  Endpoint to get individual Trail
        /// </summary>
        /// <param name="nationalParkId">Id of the nationa park </param>
        /// <returns></returns>
        [HttpGet("[action]/{nationalParkId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrailsInNationPark(int nationalParkId)
        {
            var trails = _trailRepository.GetTrailsInNationPark(nationalParkId);

            if (trails == null)
            {
                return NotFound();
            }

            var trailDtos = new List<TrailDto>();
            foreach(var trail in trails)
            {
                trailDtos.Add(_mapper.Map<TrailDto>(trail));
            }

            return Ok(trailDtos);
        }

        /// <summary>
        ///     Endpoint to create Trail
        /// </summary>
        /// <param name="trailDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateTrail([FromBody] TrailUpsertDto trailDto)
        { 
            if(trailDto == null)
            {
                return BadRequest(ModelState);
            }

            if(_trailRepository.TrailExists(trailDto.Name))
            {
                ModelState.AddModelError("", $"Trail having name {trailDto.Name} is already exists!");
                return StatusCode(404, ModelState);
            }

            var trail = _mapper.Map<Trail>(trailDto);

            if(!_trailRepository.CreateTrail(trail))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {trailDto.Name}.");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetTrail", new {trailId = trail.Id}, trail);
        }

        [HttpPatch("{trailId:int}", Name = "UpdateTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateTrail(int trailId, [FromBody] TrailUpsertDto trailDto)
        {
            if(trailDto == null)
            {
                return BadRequest();
            }

            if (!_trailRepository.TrailExists(trailId))
            {
                ModelState.AddModelError("", $"Trail with id {trailId} does not exists!");
                return StatusCode(404, ModelState);
            }

            var trail = _mapper.Map<Trail>(trailDto);
            trail.Id = trailId;

            if (!_trailRepository.UpdateTrail(trail))
            {
                ModelState.AddModelError("", $"Something went wrong while updating the record {trailDto.Name}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{trailId:int}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTrail(int trailId)
        {
            if (!_trailRepository.TrailExists(trailId))
            {
                ModelState.AddModelError("", $"Trail with id {trailId} does not exists!");
                return StatusCode(404, ModelState);
            }

            var trail = _trailRepository.GetTrail(trailId);

            if (!_trailRepository.DeleteTrail(trail))
            {
                ModelState.AddModelError("", $"Something went wrong while deleting the record with id {trailId}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
