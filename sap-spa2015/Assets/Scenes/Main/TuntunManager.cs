using NCMB;
using UnityEngine;

public class TuntunManager : MonoBehaviour {

    [SerializeField]
    private AudioSource tapSESource;

    [SerializeField]
    private Node node;

    [SerializeField]
    private GameObject checkedTextObject;

    private SelectManager.FriendData selectFriendData;

    private void Start() {
        this.checkedTextObject.SetActive( false );

        this.selectFriendData = GameObject.FindObjectOfType<Node>().FriendData;
        this.node.SetData( this.selectFriendData );
    }


    private void OnEnable() {
        NCMBManager.onNotificationReceived += OnNotificationReceived;
    }

    private void OnDisable() {
        NCMBManager.onNotificationReceived -= OnNotificationReceived;
    }

    public void OnClickTuntunButton() {
        Debug.Log( "OnClickTuntunButton" );

        // 回数インクリメント
        this.selectFriendData.sendTuntun++;

        // 画面再設定
        this.node.SetData( this.selectFriendData );

        // SE再生
        this.tapSESource.Play();

        var push = new NCMBPush() {
            PushToIOS = true,
            PushToAndroid = false,
            Message = "tuntunさんからつんつんされています！",
            BadgeIncrementFlag = true,
            Category = "TUNTUN",
        };

        push.SendPush();
    }

    public void OnClickBackButton() {
        Application.LoadLevel( "Select" );
    }

    private void OnNotificationReceived( NCMBPushPayload payload ) {
        if( payload.Message == "checked" ) {
            Debug.Log( "checked!" );

            this.checkedTextObject.SetActive( true );
        }
    }
}