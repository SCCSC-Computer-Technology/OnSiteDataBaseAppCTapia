using OnSiteApp.Models;

namespace OnSiteApp.Services;

public interface IJobService
{
    
    List<Job> GetAll();
    
    Job? GetById(int id);
    
    void SetStatus(int id, JobStatus status);
    
    void ToggleItem(int jobId, int itemId);
    
    void AddJob(Job job);
    
    List<Customer> GetCustomers();
   
    Customer? GetCustomerById(int id);
   
    void AddCustomer(Customer customer);
    
    void UpdateCustomer(Customer customer);
    
    bool DeleteCustomer(int id);
    
    void UpdateJob(Job job);
    
    void DeleteJob(int id);
    
    void UnassignTechnician(int technicianId);
}
