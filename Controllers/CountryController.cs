using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;
using AutoMapper;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryController(ICountryRepository countryRepository,IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }
 
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        public IActionResult GetCountries()
        {
            var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(countries);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();
            
            var country =  _mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(country);
        }

        [HttpGet("/owners/{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(Country))]
        public IActionResult GetCountryOfAnOwner(int ownerId)
        { 
            
            var country =  _mapper.Map<CountryDto>(
                _countryRepository.GetCountryByOwner(ownerId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(country);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCountry([FromBody] CountryDto countryCreate)
        {
            if(countryCreate==null)
                return BadRequest(ModelState);

            var country = _countryRepository.GetCountries()
                .Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();
            
            if(country != null)
            {
                ModelState.AddModelError("", "Category already Exists");
                return StatusCode(442, ModelState);
            }

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var countryMap = _mapper.Map<Country>(countryCreate);

            if(!_countryRepository.CreateCountry(countryMap))
            {
                ModelState.AddModelError("","something went wrong while saving");
                StatusCode(500, ModelState);
            }
            return Ok("Successfully created!!!");
        }
        [HttpPut("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCountry(int countryId, [FromBody]CountryDto updateCountry)
        {
            if(updateCountry == null)
                return BadRequest(ModelState);

            if(countryId != updateCountry.Id)
                return BadRequest(ModelState);
            
            if(!_countryRepository.CountryExists(countryId))
                return NotFound();
                
            if (!ModelState.IsValid)
                return BadRequest();

            var countryMap = _mapper.Map<Country>(updateCountry);

            if(!_countryRepository.UpdateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}