using UnityEngine;
using System.Collections;

/////////////////////////ここから追加コード////////////////////////
using NCMB;
/////////////////////////ここまで追加コード////////////////////////

using System.Collections.Generic;

public class PushObject : MonoBehaviour {

    /////////////////////////ここから追加コード////////////////////////
    private static bool _isInitialized = false;

    /// <summary>
    ///イベントリスナーの登録
    /// </summary>
    void OnEnable() {
        NCMBManager.onSendPush += OnSendPush;
        NCMBManager.onRegistration += OnRegistration;
        NCMBManager.onNotificationReceived += OnNotificationReceived;
    }

    /// <summary>
    ///イベントリスナーの削除
    /// </summary>
    void OnDisable() {
        NCMBManager.onSendPush -= OnSendPush;
        NCMBManager.onRegistration -= OnRegistration;
        NCMBManager.onNotificationReceived -= OnNotificationReceived;
    }

    /// <summary>
    ///端末登録後のイベント
    /// </summary>
    void OnRegistration( string errorMessage ) {
        if( errorMessage == null ) {
            Debug.Log( "OnRegistrationSucceeded" );
        } else {
            Debug.Log( "OnRegistrationFailed:" + errorMessage );
        }
    }

    /// <summary>
    ///プッシュ送信後のイベント
    /// </summary>
    void OnSendPush( string errorMessage ) {
        if( errorMessage == null ) {
            Debug.Log( "OnSendPushSucceeded" );
        } else {
            Debug.Log( "OnSendPushFailed:" + errorMessage );
        }
    }

    /// <summary>
    ///メッセージ受信後のイベント
    /// </summary>
    void OnNotificationReceived( NCMBPushPayload payload ) {
        Debug.Log( "OnNotificationReceived" );
    }

    /// <summary>
    ///シーンを跨いでGameObjectを利用する
    /// </summary>
    public virtual void Awake() {
        if( !PushObject._isInitialized ) {
            PushObject._isInitialized = true;
            DontDestroyOnLoad( this.gameObject );
        } else {
            Destroy( this.gameObject );
        }
    }


    /////////////////////////ここまで追加コード////////////////////////

}

