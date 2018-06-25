using UnityEngine;
using UnityEngine.UI;

public class TabLabelController : MonoBehaviour {

    public TabLabelBarController barController;

	public Text tabTitle;

	private Image selfImage;

	public void Init () {
		selfImage = GetComponent<Image> ();
        barController = GetComponentInParent<TabLabelBarController>();
        TurnOff();
	}

	public void TurnOn () {
		selfImage.color = barController.onBackgroundColor;
		tabTitle.fontStyle = FontStyle.Bold;
		tabTitle.color = barController.onTextColor;
	}

	public void TurnOff () {
		selfImage.color = barController.offBackgroundColor;
		tabTitle.fontStyle = FontStyle.Normal;
		tabTitle.color = barController.offTextColor;
	}

}
