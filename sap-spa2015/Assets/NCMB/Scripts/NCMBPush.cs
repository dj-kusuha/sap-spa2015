/*******
 Copyright 2014 NIFTY Corporation All Rights Reserved.
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
 http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
 **********/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MiniJSON;

namespace NCMB
{
	/// <summary>
	/// プッシュ操作を扱います。
	/// </summary>
	public class NCMBPush
	{
		private IDictionary<string, object> pushData = new Dictionary<string, object> ();
		#if UNITY_ANDROID
			static AndroidJavaClass m_AJClass;
		#elif UNITY_IOS
			[DllImport ("__Internal")]
			private static extern void initialize (string applicationId, string clientId);

			[DllImport ("__Internal")]
			private static extern void registerNotification (bool useAnalytics);

			[DllImport ("__Internal")]
			private static extern void registerNotificationWithLocation ();

			[DllImport ("__Internal")]
			private static extern void sendPush (string json, string message, int delayByMilliseconds, bool dialog);

			[DllImport ("__Internal")]
			private static extern void clearAll ();
		#endif
			/*** 初期化 ***/
			static NCMBPush ()
			{
				#if UNITY_ANDROID && !UNITY_EDITOR
					m_AJClass = new AndroidJavaClass("com.nifty.cloud.mb.NCMBPushProxy");
				#endif
			}

			/// <summary>
			/// NCMBクラスでプッシュの初期化を行う
			/// </summary>
			/// <param name="applicationId">アプリケーションキー</param>
			/// <param name="clientId">クライアントキー</param>
			internal static void Init (string applicationId, string clientId)
			{
				#if UNITY_EDITOR
				#elif UNITY_ANDROID
					m_AJClass.CallStatic("initialize", applicationId, clientId);
				#elif UNITY_IOS
					initialize(applicationId, clientId);
				#endif
			}

			/// <summary>
			/// コンストラクター。<br/>
			///プッシュの作成を行います。
			/// </summary>
			public NCMBPush ()
			{
				//プッシュ設定を保持する
				this.pushData = new Dictionary<string, object> ();
				#if UNITY_ANDROID && !UNITY_EDITOR
					m_AJClass = new AndroidJavaClass("com.nifty.cloud.mb.NCMBPushProxy");
				#endif
			}

		#if UNITY_ANDROID
			public static void Register (string senderId, bool useAnalytics)
			{
				if (!string.IsNullOrEmpty (senderId)) {				
					#if !UNITY_EDITOR
						m_AJClass.CallStatic("registerNotification", senderId,useAnalytics);
					#endif
				}
			}
		#elif UNITY_IOS
			public static void Register (bool useAnalytics)
			{
				#if !UNITY_EDITOR
					registerNotification(useAnalytics);
				#endif
			}
		#endif

		#if UNITY_ANDROID
			public static void RegisterWithLocation (string senderId)
			{
				if (!string.IsNullOrEmpty (senderId)) {
					
					#if !UNITY_EDITOR
						m_AJClass.CallStatic("registerNotificationWithLocation", senderId);
					#endif
				}
			}
		#elif UNITY_IOS
			/// <summary>
			/// Register for receiving remote notifications (with current location).
			/// </summary>
			internal static void RegisterWithLocation ()
			{
				#if !UNITY_EDITOR
					registerNotificationWithLocation();
				#endif
			}
		#endif
			/*** Push設定 ***/

			/// <summary>
			/// メッセージの取得、または設定を行います。
			/// </summary>
			public string Message {
				get {
					string message = null;
					if (pushData.ContainsKey ("message")) {
						message = (string)this.pushData ["message"];
					}
					return message;
				}
				set { this.pushData ["message"] = value; }
			}

			/// <summary>
			/// 配信時間(ミリ秒)の取得、または設定を行います。<br/>
			/// 指定ミリ秒後にPush通知の配信を行います。
			/// </summary>
			public int DelayByMilliseconds {
				get {
					int delayByMilliseconds = 0;
					if (pushData.ContainsKey ("delayByMilliseconds")) {
						delayByMilliseconds = (int)this.pushData ["delayByMilliseconds"];
					}
					return delayByMilliseconds;
				}
				set { this.pushData ["delayByMilliseconds"] = value; }
			}

			/// <summary>
			/// 配信時間(日付)の取得、または設定を行います。<br/>
			/// 指定した時間にPushの配信を行います。
			/// </summary>
			public DateTime DeliveryDate {
				get {
					DateTime deliveryDate = DateTime.Now;
					if (pushData.ContainsKey ("DeliveryDate")) {
						deliveryDate = (DateTime)this.pushData ["DeliveryDate"];
					}
					return deliveryDate;
				}
				set { this.pushData ["DeliveryDate"] = value; }
			}

