using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.Entities;
using WebStore.Interfaces;
using WebStore.Interfaces.Services;

namespace WebStore.WebAPI.Controllers;

/// <summary>Управление сотрудниками</summary>
[ApiController]
[Route(WebApiAdresses.V1.Employees)]
public class EmployeesApiController : ControllerBase
{
    private readonly IEmployeesData _employesData;
    private readonly ILogger<EmployeesApiController> _logger;

    public EmployeesApiController(IEmployeesData employesData, ILogger<EmployeesApiController> logger)
    {
        this._employesData = employesData;
        this._logger = logger;
    }

    /// <summary>Получить количество сотрудников</summary>
    [HttpGet("count")]
    public IActionResult GetCount() => Ok(_employesData.GetCount());

    /// <summary>Получить всех сотрудников</summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        if (_employesData.GetCount() == 0)
        {
            return NoContent();
        }
        return Ok(_employesData.GetAll());
    }

    /// <summary>
    /// Получить всех сотрудников по заданному диапазону
    /// </summary>
    /// <param name="skip">Сколько пропустить сотрудников</param>
    /// <param name="take">Сколько получить сотрудников</param>
    [HttpGet("{skip:int}/{take:int}")]
    public IActionResult Get(int skip, int take)
    {
        if (skip < 0 || take < 0)
        {
            return BadRequest();
        }

        if (take == 0 || skip > _employesData.GetCount())
        {
            return NoContent();
        }


        return Ok(_employesData.Get(skip, take));
    }

    /// <summary>
    /// Получить сотрудника по Идентификатору
    /// </summary>
    /// <param name="id">Индификатор</param>
    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var result = _employesData.GetById(id);
        if(result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Добавить сотрудника
    /// </summary>
    /// <param name="employee">Данные сотрудника</param>
    [HttpPost]
    public IActionResult Add([FromBody] Employee employee)
    {
        var id = _employesData.Add(employee);
        return CreatedAtAction(nameof(Get), new { id }, employee);
    }

    /// <summary>
    /// Редактирование сотрудника
    /// </summary>
    /// <param name="employee">Обновленные данные сотрудника</param>
    [HttpPut]
    public IActionResult Edit([FromBody] Employee employee)
    {
        var result = _employesData.Edit(employee);
        if (result)
        {
            return Ok(true);
        }

        return NotFound(false);
    }

    /// <summary>
    /// Удалить сотрудника
    /// </summary>
    /// <param name="id">Идентификатор</param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id) => _employesData.Delete(id) ? Ok() : NotFound();
}
