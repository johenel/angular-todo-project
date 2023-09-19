namespace test_project.Http.Requests;

public class TodoUpdateRequest
{
    public int? Id {get; set;} = null;
    public string Task {get; set;}
    public string BackgroundColor {get; set;}
    public dynamic? DueDate {get; set;} = null;
    public bool Completed {get; set;}
    public List<int> TagIds {get; set;}
}