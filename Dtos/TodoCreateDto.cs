namespace TodoApi.Dtos;

public class TodoCreateDto
{
    public string Title {get; set;} = string.Empty;
    public bool IsCompleted {get; set;} = false;
}