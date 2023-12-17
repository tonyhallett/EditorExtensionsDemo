namespace EditorExtensionsDemo
{
    interface IInvokeWithDelay
    {
        void DelayedInvoke(int delay, Action action);
    }
}
