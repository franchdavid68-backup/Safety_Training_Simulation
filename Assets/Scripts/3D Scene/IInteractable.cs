using System.Collections.Generic;

public interface IInteractable 
{
    void Interact();
    string GetDescription();
    void Highlight();
    void Unhighlight();
}