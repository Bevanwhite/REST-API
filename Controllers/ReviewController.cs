using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using AutoMapper;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository reviewRepository, IReviewerRepository reviewerRepository, IPokemonRepository pokemonRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _reviewerRepository = reviewerRepository;
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviews()
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();
            
            var review =  _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(review);
        }

        [HttpGet("pokemon/{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsForAPokemon(int pokeId)
        {
            var review = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfAPokemon(pokeId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(review);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromQuery] int reviewerId,[FromQuery] int pokeId, [FromBody] ReviewDto reviewCreate)
        {
            if(reviewCreate == null)
                return BadRequest(ModelState);

            var reviews = _reviewRepository.GetReviews()
                .Where(c => c.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper())
                .FirstOrDefault();
            
            if(reviews != null)
            {
                ModelState.AddModelError("", "Owner already Exists");
                return StatusCode(442, ModelState);
            }

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var reviewMap = _mapper.Map<Review>(reviewCreate);
            reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokeId);
            reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId);

            if(!_reviewRepository.CreateReview(reviewMap))
            {
                ModelState.AddModelError("","something went wrong while saving");
                StatusCode(500, ModelState);
            }
            return Ok("Successfully created!!!");
        }

    }
}