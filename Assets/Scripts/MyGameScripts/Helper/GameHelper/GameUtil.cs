

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppDto;
using UnityEngine;

public static class GameUtil
{
    
    public static T SafeRun<T>(Func<T> func, Action<Exception> onError = null){
        if (func == null) return default(T);
        try{
            return func();
        }catch(Exception e){
            GameDebuger.LogException (e);
            if (onError !=null) onError(e);
            return default(T);
        }
    }

    public static T SafeRun<V,T>(V v, Func<V,T> func, Action<Exception> onError = null){
        if (func == null) return default(T);
        try{
            return func(v);
        }catch(Exception e){
            GameDebuger.LogException (e);
            if (onError !=null) onError(e);
            return default(T);
        }
    }

    public static void SafeRun(Action act,Action<Exception> onError = null){
        if (act == null) return;
        try{
            act();
        }catch(Exception e){
            GameDebuger.LogException (e);
            if (onError !=null) onError(e);
        }
    }
        
    public static void SafeRun<T>(Action<T> act, T param, Action<Exception> onError = null){
        if (act == null) return;
        try{
            act(param);
        }catch(Exception e){
            GameDebuger.LogException (e);
            if (onError !=null) onError(e);
        }
    }


    public static void SafeRun<T, R>(Action<T, R> act, T t, R r){
        if (act != null)
            act(t, r);
    }

    public static void GeneralReq(
        GeneralRequest req
        , Action<GeneralResponse> respHandler = null
        , Action onSuccess = null
        , Action<ErrorResponse> onFail = null)
    {
        ServiceRequestAction.requestServer(
            req
            , ""
            , resp =>
            {
                SafeRun(respHandler, resp);
                SafeRun(onSuccess);
            }
            , error =>
            {
                TipManager.AddTopTip(error.message);
                SafeRun(onFail, error);
            });
    }

    public static void GeneralReq<T>(GeneralRequest req,Action<T> respHandler = null,Action<ErrorResponse> onFail = null) where T: GeneralResponse
    {
        ServiceRequestAction.requestServer(
            req
            ,""
            ,resp =>
            {
                respHandler.DynamicInvoke(resp);
            }
            ,error =>
            {
                TipManager.AddTopTip(error.message);
                SafeRun(onFail,error);
            });
    }
}

public static class UILabelExtension
{
    public static void SetAppVirtualItemIconAndNum(
        this UILabel label
        , AppVirtualItem.VirtualItemEnum itemId
        , int itemNum
        , string contentStr
        , string formatStr = "{0}{1}")
    {
        if (label == null) return;
        var tips = string.Format(formatStr,
            contentStr,
            ItemIconConst.GetIconConstByItemId(itemId) + " " + itemNum);
        label.text = tips;
    }
    
    public static void SetAppVirtualItemIconAndNum(
        this UILabel label
        , AppVirtualItem.VirtualItemEnum itemId
        , int itemNum
        , string contentStr
        , Color color
        , string formatStr = "{0}{1}")
    {
        if (label == null) return;
        var tips = string.Format(formatStr,
            contentStr,
            ItemIconConst.GetIconConstByItemId(itemId).WrapColor(color));
        label.text = tips;
    }
}

public static class CollectionExtension{

