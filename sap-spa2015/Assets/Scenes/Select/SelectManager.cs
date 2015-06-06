using UnityEngine;
using System.Collections;
using System.Linq;

public class SelectManager : MonoBehaviour
{
    [System.Serializable]
    public class FriendData {
        public string name;
        public int sendTuntun;
        public int recvTuntun;
    }

    [Header( "Debug Data" )]
    [SerializeField]
    private FriendData[] debugFriendData;

    [Header("GameObjects")]
    [SerializeField]
    private GameObject contentObject;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject groupPrefab;

    [SerializeField]
    private GameObject nodePrefab;

    // Use this for initialization
    void Start() {
        // 既存のノードを拾ってデータ更新してから破棄
        {
            var node = GameObject.FindObjectOfType<Node>();
            if( node != null ) {
                for( int i = 0; i < this.debugFriendData.Length; ++i ) {
                    if( this.debugFriendData[ i ].name == node.FriendData.name ) {
                        this.debugFriendData[ i ] = node.FriendData;
                        break;
                    }
                }
                Destroy( node.gameObject );
            }
        }



        // デバッグデータから生成する
        int cnt = 0;
        GameObject currentGroup = null;
        for( int i = 0; i < this.debugFriendData.Length; ++i ) {
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
            node.GetComponent<Node>().SetData( this.debugFriendData[ i ] );

            // 個数上限を超えたらcurrentGroupをnullにしておく
            cnt++;
            if( cnt % 2 == 0 ) {
                currentGroup = null;
            }
        }
    }

}
