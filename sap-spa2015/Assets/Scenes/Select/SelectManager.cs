using UnityEngine;
using System.Collections;
using System.Linq;

public class SelectManager : MonoBehaviour
{
    [System.Serializable]
    public class FriendData {
        public Sprite sprite;
        public string name;
        public int sendTuntun;
        public int recvTuntun;
    }

    [Header( "Debug Data" )]
    [SerializeField]
    private FriendData[] debugFriendData;

    [Header("Texts")]
    [SerializeField]
    private UnityEngine.UI.Text totalTuntunText;
    
    [Header("GameObjects")]
    [SerializeField]
    private GameObject contentObject;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject groupPrefab;

    [SerializeField]
    private GameObject nodePrefab;

    private static FriendData[] nowFriendData;

    public static int TotalTuntun{get;set;}

    // Use this for initialization
    void Start() {
        // デバッグデータからコピー
        if( nowFriendData == null ){
            nowFriendData = new FriendData[this.debugFriendData.Length];
            for(int i = 0; i < nowFriendData.Length; ++i){
                nowFriendData[i] = this.debugFriendData[i];
            }
        }
        
        // 既存のノードを拾ってデータ更新してから破棄
        {
            var node = GameObject.FindObjectOfType<Node>();
            if( node != null ) {
                for( int i = 0; i < nowFriendData.Length; ++i ) {
                    if( nowFriendData[ i ].name == node.FriendData.name ) {
                        nowFriendData[ i ] = node.FriendData;
                        TotalTuntun += node.FriendData.sendTuntun;
                        break;
                    }
                }
                Destroy( node.gameObject );
            }
        }



        // デバッグデータから生成する
        int cnt = 0;
        GameObject currentGroup = null;
        for( int i = 0; i < nowFriendData.Length; ++i ) {
            // 対象となるgroupがなかったら生成
            if( currentGroup == null ) {
                currentGroup = (GameObject)Instantiate( this.groupPrefab );
                currentGroup.transform.SetParent( this.contentObject.transform );
                currentGroup.transform.localScale = this.groupPrefab.transform.localScale;
            }
            // node生成
            var node = Instantiate( this.nodePrefab );
            node.transform.SetParent( currentGroup.transform );
            node.transform.localScale = this.nodePrefab.transform.localScale;
            node.GetComponent<Node>().SetData( nowFriendData[ i ] );

            // 個数上限を超えたらcurrentGroupをnullにしておく
            cnt++;
            if( cnt % 2 == 0 ) {
                currentGroup = null;
            }
        }
        
        // つんつん合計表示の更新
        this.totalTuntunText.text = TotalTuntun.ToString("#,##0");
    }

}