    public static void AddIfNotExist<T> (
        this List<T> dataSet
        , T t){
        if (dataSet == null || t == null) {
            return;
        }

        if (dataSet.IndexOf (t) < 0) {
            dataSet.Add (t);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> dataset,Action<T> act)
    {
        if (dataset == null) return;
        foreach(var item in dataset)
        {
            GameUtil.SafeRun<T>(act, item);
        }
    }

    public static void ForEachI<T>(this IEnumerable<T> dataset, Action<T, int> act){
        if (dataset == null) return;
        if (act == null) return;

        int i = 0;
        foreach(var data in dataset)
        {
            GameUtil.SafeRun<T, int>(act, data, i);
            i++;
        }
    }

    public static IEnumerable<R> Map<T, R>(this IEnumerable<T> dataset,  Func<T, R> action){
        if (dataset != null
            && action != null)
        {
            foreach (var data in dataset)
            {
                yield return action(data);
            }
        }
    }

    public static IEnumerable<R> MapI<T, R>(this IEnumerable<T> dataset,  Func<T, int, R> action){
        if (dataset != null && action != null)
        {
            var i = 0;
            foreach (var data in dataset)
            {
                yield return action(data, i);
                i++;
            }
        }
    }
    //all methods returning IEnumerable<T> require the predicate must not throw an exception
    //this is required by C# since no try catch block is allow to enclose yield return statements
    //  public static IEnumerable<FP.Tuple<T1,T2>> JoinSearch<T1,T2>(
    //      this IEnumerable<T1> set1,IEnumerable<T2> set2,Predicate<FP.Tuple<T1,T2>> pred){
    //      if (set1 != null && set2 != null)
    //          foreach ( var item1 in set1)
    //              foreach(var item2 in set2){
    //                  var pair = Tuple.Create(item1,item2);
    //                  if (pred(pair))
    //                      yield return pair;
    //              }
    //  }
    //
    //  public static IEnumerable<FP.Tuple<T1,T2>> JoinSearch<T1,T2>(
    //      this IEnumerable<T1> set1,int set1Size,IEnumerable<T2> set2,int set2Size,Predicate<FP.Tuple<T1,T2>> pred){
    //      if (set1Size < set2Size){
    //          foreach(var item in set1.JoinSearch(set2,pred))
    //              yield return item;
    //      }else{
    //          IEnumerable<FP.Tuple<T2,T1>> tmp = set2.JoinSearch<T2,T1>(set1,delegate (FP.Tuple<T2,T1> pair){
    //              return pred(FP.Tuple.Create(pair.p2,pair.p1));
    //          });
    //          foreach(var pair in tmp){
    //              yield return FP.Tuple.Create(pair.p2,pair.p1);
    //          }
    //      }
    //  }

    public static IEnumerable<T> Filter<T>(this IEnumerable <T> dataset, Predicate<T> predicate){
        if (dataset != null) {
            foreach (var item in dataset) {
                if (predicate != null) {
                    if (predicate (item)) {
                        yield return item;
                    }
                } else {
                    yield return item;
                }
            }
        } else {
            yield return default(T);
        }
    }

    private static T Find<T>(this IEnumerable<T> dataset, Predicate<T> predicate, out int idx)
    {
        idx = -1;

        if (dataset != null && predicate != null) {
            var i = 0;
            foreach(var item in dataset){
                if (item != null && predicate(item)){
                    idx = i;
                    return item;
                }
                ++i;
            }
        }

        return default(T);
    }

    public static int FindElementIdx<T>(this IEnumerable<T> dataset, Predicate<T> predicate)
    {
        var idx = -1;
        dataset.Find (predicate, out idx);
        return idx;
    }

    public static R Find<T, R>(this IEnumerable<T> dataset, Predicate<T> predicate, Func<T, R> action)
    {
        var idx = -1;
        var data = dataset.Find (predicate, out idx);
        return action(data);
    }

    public static T Find<T>(this IEnumerable<T> dataset, Predicate<T> predicate)
    {
        var idx = -1;
        return dataset.Find (predicate, out idx);
    }

    public static T[] ToArray<T>(this IEnumerable<T> dataset)
    {
        var cnt = 0;
        if (dataset == null) return null;
        foreach (T item in dataset)
        {
            cnt++;
        }

        var arr = new T[cnt];
        dataset.ForEachI(delegate(T arg1, int i)
        {
            arr[i] = arg1;
        });

        return arr;
    }

    public static List<T> ToList<T>(this IEnumerable<T> dataset)
    {
        var list = new List<T>(); 
        if (dataset != null)
            foreach(T item in dataset)
            {
                list.Add(item);
            }
        return list;
    }

    public static bool Replace<T>(this List<T> dataset, Predicate<T> predicate, T t)
    {
        bool replaceSuccess = false;
        if (!dataset.IsNullOrEmpty () && predicate != null) {
            int idx = -1;
            var item = dataset.Find (predicate, out idx);
            if (item != null) {
                dataset[idx] = t;
                replaceSuccess = true;
            }
        }
        return replaceSuccess;
    }

    public static void ReplaceOrAdd<T>(this List<T> dataset, int idx, T t)
    {
        if (dataset == null)
            dataset = new List<T>(idx + 1);
        if (dataset.Count > idx)
            dataset[idx] = t;
        else
        {
            for (var i = dataset.Count; i <= idx; i++)
            {
                var v = i == idx ? t : default(T);
                dataset.Add(v);
            }
        }
    }

    public static void ReplaceOrAdd<T>(this List<T> dataset, Predicate<T> predicate, T t)
    {
        if (t == null) return;
        var isExist = dataset.Replace (predicate, t);
        if (!isExist)
            dataset.Add (t);
    }
    public static int Count<T>(this List<T> dataSet,Predicate<T> predicate)
    {
        int count = 0;
        dataSet.ForEach(x => {
            if (predicate(x))
                count++;
        });
        return count;
    }
    public static bool RemoveItem<T>(this List<T> dataSet, T t)
    {
        if (null == t || dataSet.IsNullOrEmpty())
            return false;
        return dataSet.Remove(t);
    }

    public static bool Remove<T>(this List<T> dataSet, Predicate<T> predicate){
        bool isChange = false;
        var item = dataSet.Find<T> (predicate);
        if (item != null) {
            dataSet.Remove (item);
            isChange = true;
        }
        return isChange;
    }

    public static void RemoveItems<T>(this List<T> dataSet, Predicate<T> predicate)
    {
        if (dataSet.IsNullOrEmpty()) return;
        var set = dataSet.Filter(predicate).ToList();
        set.ForEach(s=>dataSet.RemoveItem(s));
    }

    //return old value if exists for key
    public static TValue Replace<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue val){
        if (dict == null) return default(TValue);
        TValue old;
        if (dict.TryGetValue(key,out old)){
            dict.Remove(key);
        }
        dict.Add(key,val);
        return old;
    }

