using OnSiteApp.Models;

namespace OnSiteApp.Services;

public interface IUserService
{
  
    List<Technician> GetTechnicians();
   
    Technician? GetTechnicianById(int id);
   
    Technician? FindTechnician(int techNumber);
   
    void AddTechnician(Technician technician);
   
    void DeleteTechnician(int id);
  
    List<AdminUser> GetAdmins();
   
    AdminUser? FindAdmin(string name);
   
    AdminUser? GetAdminById(int id);
   
    void AddAdmin(AdminUser admin);
   
    void SetTechnicianPassword(int id, string passwordHash);
    
    void SetAdminPassword(int id, string passwordHash);
}
