
public class MagnitChecker : Checker
{
    public Container Container { get; private set; }

    private void Awake()
    {
        Container = GetComponentInParent<Container>();
    }
   
}
