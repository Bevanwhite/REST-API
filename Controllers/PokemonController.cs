using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using AutoMapper;
using PokemonReviewApp.Dto;


namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : Controller 
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public PokemonController(IPokemonRepository pokemonRepository, IOwnerRepository ownerRepository,IReviewRepository reviewRepository,ICategoryRepository categoryRepository,IMapper mapper)
        {
            _pokemonRepository = pokemonRepository;
            _ownerRepository = ownerRepository;
            _reviewRepository = reviewRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(pokemons);
        }

        [HttpGet("{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();
            
            var pokemon =  _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(pokemon);
        }

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if(!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            var rating = _pokemonRepository.GetPokemonRating(pokeId);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateOwner([FromQuery] int ownerId,[FromQuery] int catId,[FromBody] PokemonDto pokemonCreate)
        {
            if(pokemonCreate==null)
                return BadRequest(ModelState);

            var pokemons = _pokemonRepository.GetPokemons()
                .Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();
            
            if(pokemons != null)
            {
                ModelState.AddModelError("", "Owner already Exists");
                return StatusCode(442, ModelState);
            }

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

            if(!_pokemonRepository.CreatePokemon(ownerId, catId, pokemonMap))
            {
                ModelState.AddModelError("","something went wrong while saving");
                StatusCode(500, ModelState);
            }
            return Ok("Successfully created!!!");
        }
        [HttpPut("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon(int pokeId,
        [FromQuery] int ownerId, [FromQuery] int catId, 
        [FromBody]PokemonDto pokemonUpdate)
        {
            if(pokemonUpdate == null)
                return BadRequest(ModelState);

            if(pokeId != pokemonUpdate.Id)
                return BadRequest(ModelState);
            
            if(!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();
                
            if (!ModelState.IsValid)
                return BadRequest();

            var pokeMap = _mapper.Map<Pokemon>(pokemonUpdate);

            if(!_pokemonRepository.UpdatePokemon(ownerId, catId, pokeMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeletePokemon(int pokeId)
        {
            if(!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();
            
            var reviewToDelete = _reviewRepository.GetReviewsOfAPokemon(pokeId);
            var pokemonToDelete = _pokemonRepository.GetPokemon(pokeId);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if(_reviewRepository.DeleteReviews(reviewToDelete.ToList()))  
                ModelState.AddModelError("","Some thing went wrong deleting Reviews");

            if(_pokemonRepository.DeletePokemon(pokemonToDelete))  
                ModelState.AddModelError("","Some thing went wrong deleting Category");

            return NoContent();
        }
    }
}