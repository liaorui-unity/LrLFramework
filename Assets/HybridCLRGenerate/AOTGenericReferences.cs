using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"JsonUI.dll",
		"LrFramework.dll",
		"System.Core.dll",
		"System.dll",
		"UniRx.dll",
		"Unity.Addressables.dll",
		"Unity.ResourceManager.dll",
		"UnityEngine.CoreModule.dll",
		"UnityEngine.JSONSerializeModule.dll",
		"UnityEngine.UIElementsModule.dll",
		"mscorlib.dll",
		"protobuf-net.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// DelegateList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>>
	// DelegateList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
	// DelegateList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// DelegateList<float>
	// Singleton<object>
	// System.Action<Mirror.QueuedMessage>
	// System.Action<System.ArraySegment<byte>,byte>
	// System.Action<System.ArraySegment<byte>,int>
	// System.Action<System.ArraySegment<byte>>
	// System.Action<UniRx.Unit>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,object>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Action<UnityEngine.UIElements.RuleMatcher>
	// System.Action<byte,object>
	// System.Action<float>
	// System.Action<int,System.ArraySegment<byte>,byte>
	// System.Action<int,System.ArraySegment<byte>,int>
	// System.Action<int,System.ArraySegment<byte>>
	// System.Action<int,byte,object>
	// System.Action<int,object>
	// System.Action<int>
	// System.Action<long>
	// System.Action<object,System.ArraySegment<byte>>
	// System.Action<object,UnityEngine.Color32>
	// System.Action<object,object>
	// System.Action<object>
	// System.Action<ushort>
	// System.ArraySegment.Enumerator<byte>
	// System.ArraySegment.Enumerator<object>
	// System.ArraySegment<byte>
	// System.ArraySegment<object>
	// System.ByReference<ushort>
	// System.Collections.Generic.ArraySortHelper<Mirror.QueuedMessage>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.UIElements.RuleMatcher>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.ArraySortHelper<ushort>
	// System.Collections.Generic.Comparer<Mirror.QueuedMessage>
	// System.Collections.Generic.Comparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.Comparer<UnityEngine.UIElements.RuleMatcher>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Comparer<ushort>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,ushort>
	// System.Collections.Generic.Dictionary.Enumerator<ushort,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,ushort>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<ushort,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,ushort>
	// System.Collections.Generic.Dictionary.KeyCollection<ushort,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,ushort>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<ushort,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,ushort>
	// System.Collections.Generic.Dictionary.ValueCollection<ushort,object>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<object,ushort>
	// System.Collections.Generic.Dictionary<ushort,object>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.EqualityComparer<ushort>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.ICollection<Mirror.QueuedMessage>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,ushort>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<ushort,object>>
	// System.Collections.Generic.ICollection<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.ICollection<UnityEngine.UIElements.RuleMatcher>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.ICollection<ushort>
	// System.Collections.Generic.IComparer<Mirror.QueuedMessage>
	// System.Collections.Generic.IComparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.IComparer<UnityEngine.UIElements.RuleMatcher>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IComparer<ushort>
	// System.Collections.Generic.IEnumerable<Mirror.QueuedMessage>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,ushort>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ushort,object>>
	// System.Collections.Generic.IEnumerable<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.IEnumerable<UnityEngine.UIElements.RuleMatcher>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<long>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerable<ushort>
	// System.Collections.Generic.IEnumerator<Mirror.QueuedMessage>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,ushort>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<ushort,object>>
	// System.Collections.Generic.IEnumerator<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.IEnumerator<UnityEngine.UIElements.RuleMatcher>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<long>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEnumerator<ushort>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IEqualityComparer<ushort>
	// System.Collections.Generic.IList<Mirror.QueuedMessage>
	// System.Collections.Generic.IList<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.IList<UnityEngine.UIElements.RuleMatcher>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.IList<ushort>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.KeyValuePair<object,ushort>
	// System.Collections.Generic.KeyValuePair<ushort,object>
	// System.Collections.Generic.LinkedList.Enumerator<object>
	// System.Collections.Generic.LinkedList<object>
	// System.Collections.Generic.LinkedListNode<object>
	// System.Collections.Generic.List.Enumerator<Mirror.QueuedMessage>
	// System.Collections.Generic.List.Enumerator<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.List.Enumerator<UnityEngine.UIElements.RuleMatcher>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List.Enumerator<ushort>
	// System.Collections.Generic.List<Mirror.QueuedMessage>
	// System.Collections.Generic.List<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.List<UnityEngine.UIElements.RuleMatcher>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.List<ushort>
	// System.Collections.Generic.ObjectComparer<Mirror.QueuedMessage>
	// System.Collections.Generic.ObjectComparer<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.ObjectComparer<UnityEngine.UIElements.RuleMatcher>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectComparer<ushort>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<ushort>
	// System.Collections.Generic.Queue.Enumerator<Mirror.NetworkMsgClient.MsgInfo>
	// System.Collections.Generic.Queue.Enumerator<Mirror.NetworkMsgServer.MsgInfo>
	// System.Collections.Generic.Queue<Mirror.NetworkMsgClient.MsgInfo>
	// System.Collections.Generic.Queue<Mirror.NetworkMsgServer.MsgInfo>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<Mirror.QueuedMessage>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.UIElements.RuleMatcher>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<ushort>
	// System.Comparison<Mirror.QueuedMessage>
	// System.Comparison<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Comparison<UnityEngine.UIElements.RuleMatcher>
	// System.Comparison<int>
	// System.Comparison<object>
	// System.Comparison<ushort>
	// System.Func<System.Threading.Tasks.VoidTaskResult>
	// System.Func<UniRx.Unit>
	// System.Func<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>
	// System.Func<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Func<byte>
	// System.Func<int,byte>
	// System.Func<long,byte>
	// System.Func<object,System.Threading.Tasks.VoidTaskResult>
	// System.Func<object,UniRx.Unit>
	// System.Func<object,UnityEngine.Color32>
	// System.Func<object,UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Func<object,byte>
	// System.Func<object,int>
	// System.Func<object,long>
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Func<object>
	// System.IObservable<UniRx.Unit>
	// System.IObservable<long>
	// System.IObservable<object>
	// System.IObserver<object>
	// System.Linq.Enumerable.Iterator<int>
	// System.Linq.Enumerable.Iterator<long>
	// System.Linq.Enumerable.Iterator<object>
	// System.Linq.Enumerable.WhereArrayIterator<object>
	// System.Linq.Enumerable.WhereEnumerableIterator<int>
	// System.Linq.Enumerable.WhereEnumerableIterator<long>
	// System.Linq.Enumerable.WhereEnumerableIterator<object>
	// System.Linq.Enumerable.WhereListIterator<object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,int>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,long>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,int>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,long>
	// System.Linq.Enumerable.WhereSelectListIterator<object,int>
	// System.Linq.Enumerable.WhereSelectListIterator<object,long>
	// System.Nullable<System.Decimal>
	// System.Nullable<System.Guid>
	// System.Nullable<UnityEngine.Color32>
	// System.Nullable<UnityEngine.Color>
	// System.Nullable<UnityEngine.Matrix4x4>
	// System.Nullable<UnityEngine.Plane>
	// System.Nullable<UnityEngine.Quaternion>
	// System.Nullable<UnityEngine.Ray>
	// System.Nullable<UnityEngine.Rect>
	// System.Nullable<UnityEngine.Vector2>
	// System.Nullable<UnityEngine.Vector2Int>
	// System.Nullable<UnityEngine.Vector3>
	// System.Nullable<UnityEngine.Vector3Int>
	// System.Nullable<UnityEngine.Vector4>
	// System.Nullable<byte>
	// System.Nullable<double>
	// System.Nullable<float>
	// System.Nullable<int>
	// System.Nullable<long>
	// System.Nullable<object>
	// System.Nullable<sbyte>
	// System.Nullable<short>
	// System.Nullable<uint>
	// System.Nullable<ulong>
	// System.Nullable<ushort>
	// System.Predicate<Mirror.QueuedMessage>
	// System.Predicate<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Predicate<UnityEngine.UIElements.RuleMatcher>
	// System.Predicate<int>
	// System.Predicate<object>
	// System.Predicate<ushort>
	// System.ReadOnlySpan<ushort>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<byte>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<UniRx.Unit>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<byte>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<UniRx.Unit>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<byte>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
	// System.Runtime.CompilerServices.TaskAwaiter<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.TaskAwaiter<UniRx.Unit>
	// System.Runtime.CompilerServices.TaskAwaiter<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Runtime.CompilerServices.TaskAwaiter<byte>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Span<ushort>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<UniRx.Unit>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<byte>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<object>
	// System.Threading.Tasks.Task<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.Task<UniRx.Unit>
	// System.Threading.Tasks.Task<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Threading.Tasks.Task<byte>
	// System.Threading.Tasks.Task<object>
	// System.Threading.Tasks.TaskCompletionSource<UniRx.Unit>
	// System.Threading.Tasks.TaskCompletionSource<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Threading.Tasks.TaskCompletionSource<object>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<UniRx.Unit>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<byte>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<object>
	// System.Threading.Tasks.TaskFactory<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.TaskFactory<UniRx.Unit>
	// System.Threading.Tasks.TaskFactory<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// System.Threading.Tasks.TaskFactory<byte>
	// System.Threading.Tasks.TaskFactory<object>
	// Table.Row<int>
	// Table.Table<int,object>
	// UniRx.InternalUtil.ImmutableList<object>
	// UniRx.InternalUtil.ListObserver<object>
	// UniRx.Observer.Subscribe<UniRx.Unit>
	// UniRx.Observer.Subscribe<long>
	// UniRx.Observer.Subscribe<object>
	// UniRx.Observer.Subscribe_<UniRx.Unit>
	// UniRx.Observer.Subscribe_<long>
	// UniRx.Observer.Subscribe_<object>
	// UniRx.Subject.Subscription<object>
	// UniRx.Subject<object>
	// UnirxAwaitExtensions.<>c__DisplayClass0_0<UniRx.Unit>
	// UnirxAwaitExtensions.<>c__DisplayClass0_0<object>
	// UnityEngine.AddressableAssets.AddressablesImpl.<>c__DisplayClass88_0<object>
	// UnityEngine.AddressableAssets.AddressablesImpl.<>c__DisplayClass91_0<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass60_0<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass60_0<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass61_0<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase.<>c__DisplayClass61_0<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<object>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>
	// UnityEngine.ResourceManagement.ChainOperationTypelessDepedency<object>
	// UnityEngine.ResourceManagement.ResourceManager.<>c__DisplayClass95_0<object>
	// UnityEngine.ResourceManagement.ResourceManager.CompletedOperation<object>
	// UnityEngine.ResourceManagement.Util.GlobalLinkedListNodeCache<object>
	// UnityEngine.ResourceManagement.Util.LinkedListNodeCache<object>
	// UnityEngine.UIElements.EventBase.<>c<object>
	// UnityEngine.UIElements.EventBase<object>
	// UnityEngine.UIElements.EventCallback<object>
	// UnityEngine.UIElements.EventCallbackFunctor<object>
	// UnityEngine.UIElements.ObjectPool.<>c<object>
	// UnityEngine.UIElements.ObjectPool<object>
	// UnityEngine.UIElements.PointerEventBase<object>
	// UnityEngine.UIElements.StyleEnum<int>
	// UnityEngine.UIElements.UQueryState.ActionQueryMatcher<object>
	// UnityEngine.UIElements.UQueryState.Enumerator<object>
	// UnityEngine.UIElements.UQueryState.ListQueryMatcher<object,object>
	// UnityEngine.UIElements.UQueryState<object>
	// }}

	public void RefMethods()
	{
		// System.Void ChangeEventController.InitChangeEvent<object>(object)
		// System.Void Extend.AddCheckEvent<object>(object)
		// bool Extend.CheckValue<object,int>(object,int,ushort)
		// object ProtoBuf.Serializer.Deserialize<object>(System.IO.Stream)
		// object Streamable.Get<object>()
		// System.Void Streamable.Register<object>()
		// System.Void System.Array.Resize<byte>(byte[]&,int)
		// bool System.Linq.Enumerable.Contains<object>(System.Collections.Generic.IEnumerable<object>,object)
		// bool System.Linq.Enumerable.Contains<object>(System.Collections.Generic.IEnumerable<object>,object,System.Collections.Generic.IEqualityComparer<object>)
		// object System.Linq.Enumerable.First<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Select<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Collections.Generic.IEnumerable<long> System.Linq.Enumerable.Select<object,long>(System.Collections.Generic.IEnumerable<object>,System.Func<object,long>)
		// int System.Linq.Enumerable.Sum<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// long System.Linq.Enumerable.Sum<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,long>)
		// System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Where<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Iterator<object>.Select<int>(System.Func<object,int>)
		// System.Collections.Generic.IEnumerable<long> System.Linq.Enumerable.Iterator<object>.Select<long>(System.Func<object,long>)
		// object System.Reflection.CustomAttributeExtensions.GetCustomAttribute<object>(System.Reflection.MemberInfo)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,FlowGame.<Start>d__3>(System.Runtime.CompilerServices.TaskAwaiter&,FlowGame.<Start>d__3&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,LinkerFlow.<Logic>d__4>(System.Runtime.CompilerServices.TaskAwaiter&,LinkerFlow.<Logic>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,SceneFlow.<Logic>d__4>(System.Runtime.CompilerServices.TaskAwaiter&,SceneFlow.<Logic>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,StreamableFlow.<Logic>d__4>(System.Runtime.CompilerServices.TaskAwaiter&,StreamableFlow.<Logic>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<UniRx.Unit>,SceneFlow.<Logic>d__4>(System.Runtime.CompilerServices.TaskAwaiter<UniRx.Unit>&,SceneFlow.<Logic>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,HotfixProgram.<DownLoadInstantiateType>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,HotfixProgram.<DownLoadInstantiateType>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,PreGoFlow.<Logic>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,PreGoFlow.<Logic>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,FlowGame.<Start>d__3>(System.Runtime.CompilerServices.TaskAwaiter&,FlowGame.<Start>d__3&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,LinkerFlow.<Logic>d__4>(System.Runtime.CompilerServices.TaskAwaiter&,LinkerFlow.<Logic>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,SceneFlow.<Logic>d__4>(System.Runtime.CompilerServices.TaskAwaiter&,SceneFlow.<Logic>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,StreamableFlow.<Logic>d__4>(System.Runtime.CompilerServices.TaskAwaiter&,StreamableFlow.<Logic>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<UniRx.Unit>,SceneFlow.<Logic>d__4>(System.Runtime.CompilerServices.TaskAwaiter<UniRx.Unit>&,SceneFlow.<Logic>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,HotfixProgram.<DownLoadInstantiateType>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,HotfixProgram.<DownLoadInstantiateType>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,PreGoFlow.<Logic>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,PreGoFlow.<Logic>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Mirror.TeamClient.<Cancel>d__14>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Mirror.TeamClient.<Cancel>d__14&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Mirror.TeamClient.<Creat>d__13>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Mirror.TeamClient.<Creat>d__13&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Mirror.TeamClient.<Exit>d__16>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Mirror.TeamClient.<Exit>d__16&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Mirror.TeamClient.<Join>d__15>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Mirror.TeamClient.<Join>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Mirror.TeamClient.<UpdateTeam>d__19>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Mirror.TeamClient.<UpdateTeam>d__19&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,HotfixProgram.<DownLoadType>d__2<object>>(System.Runtime.CompilerServices.TaskAwaiter<object>&,HotfixProgram.<DownLoadType>d__2<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,HotfixProgram.<DownLoadType>d__3<object>>(System.Runtime.CompilerServices.TaskAwaiter<object>&,HotfixProgram.<DownLoadType>d__3<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Mirror.TeamClient.<WaitServerRespond>d__11<object>>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Mirror.TeamClient.<WaitServerRespond>d__11<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<FlowGame.<Start>d__3>(FlowGame.<Start>d__3&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<HotfixProgram.<DownLoadInstantiateType>d__4>(HotfixProgram.<DownLoadInstantiateType>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<LinkerFlow.<Logic>d__4>(LinkerFlow.<Logic>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<PreGoFlow.<Logic>d__4>(PreGoFlow.<Logic>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<SceneFlow.<Logic>d__4>(SceneFlow.<Logic>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<StreamableFlow.<Logic>d__4>(StreamableFlow.<Logic>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<byte>.Start<Mirror.TeamClient.<Cancel>d__14>(Mirror.TeamClient.<Cancel>d__14&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<byte>.Start<Mirror.TeamClient.<Creat>d__13>(Mirror.TeamClient.<Creat>d__13&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<byte>.Start<Mirror.TeamClient.<Exit>d__16>(Mirror.TeamClient.<Exit>d__16&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<byte>.Start<Mirror.TeamClient.<Join>d__15>(Mirror.TeamClient.<Join>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<byte>.Start<Mirror.TeamClient.<UpdateTeam>d__19>(Mirror.TeamClient.<UpdateTeam>d__19&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<HotfixProgram.<DownLoadType>d__2<object>>(HotfixProgram.<DownLoadType>d__2<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<HotfixProgram.<DownLoadType>d__3<object>>(HotfixProgram.<DownLoadType>d__3<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<Mirror.TeamClient.<WaitServerRespond>d__11<object>>(Mirror.TeamClient.<WaitServerRespond>d__11<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,HotfixMain.<Flow>d__2>(System.Runtime.CompilerServices.TaskAwaiter&,HotfixMain.<Flow>d__2&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<byte>,Mirror.TeamClient.<MatchTeam>d__20>(System.Runtime.CompilerServices.TaskAwaiter<byte>&,Mirror.TeamClient.<MatchTeam>d__20&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<HotfixMain.<Flow>d__2>(HotfixMain.<Flow>d__2&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Mirror.TeamClient.<MatchTeam>d__20>(Mirror.TeamClient.<MatchTeam>d__20&)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// Table.DataSrc<int,object> Table.TableManager.Load<int,object>(byte[])
		// System.Void Table.TableManager.LoadMethod<int,object>(System.Type)
		// Table.Table<int,object> Table.TableManager.LoadTable<int,object>(byte[])
		// System.IDisposable UniRx.ObservableExtensions.Subscribe<UniRx.Unit>(System.IObservable<UniRx.Unit>,System.Action<UniRx.Unit>,System.Action<System.Exception>)
		// System.IDisposable UniRx.ObservableExtensions.Subscribe<long>(System.IObservable<long>,System.Action<long>)
		// System.IDisposable UniRx.ObservableExtensions.Subscribe<object>(System.IObservable<object>,System.Action<object>,System.Action<System.Exception>)
		// System.IObserver<UniRx.Unit> UniRx.Observer.CreateSubscribeObserver<UniRx.Unit>(System.Action<UniRx.Unit>,System.Action<System.Exception>,System.Action)
		// System.IObserver<long> UniRx.Observer.CreateSubscribeObserver<long>(System.Action<long>,System.Action<System.Exception>,System.Action)
		// System.IObserver<object> UniRx.Observer.CreateSubscribeObserver<object>(System.Action<object>,System.Action<System.Exception>,System.Action)
		// System.Runtime.CompilerServices.TaskAwaiter<UniRx.Unit> UnirxAwaitExtensions.GetAwaiter<UniRx.Unit>(System.IObservable<UniRx.Unit>)
		// System.Runtime.CompilerServices.TaskAwaiter<object> UnirxAwaitExtensions.GetAwaiter<object>(System.IObservable<object>)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.IList<object>> UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync<object>(object,System.Action<object>)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.IList<object>> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetsAsync<object>(System.Collections.Generic.IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation>,System.Action<object>,bool)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.IList<object>> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetsAsync<object>(object,System.Action<object>,bool)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.IList<object>> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetsWithChain<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,System.Collections.Generic.IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation>,System.Action<object>,bool)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.IList<object>> UnityEngine.AddressableAssets.AddressablesImpl.LoadAssetsWithChain<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,object,System.Action<object>,bool)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.AddressablesImpl.TrackHandle<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>)
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.JsonUtility.FromJson<object>(string)
		// object UnityEngine.Object.Instantiate<object>(object)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateChainOperation<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,System.Func<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateChainOperation<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,System.Func<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>,bool)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateCompletedOperation<object>(object,string)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateCompletedOperationInternal<object>(object,bool,System.Exception,bool)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.CreateCompletedOperationWithException<object>(object,System.Exception)
		// object UnityEngine.ResourceManagement.ResourceManager.CreateOperation<object>(System.Type,int,UnityEngine.ResourceManagement.Util.IOperationCacheKey,System.Action<UnityEngine.ResourceManagement.AsyncOperations.IAsyncOperation>)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.IList<object>> UnityEngine.ResourceManagement.ResourceManager.ProvideResources<object>(System.Collections.Generic.IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation>,bool,System.Action<object>)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.ResourceManagement.ResourceManager.StartOperation<object>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase<object>,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle)
		// System.Void UnityEngine.UIElements.CallbackEventHandler.AddEventCategories<object>()
		// System.Void UnityEngine.UIElements.CallbackEventHandler.RegisterCallback<object>(UnityEngine.UIElements.EventCallback<object>,UnityEngine.UIElements.TrickleDown)
		// System.Void UnityEngine.UIElements.EventCallbackRegistry.RegisterCallback<object>(UnityEngine.UIElements.EventCallback<object>,UnityEngine.UIElements.TrickleDown,UnityEngine.UIElements.InvokePolicy)
		// object UnityEngine.UIElements.UQueryExtensions.Q<object>(UnityEngine.UIElements.VisualElement,string,string)
	}
}