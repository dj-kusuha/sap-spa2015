using UnityEngine;
using System.Collections;
using NCMB;
using System.Collections.Generic;

public class TuntunManager : MonoBehaviour
{

    
    [SerializeField]
    private UnityEngine.UI.Text text;

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









    /// <summary>
    ///イベントリスナーの登録
    /// </summary>
    void OnEnable()
    {
        NCMBManager.onSendPush += OnSendPush;
        NCMBManager.onRegistration += OnRegistration;
        NCMBManager.onNotificationReceived += OnNotificationReceived;
    }

    /// <summary>
    ///イベントリスナーの削除
    /// </summary>
    void OnDisable()
    {
        NCMBManager.onSendPush -= OnSendPush;
        NCMBManager.onRegistration -= OnRegistration;
        NCMBManager.onNotificationReceived -= OnNotificationReceived;
    }

    /// <summary>
    ///端末登録後のイベント
    /// </summary>
    void OnRegistration(string errorMessage)
    {
        if (errorMessage == null)
        {
            Debug.Log("OnRegistrationSucceeded");
        }
        else
        {
            Debug.Log("OnRegistrationFailed:" + errorMessage);
        }
    }

    /// <summary>
    ///プッシュ送信後のイベント
    /// </summary>
    void OnSendPush(string errorMessage)
    {
        if (errorMessage == null)
        {
            Debug.Log("OnSendPushSucceeded");
        }
        else
        {
            Debug.Log("OnSendPushFailed:" + errorMessage);
        }
    }

    /// <summary>
    ///メッセージ受信後のイベント
    /// </summary>
    void OnNotificationReceived(NCMBPushPayload payload)
    {
        Debug.Log("OnNotificationReceived");

        Debug.Log("PushId : " + payload.PushId);
        Debug.Log("Massage : " + payload.Message);
#if UNITY_ANDROID
        Debug.Log("Title : " + payload.Title);
#endif

        this.text.text = payload.ToString();
    }


}

