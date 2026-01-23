namespace Services.UI
{
    public interface IDeckEscapable
    {
        bool HasEscaped { get; }
        bool CanBeEscapedWithRightClick { get; }
        void OnEscapeRequested();
        
    }
}