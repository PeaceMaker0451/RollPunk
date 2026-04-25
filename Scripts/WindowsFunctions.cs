using System;
using System.Runtime.InteropServices;

namespace RollPunk
{
    internal class WindowFunctions
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        const int SW_MINIMIZE = 6;
        const int SW_RESTORE = 9;

        public static void MinimizeWindow()
        {
            IntPtr hWnd = GetForegroundWindow(); // Получаем хендл текущего окна
            ShowWindow(hWnd, SW_MINIMIZE); // Сворачиваем окно
        }

        public static void RestoreWindow()
        {
            IntPtr hWnd = GetForegroundWindow();
            ShowWindow(hWnd, SW_RESTORE); // Восстанавливаем окно
        }
    }
}
