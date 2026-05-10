using TourPlanner.BL.DTOs;

namespace TourPlanner.BL.Interfaces;

public interface IContactService
{
    public List<ContactDto> GetAllContacts();
    public bool AddContact(ContactDto contact);
    public bool RemoveContact(int id);
    public bool UpdateContact(int id,ContactDto contact);
}