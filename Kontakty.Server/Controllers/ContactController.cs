using Kontakty.Data;
using Kontakty.DTOs;
using Kontakty.Helpers;
using Kontakty.Interfaces;
using Kontakty.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace Kontakty.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ILogger<ContactController> _logger;
    private readonly ApplicationDBContext _context;
    private readonly IContactRepository _contactRepository;

    public ContactController(ILogger<ContactController> logger, ApplicationDBContext context,
        IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
        _logger = logger;
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }


    [HttpGet("search")]
    public async Task<IActionResult> GetAllWithQuery([FromQuery] QueryObject query)
    {
        var kontakty = await _contactRepository.GetAllAsync(query);
        var kontaktDto = kontakty.Select(s => s.ToContactListDto());
        return Ok(kontaktDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var kontakty = await _contactRepository.GetAllAsync();
        var kontaktDto = kontakty.Select(s => s.ToContactListDto());
        return Ok(kontaktDto);
    }


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
            return Conflict(new { message = "There already exists a contact with the same email address." });
        }

        var kontaktModel = contactCreateAndUpdateDto.ToContactFromCreateDto();
        await _contactRepository.CreateAsync(kontaktModel);

        return CreatedAtAction(nameof(GetById), new { id = kontaktModel.Id }, kontaktModel.ToContactDetailDto());
    }

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
        if (await _contactRepository.ExistsByEmail(contactCreateAndUpdateDto.Email))
        {
            return Conflict(new { message = "There already exists a contact with the same email address." });
        }
        return Ok(kontaktModel.ToContactDetailDto());
    }

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