using UnityEngine;using System.Collections;using UnityEngine.UI;public class Node : MonoBehaviour {    [SerializeField]    private Text nameText;    [SerializeField]    private Text sendText;    [SerializeField]    private Text recvText;

    public SelectManager.FriendData FriendData { get; private set; }	// Use this for initialization
    public void SetData( SelectManager.FriendData data ) {
        this.FriendData = data;

        if( this.nameText != null ) {
            this.nameText.text = data.name;
        }
        if( this.sendText != null ) {
            this.sendText.text = data.sendTuntun.ToString( "#,##0" );
        }
        if( this.recvText != null ) {
            this.recvText.text = data.recvTuntun.ToString( "#,##0" );
        }
    }

    public void OnClick() {
        Debug.Log( "OnClick:" + this.FriendData.name );

        // 選択されたフレンドデータを保存
        var obj = new GameObject( "SelectFriend", typeof( Node ) );
        obj.GetComponent<Node>().SetData( this.FriendData );

        DontDestroyOnLoad( obj );

        // つんつん画面へ
        Application.LoadLevel( "Main" );
    }}