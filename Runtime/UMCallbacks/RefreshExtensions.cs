namespace UM.Runtime.UMCallbacks
{
    public static class RefreshExtensions
    {
#if UNITY_EDITOR
        public static event System.Action<object> OnRefreshRequest;
#endif
        public static void Refresh(this object obj)
        {
#if UNITY_EDITOR
            OnOnRefreshRequest(obj);
#endif
        }

#if UNITY_EDITOR
        private static void OnOnRefreshRequest(object obj)
        {
            OnRefreshRequest?.Invoke(obj);
        }
#endif
    }
}