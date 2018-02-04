using UnityEngine;
using System.Collections;

public class ProxySimpleNumberModule : MonoBehaviour {

    public static SimpleNumberInputerController Open()
    {
        var numberInputer = SimpleNumberInputerController.Show<SimpleNumberInputerController>(SimpleNumberInputer.NAME, UILayerType.FiveModule, false);
        return numberInputer;
    }

}
