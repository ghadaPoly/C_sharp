public class UserCounterService
{
    public int Count { get; set; } = 0;

    public void Increment()
    {
        Count++;
    }
}
