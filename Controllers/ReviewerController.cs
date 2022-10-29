using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using AutoMapper;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController : Controller
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;
        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviews()
        {
            var reviewer = _mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviews());
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(reviewer);
        }
        [HttpGet("{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();
            
            var reviewer =  _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewer(reviewerId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(reviewer);
        }

        [HttpGet("{reviewerId}/reviews")]
        // [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        // [ProducesResponseType(400)]
        public IActionResult GetActionResult(int reviewerId)
        {
            if(!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();

            var reviews = _mapper.Map<List<ReviewDto>>(
                _reviewerRepository.GetReviewsByReviewer(reviewerId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(reviews);
        }

    }
}