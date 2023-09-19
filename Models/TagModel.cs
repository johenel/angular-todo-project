using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using test_project.Models;

namespace test_project.Models;

public class TagModel: BaseModel
{
    public TagModel(IConfiguration configuration) : base(configuration, "tags", new List<string>{"id","name","count"})
    {
        // Construct   
    }

    public List<Dictionary<string, dynamic>> getTopTags()
    {
        string Query = $"select tags.id, tags.name, sum((case when (select count(tag_id) from tag_todo where tag_todo.tag_id=tags.id)=0 then 0 else (case when (select count(tag_id) from tag_todo left join todos on todos.id=tag_todo.todo_id where tag_todo.tag_id=tags.id and todos.soft_deleted=false) != 0 then (select count(tag_id) from tag_todo left join todos on todos.id=tag_todo.todo_id where tag_todo.tag_id=tags.id and todos.soft_deleted=false) else 0 end) end)) as count from tags group by tags.id order by count desc";

        return ExecuteQuery(Query, "select");
    }
}