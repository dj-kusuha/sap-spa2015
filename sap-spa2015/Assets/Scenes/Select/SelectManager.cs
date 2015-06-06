using UnityEngine;
using System.Collections;

public class SelectManager : MonoBehaviour
{

    [Header("GameObjects")]
    [SerializeField]
    private GameObject contentObject;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject groupPrefab;

    [SerializeField]
    private GameObject nodePrefab;

    // Use this for initialization
    void Start()
    {
        // TODO debug
        int cnt = 0;
        GameObject currentGroup = null;
        for (int i = 0; i < 4; ++i)
        {
            // 対象となるgroupがなかったら生成
            if (currentGroup == null)
            {
                currentGroup = (GameObject)Instantiate(this.groupPrefab);
                currentGroup.transform.SetParent(this.contentObject.transform);
                currentGroup.transform.localScale = this.groupPrefab.transform.localScale;
            }
            // node生成
            var node = Instantiate(this.nodePrefab);
            node.transform.SetParent(currentGroup.transform);
            node.transform.localScale = this.nodePrefab.transform.localScale;

            // 個数上限を超えたらcurrentGroupをnullにしておく
            cnt++;
            if (cnt % 2 == 0) { 
				currentGroup = null;
			}
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
