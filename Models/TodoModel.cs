using test_project.Models;

namespace test_project.Models;

public class TodoModel: BaseModel
{
    public TodoModel(IConfiguration configuration) : base(configuration, "todos", new List<string>{"id","task","order"})
    {
        // Construct   
    }

    public List<Dictionary<string, dynamic>> getPerTag(dynamic tagId)
    {
        string Query = $"SELECT todos.* FROM todos INNER JOIN tag_todo on todos.id = tag_todo.todo_id and tag_todo.tag_id={tagId} and todos.soft_deleted=false order by `order` desc";

        return ExecuteQuery(Query, "select");
    }

    public List<Dictionary<string, dynamic>> GetTodoTags(int todoId)
    {
        string Query = $"SELECT * FROM tags where id in (SELECT tag_id FROM tag_todo WHERE todo_id={todoId})";

        return ExecuteQuery(Query, "select");
    }

    public void addTag(int? todoId, int tagId)
    {
        string Query = $"INSERT INTO tag_todo (todo_id, tag_id) VALUES ({todoId}, {tagId})";

        ExecuteQuery(Query);
    }

    public void removeTag(int todoId, int tagId)
    {
        string Query = $"DELETE FROM tag_todo where todo_id={todoId} and tag_id={tagId}";

        ExecuteQuery(Query);
    }

    public void clearTags(int? todoId) 
    {
        string Query = $"DELETE FROM tag_todo where todo_id={todoId}";

        ExecuteQuery(Query);
    }
}