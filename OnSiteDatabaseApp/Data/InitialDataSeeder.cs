using OnSiteApp.Models;
using OnSiteApp.Services;

namespace OnSiteApp.Data;


// adds preset data to table 
public static class InitialDataSeeder
{
	
	public static void AddInitialData(OnSiteDatabase database)
	{
		AddFirstAdmin(database);
		AddSampleData(database);
	}

	
	private static void AddFirstAdmin(OnSiteDatabase database)
	{
		if (database.Admins.Any())
		{
			return;
		}

		database.Admins.Add(new AdminUser
		{
			Name = "Carlos Tapia",
			PasswordHash = PasswordHelper.Hash("12345")
		});
		database.SaveChanges();
	}

	
	private static void AddSampleData(OnSiteDatabase database)
	{
		if (database.Customers.Any())
		{
			return;
		}

		var customers = new[]
		{
			new Customer { Name = "Alice Smith", Phone = "864-555-0101", Address = "347 Timberwood drive, Woodruff" },
			new Customer { Name = "Bob Jones", Phone = "864-555-0102", Address = "349 Timberwood drive, Woodruff" },
			new Customer { Name = "Carla Diaz", Phone = "803-555-0103", Address = "1536 Old Anderson Mill, Moore" }
		};

		database.Customers.AddRange(customers);
		database.SaveChanges();

		database.Jobs.AddRange(
			new Job
			{
				CustomerId = customers[0].Id,
				ClientName = customers[0].Name,
				Service = "Full Detail",
				Address = customers[0].Address,
				ScheduledTime = DateTime.Now.AddHours(-1),
				Checklist = new List<ChecklistItem>
				{
					new() { TaskLabel = "Exterior Wash & Wax", BillableValue = 100 },
					new() { TaskLabel = "Interior Vacuum & Wipe", BillableValue = 50 },
					new() { TaskLabel = "Headlight Clean (Add)", BillableValue = 25 }
				}
			},
			new Job
			{
				CustomerId = customers[1].Id,
				ClientName = customers[1].Name,
				Service = "Fix Leaky Faucet",
				Address = customers[1].Address,
				ScheduledTime = DateTime.Now.AddHours(2),
				Checklist = new List<ChecklistItem>
				{
					new() { TaskLabel = "Replace washer", BillableValue = 40 },
					new() { TaskLabel = "Test for leaks", BillableValue = 20 }
				}
			},
			new Job
			{
				CustomerId = customers[2].Id,
				ClientName = customers[2].Name,
				Service = "Dog Grooming",
				Address = customers[2].Address,
				ScheduledTime = DateTime.Now.AddHours(4),
				Checklist = new List<ChecklistItem>
				{
					new() { TaskLabel = "Bath", BillableValue = 45 },
					new() { TaskLabel = "Haircut", BillableValue = 35 },
					new() { TaskLabel = "Nail trim", BillableValue = 15 }
				}
			});

		database.SaveChanges();
	}
}