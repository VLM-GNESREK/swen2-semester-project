using TourPlanner.BL.DTOs;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.BL.Services;

public class ContactService : IContactService
{
    private static Dictionary<int, ContactDto> _contacts = new Dictionary<int, ContactDto>();
    private static int _contactId = 0;
    public List<ContactDto> GetAllContacts()
    {
        return _contacts.Values.ToList();
    }

    public bool AddContact(ContactDto contact)
    {
        contact.Id = _contactId++;
        _contacts.Add(contact.Id, contact);
        return true;
    }

    public bool RemoveContact(int id)
    {
        return _contacts.Remove(id);
    }

    public bool UpdateContact(int id, ContactDto contact)
    {
        if (_contacts.ContainsKey(id))
        {
            contact.Id = id;
            _contacts[id] = contact;
            return true;
        }
        return false;
    }
}