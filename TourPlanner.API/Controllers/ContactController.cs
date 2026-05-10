using Microsoft.AspNetCore.Mvc;
using TourPlanner.BL.DTOs;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.API.Controllers;

[ApiController]
[Route("api/contacts")]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var contacts = _contactService.GetAllContacts();
        if (contacts.Count == 0)
        {
            return NoContent();
        }

        return Ok(contacts);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteContact(int id)
    {
        if (_contactService.RemoveContact(id))
        {
            return Ok();
        }

        return NotFound();
    }

    [HttpPost]
    public IActionResult AddContact(ContactDto contact)
    {
        if (_contactService.AddContact(contact))
        {
            return Ok();
        }

        return BadRequest();
    }

    [HttpPut("{id}")]
    public IActionResult UpdateContact(int id, ContactDto contact)
    {
        if (_contactService.UpdateContact(id, contact))
        {
            return Ok();
        }

        return NotFound();
    }
}