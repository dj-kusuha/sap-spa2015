using UnityEngine;
using System.Collections;
using NCMB;
using System.Collections.Generic;

public class TuntunManager : MonoBehaviour
{

    [SerializeField]
    private AudioSource tapSESource;

    private SelectManager.FriendData selectFriendData;

    private void Start() {
        this.selectFriendData = GameObject.FindObjectOfType<Node>().FriendData;
    }

    public void OnClickTuntunButton()
    {
        Debug.Log("OnClickTuntunButton");

        // 回数インクリメント
        this.selectFriendData.sendTuntun++;

        // SE再生
        this.tapSESource.Play();

        var push = new NCMBPush()
        {
            PushToIOS = true,
            PushToAndroid = false,
            Message = "tuntunさんからつんつんされています！",
            BadgeIncrementFlag = true,
            Category = "INVITE_CATEGORY",
        };

        push.SendPush();
    }

    public void OnClickBackButton() {
        Application.LoadLevel( "Select" );
    }
}

