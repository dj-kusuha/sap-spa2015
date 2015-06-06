using UnityEngine;
using System.Collections;

public class TitleManager : MonoBehaviour {

    private void Start() {
        NCMB.NCMBPush.ClearAll();

#if UNITY_IPHONE
        var notifications = UnityEngine.iOS.NotificationServices.remoteNotifications;
        foreach( var notification in notifications ) {
            Debug.Log( "Notification :" );
            Debug.Log( notification.alertBody );
            Debug.Log( notification.applicationIconBadgeNumber );
            Debug.Log( notification.hasAction );
            Debug.Log( notification.soundName );
}
#endif
    }

	public void OnClickStartButton() {
		Application.LoadLevel("Select");
	}

}