			/// <summary>
			/// タイトルの取得、または設定を行います(Androidのみ)。
			/// </summary>
			public string Title {
				get {
					string title = null;
					if (pushData.ContainsKey ("title")) {
						title = (string)this.pushData ["title"];
					}
					return title;
				}
				set { this.pushData ["title"] = value; }
			}

			/// <summary>
			/// iOS端末へ送信フラグの取得、または設定を行います。<br/>
			/// target = [ios, android] または target = [android]の時にfalseを返します。
			/// </summary>
			public bool PushToIOS {
				get {
					bool pushToIOSFlag = false;
					if (pushData.ContainsKey ("target")) {
						string[] target = (string[])this.pushData ["target"];
						foreach (string value in target) {
							if (value == "ios") {
								pushToIOSFlag = true;
							}
						}
					}
					return pushToIOSFlag;
				}
				set {
					bool pushToAndroidFlag = this.PushToAndroid;
					if (value == true && pushToAndroidFlag == false) {
						this.pushData ["target"] = new string[]{ "ios" };
					} else if (value == false && pushToAndroidFlag == true) {
							this.pushData ["target"] = new string[]{ "android" };
					} else {
						//[true,true] or [false,false]
						if (pushData.ContainsKey ("target")) {
							pushData.Remove ("target");
						}
					}
				}
			}

			/// <summary>
			/// Android端末へ送信フラグの取得、または設定を行います。<br/>
			/// target = [ios, android] または target = [ios]の時にfalseを返します。
			/// </summary>
			public bool PushToAndroid {
				get {
					bool PushToAndroidFlag = false;
					if (pushData.ContainsKey ("target")) {
						string[] target = (string[])this.pushData ["target"];
						foreach (string value in target) {
							if (value == "android") {
								PushToAndroidFlag = true;
							}
						}
					}
					return PushToAndroidFlag;
				}
				set {
					bool pushToIOSFlag = this.PushToIOS;
					if (value == true && pushToIOSFlag == false) {
						this.pushData ["target"] = new string[]{ "android" };
					} else if (value == false && pushToIOSFlag == true) {
							this.pushData ["target"] = new string[]{ "ios" };
						} else {
							//[true,true] or [false,false]
							if (pushData.ContainsKey ("target")) {
								pushData.Remove ("target");
							}
						}
				}
			}

			/// <summary>
			/// 設定するバッジ数の取得、または設定を行います(iOSのみ)。<br/>
			/// プッシュ通知のバッジ数は取得出来ません。
			/// </summary>
			public int? Badge {
				get {
					int? badge = null;
					if (pushData.ContainsKey ("badgeSetting")) {
						badge = (int)this.pushData ["badgeSetting"];
					}
					return badge;
				}
				set { this.pushData ["badgeSetting"] = value; }
			}

			/// <summary>
			/// バッジ増加フラグの取得、または設定を行います(iOSのみ)。
			/// </summary>
			public bool BadgeIncrementFlag {
				get {
					bool badgeIncrementFlag = true;
					if (pushData.ContainsKey ("badgeIncrementFlag")) {
						badgeIncrementFlag = (bool)this.pushData ["badgeIncrementFlag"];
					}
					return badgeIncrementFlag;
				}
				set { this.pushData ["badgeIncrementFlag"] = value; }
			}

			/// <summary>
			/// リッチプッシュURLの取得、または設定を行います。
			/// </summary>
			public string RichUrl {
				get {
					string richUrl = null;
					if (pushData.ContainsKey ("richUrl")) {
						richUrl = (string)this.pushData ["richUrl"];
					}
					return richUrl;
				}
				set { this.pushData ["richUrl"] = value; }
			}

			/// <summary>
			/// ダイアログプッシュの取得、または設定を行います(Androidのみ)。
			/// </summary>
			public bool Dialog {
				get {
					bool dialog = false;
					if (pushData.ContainsKey ("dialog")) {
						dialog = (bool)this.pushData ["dialog"];
					}
					return dialog;
				}
				set { this.pushData ["dialog"] = value; }
			}

			/// <summary>
			/// ContentAvailableの取得、または設定を行います(iOSのみ)。
			/// </summary>
			public bool ContentAvailable {
				get {
					bool contentAvailable = false;
					if (pushData.ContainsKey ("contentAvailable")) {
						contentAvailable = (bool)this.pushData ["contentAvailable"];
					}
					return contentAvailable;
				}
				set { this.pushData ["contentAvailable"] = value; }
			}

