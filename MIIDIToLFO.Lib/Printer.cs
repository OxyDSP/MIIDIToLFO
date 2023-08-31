namespace MIIDIToLFO.Lib
{
    // Global callback for the conversion lib to print to, makes it more modular.
    public static class Printer
    {
        private static Action<string>? OnPrint;

        public static void SetOnPrint(Action<string> _OnPrint)
        {
            OnPrint = _OnPrint;
        }
        public static void Print(string message)
        {
            OnPrint?.Invoke(message);
        }
    }
}