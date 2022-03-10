using Godot;

namespace PackageResolved.Logic
{
    /// <summary>
    /// An interface that indicates that the class has a teardown function that should be called when being 
    /// deinitialized from the scene tree.
    /// </summary>
    public interface ITeardownable
    {
        /// <summary>
        /// Tears down the class to either be freed from memory or be marked as ready for reinitialization.
        /// </summary>
        void Teardown();
    }
}