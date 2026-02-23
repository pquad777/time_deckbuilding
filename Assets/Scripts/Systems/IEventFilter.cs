using UnityEngine;

public interface IEventFilter
{
    int Priority { get; }
    int Remains { get; set; }
    Team Owner {get; set;}
    void BeforeExecute(ref ActionEvent e, ResolveContext ctx);
    void AfterExecute(ActionEvent executed, ResolveContext ctx);
    public Sprite ReturnSprite();
}
