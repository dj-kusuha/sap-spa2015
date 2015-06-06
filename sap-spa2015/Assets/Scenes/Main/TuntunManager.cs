using NCMB;
using UnityEngine;
using UnityEngine.UI;

public class TuntunManager : MonoBehaviour {

    [SerializeField]
    private AudioSource tapSESource;

    [SerializeField]
    private Node node;

    [Header( "Check Objects" )]
    [SerializeField]
    private Button tuntunButton;
    [SerializeField]
    private GameObject noCheckedObject;
    [SerializeField]
    private GameObject checkedObject;

    private SelectManager.FriendData selectFriendData;

    private float timer;

    private void Start() {
        SetChecked( false );

        this.selectFriendData = GameObject.FindObjectOfType<Node>().FriendData;
        this.node.SetData( this.selectFriendData );
    }

    private void Update() {
        if( this.timer > 0 ) {
            this.timer -= Time.deltaTime;
            if( this.timer <= 0 ) {
                SetChecked( false );
            }
        }
    }

    private void OnEnable() {
        NCMBManager.onNotificationReceived += OnNotificationReceived;
    }

    private void OnDisable() {
        NCMBManager.onNotificationReceived -= OnNotificationReceived;
    }


    private void SetChecked( bool checkdFlag ) {
        this.tuntunButton.enabled = !checkdFlag;
        this.noCheckedObject.SetActive( !checkdFlag );
        this.checkedObject.SetActive( checkdFlag );
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

            SetChecked( true );
            this.timer = 10f;
        }
    }
}