using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using test_project.Http.Requests;
using test_project.Models;

namespace test_project.Controllers;

[ApiController]
[Route("tags")]
public class TagController: ControllerBase
{
     protected TagModel _tagModel;

    public TagController(IConfiguration configuration)
    {
        _tagModel = new TagModel(configuration);
    }

    [HttpGet()]
    public IActionResult Get()
    {
        return Ok(_tagModel.getTopTags());
    }
    
    [HttpPost()]
    public IActionResult Store([FromBody] TagStoreRequest request)
    {
        Dictionary<string, dynamic> tag = new Dictionary<string, dynamic>();
        
        tag["name"] = request.Name;

        Dictionary<string, dynamic> newTodo = _tagModel.Store(tag);

        return Ok(newTodo);
    }

    [HttpPut()]
    public IActionResult Update([FromBody] TagUpdateRequest request)
    {
        Dictionary<string, dynamic> todo = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();

        todo["name"] = request.Name;
        condition["id"] = request.Id;

        _tagModel.Update(todo, condition);

        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        Dictionary<string, dynamic> conditions = new Dictionary<string, dynamic>();
        conditions["id"] = id;

        _tagModel.Delete(conditions);

        return Ok();
    }
}