    //return true if key is not present and (key value) pair is added
//    public static bool ReplaceOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue val){
//        if (dict == null) return false;
//        bool result;
//        if (result == !dict.ContainsKey(key))
//            dict.Add(key,val);
//        return true;
//    }

    public static TValue CreateIfNotExist<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> createFunc)
        where TKey : class
        where TValue : class
    {
        if (dict == null || key == null)
        {
            return null;
        }

        if (!dict.ContainsKey(key))
        {
            if (createFunc != null)
            {
                var value = createFunc();
                dict.Add(key, value);
                return value;
            }
            else
                return null;
        }
        else if (dict[key] == null && createFunc != null)
        {
            var value = createFunc();
            dict[key] = value;
            return value;
        }
        else
            return dict[key];
    }

    public static R ShallowCopyCollection<T,R>(this  ICollection<T> dataset) where R :   ICollection<T>, new(){
        if (dataset == null) return default(R);
        R result = new R();
        foreach(var item in dataset){
            result.Add(item);
        }
        return result;
    }

    public static bool IsNullOrEmpty<T>(this List<T> dataSet)
    {
        return dataSet == null || dataSet.Count <= 0;
    }

    public static int TryGetLength<T>(this List<T> set)
    {
        return set == null ? 0 : set.Count;
    }

    public static IEnumerable<T> GetElememtsByRange<T>(
        this IEnumerable<T> dataSet
        , int begin
        , int cnt)
    {
        var len = 0;
        return dataSet.ToList().GetElememtsByRange<T>(begin, cnt, out len);
    }

    public static IEnumerable<T> GetElememtsByRange<T>(
        this List<T> dataSet
        , int begin
        , int len
        , out int length)
    {
        length = 0;
        if (dataSet.IsNullOrEmpty()
            || begin >= dataSet.Count)
            return null;

        if (begin < 0)
        {
            if (len < 0 || len >= dataSet.Count)
                return dataSet;
            else
            {
                begin = dataSet.Count - len;
                length = len;
                return dataSet.GetRange(begin, length);
            }
        }
        else
        {
            if ((begin + len) >= dataSet.Count
                || len < 0)
            {
                length = dataSet.Count - begin;
            }
            else
            {
                length = len;
            }
            return dataSet.GetRange(begin, length);
        }
    }

    public static bool TryGetValue<T>(this List<T> set, int index, out T value){
        value = default(T);
        if (!set.IsNullOrEmpty () && index >= 0 && index < set.Count) {
            value = set [index];
            return true;
        } else {
            return false;
        }
    }

    public static T TryGetValue<T>(this List<T> set, int index){
        if (!set.IsNullOrEmpty () && index < set.Count && index >= 0) {
            return set [index];
        } else {
            return default(T);;
        }
    }

    public static T TryGetValue<T>(this IEnumerable<T> set, int index)
    {
        return set.ToList().TryGetValue(index);
    }

    public static bool IsNullOrEmpty(this ArrayList array){
        return array == null || array.Count <= 0;
    }

    public static int FindElementCnt<T>(this IEnumerable<T> dataset, Predicate<T> predicate)
    {
        var cnt = 0;
        if (dataset != null && predicate != null) {
            foreach(var item in dataset){
                if (item != null && predicate(item)){
                    cnt++;
                }
            }
        }
        return cnt;
    }

    public static void AddOrReplace<K, V>(this IDictionary<K, V> dic, K key, V value)
    {
        if (dic == null)
            return;
        if (dic.ContainsKey(key))
            dic[key] = value;
        else
            dic.Add(key, value);
    }

    public static List<T> FiltAndSort<T>(IEnumerable<T> dataSet,
        Predicate<T> predicate
        , out int length
        , Comparison<T> com = null){
        length = 0;

        IEnumerable<T> temp = dataSet.Filter(predicate);
        if (temp == null)
            return null;
        else {
            var tempSet = temp.ToList();
            length = tempSet.Count;
            if (com == null) {
                return temp.ToList();
            }
            else{
                tempSet.Sort(com);
                return tempSet;
            }
        }
    }
    
    public static IEnumerable<TSource> ConcatWithNull<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
    {
        if (first == null)
            return second;
        else if (second == null)
        {
            return first;
        }
        else
        {
            return first.Concat(second);
        }
    }
}

