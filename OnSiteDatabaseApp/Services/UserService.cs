using OnSiteApp.Data;
using OnSiteApp.Models;

namespace OnSiteApp.Services;

public class UserService : IUserService
{
    private readonly OnSiteDatabase _context;

   
    public UserService(OnSiteDatabase context)
    {
        _context = context;
    }

   
    public List<Technician> GetTechnicians()
    {
        return _context.Technicians.OrderBy(t => t.TechNumber).ToList();
    }

   
    public Technician? GetTechnicianById(int id)
    {
        return _context.Technicians.Find(id);
    }

    // Find a technician by their assigned number.
    public Technician? FindTechnician(int techNumber)
    {
        return _context.Technicians.FirstOrDefault(t => t.TechNumber == techNumber);
    }

    // Save a new technician.
    public void AddTechnician(Technician technician)
    {
        _context.Technicians.Add(technician);
        _context.SaveChanges();
    }

    // Delete a technician if the ID exists.
    public void DeleteTechnician(int id)
    {
        var existing = _context.Technicians.Find(id);
        if (existing != null)
        {
            _context.Technicians.Remove(existing);
            _context.SaveChanges();
        }
    }

    // Get all administrator accounts.
    public List<AdminUser> GetAdmins()
    {
        return _context.Admins.ToList();
    }

    
    public AdminUser? FindAdmin(string name)
    {
        return _context.Admins.FirstOrDefault(a => a.Name.ToLower() == name.ToLower());
    }

    // Save a new administrator.
    public void AddAdmin(AdminUser admin)
    {
        _context.Admins.Add(admin);
        _context.SaveChanges();
    }

    // Find an administrator by database ID.
    public AdminUser? GetAdminById(int id)
    {
        return _context.Admins.Find(id);
    }

  
    public void SetTechnicianPassword(int id, string passwordHash)
    {
        var existing = _context.Technicians.Find(id);
        if (existing != null)
        {
            existing.PasswordHash = passwordHash;
            _context.SaveChanges();
        }
    }

    public void SetAdminPassword(int id, string passwordHash)
    {
        var existing = _context.Admins.Find(id);
        if (existing != null)
        {
            existing.PasswordHash = passwordHash;
            _context.SaveChanges();
        }
    }
}