			/// <summary>
			/// カテゴリーの取得、または設定を行います(iOSのみ)。
			/// </summary>
			public string Category {
				get {
					string category = null;
					if (pushData.ContainsKey ("category")) {
						category = (string)this.pushData ["category"];
					}
					return category;
				}
				set { this.pushData ["category"] = value; }
			}

			/// <summary>
			/// 配信期限日の取得、または設定を行います。
			/// </summary>
			public DateTime? DeliveryExpirationDate {
				get {
					DateTime? DeliveryExpirationDate = null;
					if (pushData.ContainsKey ("deliveryExpirationDate")) {
						DeliveryExpirationDate = (DateTime)this.pushData ["deliveryExpirationDate"];
					}
					return DeliveryExpirationDate;
				}
				set { this.pushData ["deliveryExpirationDate"] = value; }
			}

			/// <summary>
			/// 配信期限時間の取得、または設定を行います。<br/>
			/// 時間単位で指定する場合は「n hour」(n=1～24）、<br/>
			/// 日単位で指定する場合は「n day」（n=1～28） を設定します。
			/// </summary>
			public string DeliveryExpirationTime {
				get {
					string category = null;
					if (pushData.ContainsKey ("deliveryExpirationTime")) {
						category = (string)this.pushData ["deliveryExpirationTime"];
					}
					return category;
				}
				set { this.pushData ["deliveryExpirationTime"] = value; }
			}

			/*** Push送信 ***/

			/// <summary>
			/// プッシュの送信を行います。
			/// </summary>
			public void SendPush ()
			{

				//ローカル
				string setLocalMessge = null;

				//エラー判定
				if (pushData.ContainsKey ("DeliveryDate") && pushData.ContainsKey ("delayByMilliseconds")) {
					throw new ArgumentException ("DeliveryDate and DelayByMilliseconds can not be set at the same time.Please set only one.");
				} else if (pushData.ContainsKey ("deliveryExpirationDate") && pushData.ContainsKey ("deliveryExpirationTime")) {
					throw new ArgumentException ("DeliveryExpirationDate and DeliveryExpirationTime can not be set at the same time.Please set only one.");
				}
			
				//本文設定
				string message = this.Message;
				if (message != null) {
					pushData.Remove ("message");
				}

				//配信時間設定
				int delayByMilliseconds = 0;
				if (pushData.ContainsKey ("delayByMilliseconds")) {
					delayByMilliseconds = this.DelayByMilliseconds;
					pushData.Remove ("delayByMilliseconds");
				}

				//ダイアログプッシュ設定
				bool dialog = false;
				if (pushData.ContainsKey ("dialog")) {
					if ((bool)pushData ["dialog"] == true) {
						this.pushData ["action"] = "nifty.com.push.unity.RECEIVE_PUSH";
						dialog = true;
					}
					pushData.Remove ("dialog");
				}
				
				//その他プッシュ設定
				string json = Json.Serialize (this.pushData);
				SendPush (json, message, dialog, delayByMilliseconds);

				if (setLocalMessge != null) {
					this.pushData ["message"] = setLocalMessge; 
				}
				if (dialog == true) {
					this.pushData ["dialog"] = dialog; 
				}
			}

			/// <summary>
			/// iOSとAndroidのネイティブコードにデータを送信する
			/// </summary>
			/// <param name="json">JSONData</param>
			/// <param name="message">メッセージ</param>
			/// <param name="delayByMilliseconds">配信時間</param>
			/// <param name="dialog">ダイアログ</param>
			private static void SendPush (string json, string message, bool dialog, int delayByMilliseconds)
			{
				#if UNITY_EDITOR
					UnityEngine.Debug.Log( "EditorMode: Json = " + json );
					UnityEngine.Debug.Log( "EditorMode: Message = " + message );
					UnityEngine.Debug.Log( "EditorMode: Dialog = " + dialog );
					UnityEngine.Debug.Log( "EditorMode: DelayByMilliseconds = " + delayByMilliseconds );
				#elif UNITY_ANDROID
					m_AJClass.CallStatic("sendPush", json, message, delayByMilliseconds, dialog);
				#elif UNITY_IOS
					sendPush(json, message, delayByMilliseconds,dialog);
				#endif
			}

		#if UNITY_IOS
			// Clears all notifications.
			public static void ClearAll ()
			{
				UnityEngine.iOS.NotificationServices.ClearRemoteNotifications ();
				#if !UNITY_EDITOR
					clearAll();
				#endif
			}
		#endif
	}
}
