 using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Dto;
using AutoMapper;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller 
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
         
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        public IActionResult GetCategories()
        {
            var category = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(category);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();
            
            var category =  _mapper.Map<CountryDto>(_categoryRepository.GetCategory(categoryId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(category);
        }

        [HttpGet("pokemon/{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByCategoryId(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var category =  _mapper.Map<List<PokemonDto>>(
                _categoryRepository.GetPokemonByCategory(categoryId));
            
            if(!ModelState.IsValid)
                return BadRequest();
            
            return Ok(category);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
        {
            if(categoryCreate==null)
                return BadRequest(ModelState);

            var category = _categoryRepository.GetCategories()
                .Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();
            
            if(category != null)
            {
                ModelState.AddModelError("", "Category already Exists");
                return StatusCode(442, ModelState);
            }

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var categoryMap = _mapper.Map<Category>(categoryCreate);

            if(!_categoryRepository.CreateCategory(categoryMap))
            {
                ModelState.AddModelError("","something went wrong while saving");
                StatusCode(500, ModelState);
            }
            return Ok("Successfully created!!!");
        }
        [HttpPut("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int categoryId, [FromBody]CategoryDto updateCategory)
        {
            if(updateCategory == null)
                return BadRequest(ModelState);

            if(categoryId != updateCategory.Id)
                return BadRequest(ModelState);
            
            if(!_categoryRepository.CategoryExists(categoryId))
                return NotFound();
                
            if (!ModelState.IsValid)
                return BadRequest();

            var categoryMap = _mapper.Map<Category>(updateCategory);

            if(!_categoryRepository.UpdateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}