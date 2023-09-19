namespace test_project.Http.Requests;

public class TodoStoreRequest
{
    public string Task {get; set;}
    public string BackgroundColor {get; set;}
     public string? DueDate {get; set;} = null;

     public List<int>? TagIds {get; set;} = null;
}