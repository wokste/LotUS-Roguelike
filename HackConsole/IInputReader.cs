using System;

namespace HackConsole
{
    public interface IInputReader
    {
        /// <summary>
        /// Called when a key is pressed
        /// </summary>
        /// <param name="keyCode">the Ascii code of the key</param>
        /// <param name="flags">This includes at least the Shift, Ctrl and Alt modifiers</param>
        /// <returns>True if the IInputReader handled the event</returns>
        bool OnKeyPress(char keyCode, EventFlags flags);

        /// <summary>
        /// Called when an arrow key is pressed
        /// </summary>
        /// <param name="move">The movement provided by the arrow keys.</param>
        /// <param name="flags">This includes at least the Shift, Ctrl and Alt modifiers</param>
        /// <returns>True if the IInputReader handled the event</returns>
        bool OnArrowPress(Vec move, EventFlags flags);

        /// <summary>
        /// Called when the mouse is clicked or released. 
        /// </summary>
        /// <param name="mousePos">The position of the mouse, in console coordinates.</param>
        /// <param name="flags">This includes at least whether it was pressed or released and the mouse button.</param>
        /// <returns>True if the IInputReader handled the event</returns>
        bool OnMouseEvent(Vec mousePos, EventFlags flags);

        /// <summary>
        /// Called when the mouse is moved. 
        /// </summary>
        /// <param name="mousePos">The position of the mouse, at the end, in console coordinates.</param>
        /// <param name="mouseMove">The relative position of the mouse, in console coordinates.</param>
        /// <param name="flags">This may include special keys and mouse buttons that are pressed.</param>
        /// <returns>True if the IInputReader handled the event</returns>
        bool OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta">The direction of the scroll movement. Normal scroll wheels only have a non-zero Y coordinate.</param>
        /// <param name="flags">This may include special keys and mouse buttons that are pressed.</param>
        /// <returns>True if the IInputReader handled the event</returns>
        bool OnMouseWheel(Vec delta, EventFlags flags);
    }
    
    [Flags]
    public enum EventFlags
    {
        None = 0,
        Shift = 0x1,
        Ctrl = 0x2,
        Alt = 0x4,

        LeftButton = 0x10,
        MidButton = 0x20,
        RightButton = 0x40,

        MouseEventPress = 0x100,
        MouseEventRelease = 0x100,
    }
}
