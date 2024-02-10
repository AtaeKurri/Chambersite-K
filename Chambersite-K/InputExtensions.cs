using Microsoft.Xna.Framework.Input;

namespace Chambersite_K
{
    public static class InputExtensions
    {
        // Store the previous state of the keyboard
        private static KeyboardState previousKeyboardState;

        /// <summary>
        /// Checks if the specified key has been pressed in the current frame and was not pressed in the previous frame.
        /// </summary>
        /// <param name="keyboardState">The current keyboard state.</param>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key was pressed in the current frame and was not pressed in the previous frame, otherwise false.</returns>
        public static bool IsKeyPressedOnce(this KeyboardState keyboardState, Keys key)
        {
            return keyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Updates the previous keyboard state to the current keyboard state.
        /// This method should be called at the end of each frame.
        /// </summary>
        /// <param name="keyboardState">The current keyboard state.</param>
        public static void UpdatePreviousKeyboardState(this KeyboardState keyboardState)
        {
            previousKeyboardState = keyboardState;
        }
    }
}