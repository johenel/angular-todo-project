using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using test_project.Http.Requests;
using test_project.Models;

namespace test_project.Controllers;

[ApiController]
[Route("todos")]

public class TodoController : ControllerBase
{
    protected TodoModel _todoModel;

    public TodoController(IConfiguration configuration)
    {
        _todoModel = new TodoModel(configuration);
    }

    [HttpGet()]
    public IActionResult Get(int? tagId)
    {
        List<Dictionary<string, dynamic>> todos;

        if (tagId != null) {
            todos = _todoModel.getPerTag(tagId);
        } else {
            todos = _todoModel.Get();
        }

        foreach (Dictionary<string, dynamic> todo in todos) {
            todo["tags"] = _todoModel.GetTodoTags(todo["id"]);
        }

        return Ok(todos);
    }

    public IActionResult GetPerTag(int TagId)
    {
        return Ok(_todoModel.getPerTag(TagId));
    }
    
    [HttpPost()]
    public IActionResult Store([FromBody] TodoStoreRequest request)
    {
        Dictionary<string, dynamic> todo = new Dictionary<string, dynamic>();
        DateTime parsedDate;
        List<int> tagIds = request.TagIds.Distinct().ToList();

        todo["task"] = request.Task;
        todo["bg_color"] = request.BackgroundColor;

        if (DateTime.TryParse(request.DueDate, out parsedDate)) {
            todo["due_date"] = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
        }

        Dictionary<string, dynamic> newTodo = _todoModel.Store(todo);

        if (tagIds.Count > 0) {
            foreach (int tagId in tagIds) {
                _todoModel.addTag(newTodo["id"], tagId);
            }
        }

        return Ok(newTodo);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int todoId, [FromBody] TodoUpdateRequest request)
    {
        Dictionary<string, dynamic> todo = new Dictionary<string, dynamic>();
        Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
        List<int> tagIds = request.TagIds.Distinct().ToList();

        todo["task"] = addSingleQuote(request.Task.ToString());
        todo["bg_color"] = addSingleQuote(request.BackgroundColor.ToString());
        todo["due_date"] = addSingleQuote(request.DueDate.ToString());
        todo["completed"] = request.Completed;
        condition["id"] = request.Id;

        _todoModel.Update(todo, condition);

        if (typeof(int).IsInstanceOfType(request.Id)) {
             _todoModel.clearTags(request.Id);

            if (tagIds.Count > 0) {
                foreach (int tagId in tagIds) {
                    _todoModel.addTag(request.Id, tagId);
                }
            }
        }

        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Archive(int id)
    {
        Dictionary<string, dynamic> conditions = new Dictionary<string, dynamic>();
        conditions["id"] = id;

        _todoModel.Archive(conditions);

        return Ok();
    }

    [HttpPost("{id}/tag")]
    public IActionResult AddTag([FromBody] TodoAddTagRequest request)
    {
       _todoModel.addTag(request.TodoId, request.TagId);

        return Ok();
    }

     [HttpDelete()]
    public IActionResult RemoveTag([FromBody] TodoRemoveTagRequest request)
    {
        _todoModel.removeTag(request.TodoId, request.TagId);
        
        return Ok();
    }

    private string addSingleQuote(string str)
    {
        return "'" + str + "'";
    }
}