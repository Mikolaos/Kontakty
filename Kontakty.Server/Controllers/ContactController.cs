using Kontakty.Data;
using Kontakty.DTOs;
using Kontakty.Helpers;
using Kontakty.Interfaces;
using Kontakty.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace Kontakty.Controllers;

/// <summary>
/// Controller responsible for managing contact operations including creation, retrieval, update, and deletion
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ILogger<ContactController> _logger;
    private readonly ApplicationDBContext _context;
    private readonly IContactRepository _contactRepository;

    /// <summary>
    /// Initializes a new instance of the ContactController
    /// </summary>
    /// <param name="logger">Logger service for tracking application events</param>
    /// <param name="context">Database context for contact operations</param>
    /// <param name="contactRepository">Repository for managing contact-related operations</param>
    /// <exception cref="ArgumentNullException">Thrown when context is null</exception>
    public ContactController(ILogger<ContactController> logger, ApplicationDBContext context,
        IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
        _logger = logger;
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves contacts based on the provided search query parameters
    /// </summary>
    /// <param name="query">Query parameters for filtering contacts</param>
    /// <returns>200 OK with filtered list of contacts</returns>
    [HttpGet("search")]
    public async Task<IActionResult> GetAllWithQuery([FromQuery] QueryObject query)
    {
        var kontakty = await _contactRepository.GetAllAsync(query);
        var kontaktDto = kontakty.Select(s => s.ToContactListDto());
        return Ok(kontaktDto);
    }

    /// <summary>
    /// Retrieves all contacts from the database
    /// </summary>
    /// <returns>200 OK with list of all contacts</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var kontakty = await _contactRepository.GetAllAsync();
        var kontaktDto = kontakty.Select(s => s.ToContactListDto());
        return Ok(kontaktDto);
    }

    /// <summary>
    /// Retrieves a specific contact by their ID
    /// </summary>
    /// <param name="id">The ID of the contact to retrieve</param>
    /// <returns>
    /// 200 OK with contact details if found
    /// 404 Not Found if contact doesn't exist
    /// </returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var kontakt = await _contactRepository.GetByIdAsync(id);
        if (kontakt == null)
        {
            return NotFound();
        }
        return Ok(kontakt.ToContactDetailDto());
    }

    /// <summary>
    /// Creates a new contact
    /// </summary>
    /// <param name="contactCreateAndUpdateDto">The contact information to create</param>
    /// <returns>
    /// 201 Created with the newly created contact details
    /// 400 Bad Request if model validation fails
    /// 409 Conflict if email already exists
    /// </returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ContactCreateAndUpdateDto contactCreateAndUpdateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (await _contactRepository.ExistsByEmail(contactCreateAndUpdateDto.Email))
        {
            return Conflict(new { message = "Użytkownik z takim adresem email już istnieje." });
        }

        var kontaktModel = contactCreateAndUpdateDto.ToContactFromCreateDto();
        await _contactRepository.CreateAsync(kontaktModel);

        return CreatedAtAction(nameof(GetById), new { id = kontaktModel.Id }, kontaktModel.ToContactDetailDto());
    }

    /// <summary>
    /// Updates an existing contact
    /// </summary>
    /// <param name="id">The ID of the contact to update</param>
    /// <param name="contactCreateAndUpdateDto">The updated contact information</param>
    /// <returns>
    /// 200 OK with updated contact details
    /// 404 Not Found if contact doesn't exist
    /// </returns>
    [HttpPut]
    [Route("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update([FromRoute] int id,
        [FromBody] ContactCreateAndUpdateDto contactCreateAndUpdateDto)
    {
        var kontaktModel = await _contactRepository.UpdateAsync(id, contactCreateAndUpdateDto);
        if (kontaktModel == null)
        {
            return NotFound();
        }
        return Ok(kontaktModel.ToContactDetailDto());
    }

    /// <summary>
    /// Deletes a specific contact
    /// </summary>
    /// <param name="id">The ID of the contact to delete</param>
    /// <returns>
    /// 204 No Content if successfully deleted
    /// 404 Not Found if contact doesn't exist
    /// </returns>
    [HttpDelete]
    [Route("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var kontaktModel = await _contactRepository.DeleteAsync(id);
        if (kontaktModel == null)
        {
            return NotFound();
        }
        return NoContent();
    }
}