namespace Blazey.Communications;

public record OperationStateProvider
{
    public static OperationStateProvider Empty { get => new(); }

    public bool Success { get => Failures == 0; }

    public bool Completed { get; set; }

    public int Failures { get => ErrorMessages.Count; }

    public List<string> ErrorMessages { get; private set; } = new();

    public List<string> WarningMessages { get; private set; } = new();

    public string SuccessMessage { get; set; } = string.Empty;

    public OperationStateProvider AddError(string Message)
    {
        ErrorMessages.Add(Message);
        return this;
    }

    public OperationStateProvider AddWarning(string Message)
    {
        WarningMessages.Add(Message);
        return this;
    }

    public OperationStateProvider Reset()
    {
        ErrorMessages = new();
        WarningMessages = new();
        return this;
    }

    public OperationStateProvider Complete()
    {
        Completed = true;
        return this;
    }
}