namespace TodoApi.Dtos;

public class TodoUpdateDto
{
    public string Title {get; set;} = string.Empty;
    public bool IsCompleted {get; set;}
}