public static class UIButtonExtension{
    public static void SetClickHandler(this UIButton btn, EventDelegate.Callback callback) {
        if (btn == null || callback == null)
            return;
        EventDelegate.Set(btn.onClick, callback);   
    }

    public static void RemoveClickHandler(this UIButton btn, EventDelegate.Callback callback) {
        if (btn == null || callback == null)
            return;
        EventDelegate.Remove(btn.onClick, callback);    
    }
}

public static class UIWidgetContainerExtension{

    /// <summary>
    /// Updates the cells.
    /// </summary>
    /// <param name="grid">Grid.</param>
    /// <param name="cellSet">Cell set.</param>
    /// <param name="cellCnt">Cell count.</param>
    /// <param name="cellName">Cell name.</param>
    /// <param name="updateCell">Update cell.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    //  public static void UpdateCells<T>(this UIGrid grid
    //      , ref List<T> cellSet
    //      , int dataLength
    //      , string cellName
    //      , Action<T,int> updateCell) where T: BaseCellView, new() 
    //  {
    //      if (grid == null)
    //          return;
    //      
    //      int i = 0;
    //
    //      while(i < dataLength){
    //          if (i >= cellSet.Count) {
    //              var _cell = BaseCellView.CreateAndSpawn<T>(cellName, grid.gameObject);
    //              cellSet.Add (_cell);
    //          }
    //          var cell = cellSet [i];
    //          cellSet [i].gameObject.SetActive (true);
    //          if (updateCell != null)
    //              updateCell (cell, i);
    //          i++;
    //      }
    //
    //      var cellCnt = grid.transform.childCount;
    //      while (i < cellCnt) {
    //          grid.transform.GetChild (i).gameObject.SetActive(false);
    //      }
    //
    //      grid.Reposition ();
    //  }

