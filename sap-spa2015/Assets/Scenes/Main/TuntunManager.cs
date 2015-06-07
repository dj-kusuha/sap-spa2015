using NCMB;
using UnityEngine;
using UnityEngine.UI;

public class TuntunManager : MonoBehaviour
{

    [Header("Sounds")]
    [SerializeField]
    private AudioSource tapSESource;
    [SerializeField]
    private AudioSource turnSESource;

    [Header("Node")]
    [SerializeField]
    private Node node;

    [Header("Check Objects")]
    [SerializeField]
    private GameObject noCheckedObject;
    [SerializeField]
    private GameObject checkedObject;

    private SelectManager.FriendData selectFriendData;

    private float timer;


    private bool isTuntun;


    private void Start()
    {
        SetChecked(false, true);

        this.selectFriendData = GameObject.FindObjectOfType<Node>().FriendData;
        if(this.selectFriendData == null){
            this.selectFriendData = new SelectManager.FriendData();
        }
        this.node.SetData(this.selectFriendData);
    }

    private void Update()
    {
        if (this.timer > 0)
        {
            this.timer -= Time.deltaTime;
            if (this.timer <= 0)
            {
                SetChecked(false);
            }
        }
    }

    private void OnEnable()
    {
        NCMBManager.onNotificationReceived += OnNotificationReceived;
    }

    private void OnDisable()
    {
        NCMBManager.onNotificationReceived -= OnNotificationReceived;
    }

    private bool prevCheckedFlag;
    private void SetChecked(bool checkedFlag, bool forceChange = false)
    {
        if (forceChange || prevCheckedFlag != checkedFlag)
        {
            // 表示切り替え
            this.noCheckedObject.SetActive(!checkedFlag);
            this.checkedObject.SetActive(checkedFlag);
            // SE再生
            if(!forceChange) {
                this.turnSESource.Play();
            }
            // フラグ更新
            this.prevCheckedFlag = checkedFlag;
        }
    }


    public void OnClickTuntunButton()
    {
        Debug.Log("OnClickTuntunButton");

        // 回数インクリメント
        this.selectFriendData.sendTuntun++;

        // 画面再設定
        this.node.SetData(this.selectFriendData);

        // SE再生
        this.tapSESource.Play();

        var push = new NCMBPush()
        {
            PushToIOS = true,
            PushToAndroid = false,
            Message = "tuntunさんからつんつんされています！",
            BadgeIncrementFlag = true,
            Category = "TUNTUN",
        };

        push.SendPush();

        this.isTuntun = true;
    }

    public void OnClickBackButton()
    {
        Application.LoadLevel("Select");
    }

    private void OnNotificationReceived(NCMBPushPayload payload)
    {
        if (this.isTuntun && payload.Message == "checked")
        {
            Debug.Log("checked!");

            SetChecked(true);
            this.timer = 180f;

            this.isTuntun = false;
        }
    }
}