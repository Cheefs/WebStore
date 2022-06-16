using Microsoft.AspNetCore.Mvc;

namespace WebStore.WebAPI.Controllers
{
    [ApiController]
    [Route("api/values")]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private static Dictionary<int, string> _values = Enumerable.Range(1, 10)
            .Select(i => (id: i, value: $"Value-{i}"))
            .ToDictionary(v => v.id, v => v.value);

        public ValuesController(ILogger<ValuesController> logger) => _logger = logger;

        [HttpGet]
        public IActionResult GetAll()
        {
            if(_values.Count == 0)
            {
                return NoContent();
            }
                
            return Ok(_values.Values);
        }

        [HttpGet("{Id:int}")]
        public IActionResult GetById(int Id)
        {
            if(_values.TryGetValue(Id, out var value))
            {
                return Ok(value);
            }
            return NotFound(new { Id });
        }
        [HttpPost]
        [HttpPost("{Value}")]
        public IActionResult Add([FromBody] string Value)
        {
            var id = _values.LastOrDefault().Key + 1;
            _values[id] = Value;
            _logger.LogInformation($"Значение {Value} добавлено под Id {id}");

            return CreatedAtAction(nameof(GetById), new { Id = id }, Value);
        }

        [HttpPut("{Id:int}")]
        public IActionResult Edit(int Id, [FromBody] string Value)
        {
            if(!_values.ContainsKey(Id))
            {
                _logger.LogWarning($"При попытке редактирования записи с Id: {Id} - запись не найденна");
                return NotFound(new { Id });
            }

            var oldValue = _values[Id];
            _values[Id] = Value;

            _logger.LogInformation($"Редактирвоание записи id:{Id} (старое значение {oldValue} - новое значение {Value})");

            return Ok(new { Id, OldValue = oldValue, NewValue = Value });
        }

        [HttpDelete("{Id:int}")]
        public IActionResult Delete(int Id)
        {
            if (!_values.ContainsKey(Id))
            {
                _logger.LogWarning($"При попытки записи с Id: {Id} - запись не найденна");
                return NotFound(new { Id });
            }

            var value = _values[Id];
            _values.Remove(Id);

            _logger.LogInformation($"Удаление записи id:{Id} (значение {value})");

            return Ok(new { Id, Value = value });
        }
    }
}
