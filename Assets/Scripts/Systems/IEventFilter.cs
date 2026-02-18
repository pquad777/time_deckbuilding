public interface IEventFilter
{
    int Priority { get; }
    void BeforeExecute(ref ActionEvent e, ResolveContext ctx);
    void AfterExecute(ActionEvent executed, ResolveContext ctx);
}