    public static void UpdateCellsWithFixGO(this UIGrid grid
        , int dataLength
        , Action<GameObject, int> updateCell)
    {
        if (grid == null)
            return;
        int i = 0;
        var cellCnt = grid.transform.childCount;

        while (i < cellCnt) {
            bool isOverRange = i < dataLength;
            var go = grid.transform.GetChild (i).gameObject;
            go.SetActive(isOverRange);  
            if (isOverRange) {
                updateCell (go, i);
            }
        }
        grid.Reposition ();
    }
}

public static class EnumParserHelper{

    public static T TryParse<T>(
        string value
        , T defaultVal = default(T)
        , bool needLogEx = true ) where T : struct{
        if (string.IsNullOrEmpty(value))
            return defaultVal;

        T result = defaultVal;
        try {
            result = (T)Enum.Parse(typeof(T),value,true);
        } catch (Exception ex) {
            if (needLogEx)
                GameDebuger.LogException (ex);
        }
        return result;
    }

    public static T? TryParseOptional<T>(string value) where T : struct{
        T? result = null;
        try {
            result = (T)Enum.Parse(typeof(T),value);
        } catch (Exception ex) {
            GameDebuger.LogException (ex);
        }
        return result;
    }
}

public interface IntEnum<T> where T : struct {
    T[] getEnums();
    int getIndex(T e);
}

internal class IntEnumHelper{
    public static T[] getEnums<T>() where T : struct{
        System.Array enums = Enum.GetValues(typeof(T));
        T[] values = new T[enums.Length];
        for(int i = 0; i< enums.Length; i++){
            values[i] = (T)enums.GetValue(i);
        }
        return values;
    }

    public static E Parse<E>(string nm,bool ignoreCase=false) where E : struct{
        Type ty = typeof(E);
        try{
            return (E)Enum.Parse(ty,nm,ignoreCase);
        }catch (Exception e){
            GameDebuger.LogWarning(string.Format("can not parse enum:{0} for type<{1}>",nm,ty));
            throw e;
        }
    }

    public static bool tryParse<E>(string nm,bool ignoreCase,ref E enm) where E:struct{
        Type ty = typeof(E);
        try{
            enm = (E)Enum.Parse(ty,nm,ignoreCase);
            return true;
        }catch{
            GameDebuger.LogWarning(string.Format("can not parse enum:{0} for type<{1}>",nm,ty));
            return false;
        }
    }
    public static int getIndex(Type enumType, object enumObjWrapper){
        string nm = Enum.GetName(enumType,enumObjWrapper);
        string[] names = Enum.GetNames(enumType);
        for(int i = 0; i< names.Length; i++){
            if (nm.Equals(names[i])){
                return i;
            }
        }
        GameDebuger.LogWarning(string.Format("bug, not all enumeration is collected {0}",enumType));
        return -1;
    }
}

//public sealed class DefaultIntEnum<E> : IntEnum<E> where E : struct {
//    public readonly static DefaultIntEnum<E> instance = new DefaultIntEnum<E>();
//    static private E[] values;
//    static DefaultIntEnum(){
//        values = IntEnumHelper.getEnums<E>();
//    }
//    public E[] getEnums(){
//        return values;
//    }
//    public int getIndex(E e){
//        for (int i = 0 ; i<values.Length;i++){
//            if (values[i].Equals(e)){
//                return i;
//            }
//        }
//        GameDebuger.LogWarning(string.Format("bug in IntEnumHelper.getEnums<{0}>, not all enumeration is collected",typeof(E)));
//        return -1;
//    }
//}

public static class DelegateExtension{
    public static Action DoOnceNull (this Action act)
    {
        GameUtil.SafeRun (act);
        return null;
    }
}