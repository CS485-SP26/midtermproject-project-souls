using UnityEngine;

public interface IWaterable //you dont want an interface any bigger than this, linked to tile selector
{

    void Water(float amount); //respond to someone trying to water me with an amount

}
