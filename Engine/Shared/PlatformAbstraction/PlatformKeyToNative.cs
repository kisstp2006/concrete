namespace Concrete;

public static class PlatformKeyToNative
{
    public static int MapToSDLKey(PlatformKey key)
    {
        return key switch
        {
            // Letters (A-Z)
            PlatformKey.A => 4,
            PlatformKey.B => 5,
            PlatformKey.C => 6,
            PlatformKey.D => 7,
            PlatformKey.E => 8,
            PlatformKey.F => 9,
            PlatformKey.G => 10,
            PlatformKey.H => 11,
            PlatformKey.I => 12,
            PlatformKey.J => 13,
            PlatformKey.K => 14,
            PlatformKey.L => 15,
            PlatformKey.M => 16,
            PlatformKey.N => 17,
            PlatformKey.O => 18,
            PlatformKey.P => 19,
            PlatformKey.Q => 20,
            PlatformKey.R => 21,
            PlatformKey.S => 22,
            PlatformKey.T => 23,
            PlatformKey.U => 24,
            PlatformKey.V => 25,
            PlatformKey.W => 26,
            PlatformKey.X => 27,
            PlatformKey.Y => 28,
            PlatformKey.Z => 29,
            
            // Numbers (1-0)
            PlatformKey.Number1 => 30,
            PlatformKey.Number2 => 31,
            PlatformKey.Number3 => 32,
            PlatformKey.Number4 => 33,
            PlatformKey.Number5 => 34,
            PlatformKey.Number6 => 35,
            PlatformKey.Number7 => 36,
            PlatformKey.Number8 => 37,
            PlatformKey.Number9 => 38,
            PlatformKey.Number0 => 39,
            
            // Special characters
            PlatformKey.Enter => 40,
            PlatformKey.Escape => 41,
            PlatformKey.Backspace => 42,
            PlatformKey.Tab => 43,
            PlatformKey.Space => 44,
            PlatformKey.Minus => 45,
            PlatformKey.Equal => 46,
            PlatformKey.LeftBracket => 47,
            PlatformKey.RightBracket => 48,
            PlatformKey.BackSlash => 49,
            PlatformKey.Semicolon => 51,
            PlatformKey.Apostrophe => 52,
            PlatformKey.GraveAccent => 53,
            PlatformKey.Comma => 54,
            PlatformKey.Period => 55,
            PlatformKey.Slash => 56,
            
            // Function keys
            PlatformKey.CapsLock => 57,
            PlatformKey.F1 => 58,
            PlatformKey.F2 => 59,
            PlatformKey.F3 => 60,
            PlatformKey.F4 => 61,
            PlatformKey.F5 => 62,
            PlatformKey.F6 => 63,
            PlatformKey.F7 => 64,
            PlatformKey.F8 => 65,
            PlatformKey.F9 => 66,
            PlatformKey.F10 => 67,
            PlatformKey.F11 => 68,
            PlatformKey.F12 => 69,
            
            PlatformKey.PrintScreen => 70,
            PlatformKey.ScrollLock => 71,
            PlatformKey.Pause => 72,
            PlatformKey.Insert => 73,
            PlatformKey.Home => 74,
            PlatformKey.PageUp => 75,
            PlatformKey.Delete => 76,
            PlatformKey.End => 77,
            PlatformKey.PageDown => 78,
            
            // Arrow keys
            PlatformKey.Right => 79,
            PlatformKey.Left => 80,
            PlatformKey.Down => 81,
            PlatformKey.Up => 82,
            
            // Keypad
            PlatformKey.NumLock => 83,
            PlatformKey.KeypadDivide => 84,
            PlatformKey.KeypadMultiply => 85,
            PlatformKey.KeypadSubtract => 86,
            PlatformKey.KeypadAdd => 87,
            PlatformKey.KeypadEnter => 88,
            PlatformKey.Keypad1 => 89,
            PlatformKey.Keypad2 => 90,
            PlatformKey.Keypad3 => 91,
            PlatformKey.Keypad4 => 92,
            PlatformKey.Keypad5 => 93,
            PlatformKey.Keypad6 => 94,
            PlatformKey.Keypad7 => 95,
            PlatformKey.Keypad8 => 96,
            PlatformKey.Keypad9 => 97,
            PlatformKey.Keypad0 => 98,
            PlatformKey.KeypadDecimal => 99,
            PlatformKey.KeypadEqual => 103,
            
            // F13-F24
            PlatformKey.F13 => 104,
            PlatformKey.F14 => 105,
            PlatformKey.F15 => 106,
            PlatformKey.F16 => 107,
            PlatformKey.F17 => 108,
            PlatformKey.F18 => 109,
            PlatformKey.F19 => 110,
            PlatformKey.F20 => 111,
            PlatformKey.F21 => 112,
            PlatformKey.F22 => 113,
            PlatformKey.F23 => 114,
            PlatformKey.F24 => 115,
            
            // Modifier keys
            PlatformKey.ControlLeft => 224,
            PlatformKey.ShiftLeft => 225,
            PlatformKey.AltLeft => 226,
            PlatformKey.SuperLeft => 227,
            PlatformKey.ControlRight => 228,
            PlatformKey.ShiftRight => 229,
            PlatformKey.AltRight => 230,
            PlatformKey.SuperRight => 231,
            
            PlatformKey.Menu => 118,
            
            _ => -1
        };
    }
}