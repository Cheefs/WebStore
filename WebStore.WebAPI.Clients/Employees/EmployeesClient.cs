using System.Net;
using System.Net.Http.Json;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;
using WebStore.WebAPI.Clients.Base;

namespace WebStore.WebAPI.Clients.Employees;

public class EmployeesClient : BaseClient, IEmployeesData
{
    public EmployeesClient(HttpClient client) : base(client, "api/employees") { }

    public int GetCount() => Get<int>($"{Address}/count");

    public IEnumerable<Employee> Get(int skip, int take) => Get<IEnumerable<Employee>>($"{Address}/{skip}/{take}") ?? Enumerable.Empty<Employee>();

    public Employee? GetById(int id) => Get<Employee>($"{Address}/{id}");

    public IEnumerable<Employee> GetAll() => Get<IEnumerable<Employee>>(Address) ?? Enumerable.Empty<Employee>();

    public int Add(Employee employee)
    {
        var response = Post(Address, employee);
        var newEmploye = response.Content.ReadFromJsonAsync<Employee>().Result;

        if(newEmploye is null)
        {
            throw new InvalidOperationException("Не удалось добавить сотрудника");
        }
        var id = newEmploye.Id;
        employee.Id = id;

        return id;
    }

    public bool Edit(Employee employee)
    {
        var response = Put(Address, employee);
        return response.StatusCode == HttpStatusCode.OK; 
    }

    public bool Delete(int id)
    {
        var response = Delete($"{Address}/{id}");
        return response.IsSuccessStatusCode;
    }
}
