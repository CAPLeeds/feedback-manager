using UnityEngine;
using UnityEngine.UI;

public class PlaceholderController : MonoBehaviour {

    public Image background;
    public Text text;

    public Vector2 GetOffsetMin ()
    {
        return transform.GetComponent<RectTransform>().offsetMin;
    }

    public Vector2 GetOffsetMax ()
    {
        return transform.GetComponent<RectTransform>().offsetMax;
    }

}
