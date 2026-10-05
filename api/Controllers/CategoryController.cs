using Microsoft.AspNetCore.Http;

namespace api.Controllers;

public class CategoryController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICategoryService _cat;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CategoryController(
        UserManager<AppUser> userManager,
        ICategoryService cat,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _userManager = userManager;
        _cat = cat;
        _httpContextAccessor = httpContextAccessor;
    }

    [Authorize]
    [HttpGet("getAllowedCategories")]
    public async Task<PagedList<CategoryDto>?> AllowedCategories(
       [FromQuery] CategoryParams cp
   )
    {
        // get the allowed list from the server based on who is loggedin
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }
        var loggedinUser = await _userManager.FindByIdAsync(userId);
        if (loggedinUser == null)
        {
            return null;
        }
        if (User.IsInRole("Admin"))
        {
            var allCategories = await _cat.GetAllCategories(cp, new CancellationToken());
            if (allCategories == null)
            {
                return null;
            }
            var ti = allCategories.TotalCount;
            var tp = (int)Math.Ceiling(ti / (double)cp.PageSize);
            Response.AddPaginationHeader(
                new PaginationHeader(cp.PageNumber, cp.PageSize, ti, tp)
            );
            return allCategories;
        }
        else
        {
            if (loggedinUser.PhotoAllowedToSee == null || !loggedinUser.PhotoAllowedToSee.Any())
            {
                return null;
            }
            cp.AllowedCategories = loggedinUser.PhotoAllowedToSee.ToList();
            var allowedCategories = await _cat.GetAllowedCategories(cp, new CancellationToken());

            if (allowedCategories == null)
            {
                return null;
            }
            var totalItems = allowedCategories.TotalCount; // this is the totalnumber of categories for this person, because the allowed categories are only a subset of the total categories
            var totalPages = (int)Math.Ceiling(totalItems / (double)cp.PageSize);
            Response.AddPaginationHeader(
                new PaginationHeader(cp.PageNumber, cp.PageSize, totalItems, totalPages)
            );
            return allowedCategories;
        }

    }

    [HttpGet("getDescription/{category}")]
    public async Task<IActionResult> GetDescription(int category)
    {
        var result = await _cat.GetCategoryDescription(category, new CancellationToken());
        if (string.IsNullOrEmpty(result))
        {
            return NotFound("Category description not found");
        }
        return Ok(result);
    }

    [HttpGet("getSpecificCategory/{category}")]
    public async Task<IActionResult> GetSpecificCategory(int category)
    {
        var result = await _cat.GetSpecificCategory(category, new CancellationToken());
        if (result == null)
        {
            return NotFound("Category not found");
        }
        return Ok(result);
    }

    [HttpGet("getAllCategories")]
    public async Task<PagedList<CategoryDto>?> GetAllCategories([FromBody] CategoryParams cp, CancellationToken cancellationToken = default)
    {
        var allCategories = await _cat.GetAllCategories(cp, cancellationToken);
        if (allCategories == null)
        {
            return null;
        }
        var totalItems = allCategories.TotalCount; // this is the totalnumber of categories for this person, because the allowed categories are only a subset of the total categories
        var totalPages = (int)Math.Ceiling(totalItems / (double)cp.PageSize);
        Response.AddPaginationHeader(
            new PaginationHeader(cp.PageNumber, cp.PageSize, totalItems, totalPages)
        );
        return allCategories;

    }

    [HttpGet("getAllCategoriesForAdmin")]
    public async Task<CategoryDto[]>? GetAllCategoriesForAdmin(CancellationToken cancellationToken = default)
    {
        var allCategories = await _cat.GetAllCategoriesForAdmin(cancellationToken);
        if (allCategories == null)
        {
            return null;
        }

        return allCategories;
    }

    [HttpPut("updateCategory")]
    public async Task<IActionResult> UpdateCategory([FromBody] Category category, CancellationToken cancellationToken)
    {
        if (category == null)
        {
            return BadRequest("Request body is required.");
        }

        var result = await _cat.UpdateCategory(category, cancellationToken);
        if (string.IsNullOrEmpty(result))
        {
            return BadRequest("Failed to update category");
        }
        return Ok(result);
    }

    [HttpPost("addCategory")]
    public async Task<IActionResult> AddCategory([FromBody] Category category, CancellationToken cancellationToken)
    {
        if (category == null)
        {
            return BadRequest("Request body is required.");
        }

        var result = await _cat.CreateCategory(category, cancellationToken);
        if (result == null)
        {
            return BadRequest("Failed to create category");
        }
        return Ok(result); // this will return - Category was added
    }







}
