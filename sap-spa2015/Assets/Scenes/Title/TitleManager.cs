using UnityEngine;
using System.Collections;

public class TitleManager : MonoBehaviour {

    private void Start() {
        NCMB.NCMBPush.ClearAll();
    }

	public void OnClickStartButton() {
		ClickSEManager.Instance.PlaySE();
		Application.LoadLevel("Select");
	}

}
