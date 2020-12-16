
public class FSM<T>
{
    public State<T> current;

    public FSM(State<T> initialState)
    {
        current = initialState;
        current.Enter();
    }

    public void Update()
    {
        current.Update();
    }

    public void Feed(T input)
    {
        var next = current.GetTransition(input);
        if (next != null)
        {
            current.Exit();
            current = next;
            current.Enter();
        }
    }


}
