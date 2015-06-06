﻿/*******
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

using System;
using System.Collections;//IEnumerator
using System.Collections.Generic;


namespace NCMB.Internal
{
	//Remove操作の履歴操作を扱う
	internal class NCMBRemoveOperation : INCMBFieldOperation
	{
	
		ArrayList objects = new ArrayList ();//追加
		public NCMBRemoveOperation (object values)
		{
			//List等
			if (values is IEnumerable) {
				IEnumerable newValues = (IEnumerable)values;
				IEnumerator obj = newValues.GetEnumerator ();
				while (obj.MoveNext()) {
					object val = (object)obj.Current;
					objects.Add (val);
				}
			} else {
				this.objects.Add (values);
			}
		}
		//AndroidのmaybeReferenceAndEncode注意
		public object Encode ()
		{
			Dictionary<string, object> dic = new Dictionary<string, object> ();
			dic.Add ("__op", "Remove");
			dic.Add ("objects", NCMBUtility._maybeEncodeJSONObject (new ArrayList (this.objects), true));
			return dic;
		}

		//前回の履歴データを元に新規履歴データの作成
		public INCMBFieldOperation MergeWithPrevious (INCMBFieldOperation previous)
		{
			//過去のデータ操作履歴はなし
			if (previous == null) {
				return this;
			}
			//前回すでに削除(Remove実行)されていれば空のリストを返す
			if (previous is NCMBDeleteOperation) {
				return new NCMBSetOperation (new ArrayList ());
			}

			//前回データ操作がSetOperationだった場合はSetOPerationの値を返す
			if (previous is NCMBSetOperation) {
				object value = ((NCMBSetOperation)previous).getValue ();
				if ((value is IList)) {
					return new NCMBSetOperation (Apply (value, null, null));
				}
				throw new  InvalidOperationException ("You can only add an item to a List.");
			}

			//前回RemoveAllFromList(objectIdがある時)の場合、重複しない値の追加
			if (previous is NCMBRemoveOperation) {
				ArrayList result = new ArrayList (((NCMBRemoveOperation)previous).objects);
				foreach (object o in this.objects) {
					result.Add (o);
				}
				//result.UnionWith (this.objects);//重複しない値の追加
				return new NCMBRemoveOperation (result);
			}
			throw new  InvalidOperationException ("Operation is invalid after previous operation.");
		}

		//前回のローカルデータから新規ローカルデータの作成
		public object Apply (object oldValue, NCMBObject obj, string key)
		{
			//前回のローカルデータ(estimatedDataに指定のキーが無い場合)がNullの場合
			if (oldValue == null) {
				//return new List<object> ();
				return new ArrayList ();//追加
			}
			//配列のみリムーブ実行
			if ((oldValue is IList)) {
			
				//取り出したローカルデータから今回の引数で渡されたオブジェクトの削除
				ArrayList result = new ArrayList ((IList)oldValue);
				foreach (object removeObj in this.objects) {
					while (result.Contains(removeObj)) {//removeAllと同等
						result.Remove (removeObj);
					}
				}
				//以下「NCMBObjectが保存されているかつ、estimatedData(result)の中の一致するNCMBObjectが消せなかった」時の処理
				
				//今回引数で渡されたオブジェクトからオブジェクトの削除
				ArrayList objectsToBeRemoved = new ArrayList ((IList)this.objects);

				foreach (object removeObj2 in result) {
					while (objectsToBeRemoved.Contains(removeObj2)) {//removeAllと同等
						objectsToBeRemoved.Remove (removeObj2);
					}
				}
								
				//結果のリスト（引数）の中のNCMBObjectがすでに保存されている場合はobjectIdを返す
				//まだ保存されていない場合はnullを返す
				HashSet<object> objectIds = new HashSet<object> ();
				foreach (object hashSetValue in objectsToBeRemoved) {
					if (hashSetValue is NCMBObject) {
						NCMBObject valuesNCMBObject = (NCMBObject)hashSetValue;
						objectIds.Add (valuesNCMBObject.ObjectId);
					}
					
					//resultの中のNCMBObjectからobjectIdsの中にあるObjectIdと一致するNCMBObjectの削除
					object resultValue;
					for (int i = 0; i < result.Count; i++) {
						resultValue = result [i];
						if (resultValue is NCMBObject) {
							NCMBObject resultNCMBObject = (NCMBObject)resultValue;
							if (objectIds.Contains (resultNCMBObject.ObjectId)) {
								result.RemoveAt (i);
							}
						}
					}
				}
				return result;
			}
			throw new  InvalidOperationException ("Operation is invalid after previous operation.");
		}
	}
}
