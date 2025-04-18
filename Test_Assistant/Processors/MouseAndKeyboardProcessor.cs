﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Test_Assistant.Enums;
using Test_Assistant.Models;
using Test_Assistant.pagesModels;

namespace Test_Assistant.Processors
{
    public class MouseAndKeyboardProcessor
    {
        public static Form1 _instanceForm1;
        public static System.Windows.Forms.Timer _globalTimer;

        private static FileData _fileData;
        private static FileDataProcessor _fileDataProcessor;
        private static string debugFile = ".\\lastTestCaseFile.txt";// Debug file location
        private static bool isRecordingClicks = false;
        private static int elapsedPartialSeconds = 0;

        /// <HOOK_IMPORTS>

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        ///</HOOK_IMPORTS>

        /// <KEYBOARD_EVENT_VARIABLES>


        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int VK_RETURN = 0x0D; // Virtual key code for Enter
        private const int VK_R = 0x52; // Virtual key code for R
        private const int VK_P = 0x50; // Virtual key code for P

        /// </KEYBOARD_EVENT_VARIABLES>

        /// <MOUSE_EVENT_VARIABLES>

        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;

        /// </MOUSE_EVENT_VARIABLES>

        /// <MOUSE_AND_KEYBOARD_HOOK_VARIABLES>

        private static IntPtr _keyboardHookID = IntPtr.Zero;
        private static LowLevelMouseProc _mouseProc = MouseHookCallback;
        private static IntPtr _mouseHookID = IntPtr.Zero;
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        /// </MOUSE_AND_KEYBOARD_HOOK_VARIABLES>

        /// <HOOK_LOGISTIC>
        private static IntPtr SetMouseHook(LowLevelMouseProc proc)
        {
            using (var currentProcess = Process.GetCurrentProcess())
            using (var currentModule = currentProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(currentModule.ModuleName), 0);
            }
        }
        private static IntPtr SetKeyboardHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (isRecordingClicks)
            {
                if (nCode >= 0 && (wParam == (IntPtr)WM_LBUTTONDOWN || wParam == (IntPtr)WM_RBUTTONDOWN))
                {
                    MSLLHOOKSTRUCT mouseStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                    if (mouseStruct.mouseData != null)
                    {
                        File.AppendAllText(debugFile, $"X: {mouseStruct.pt.x}, Y: {mouseStruct.pt.y}, Button: {(wParam == (IntPtr)WM_LBUTTONDOWN ? "Left" : "Right")}" + Environment.NewLine);
                        File.AppendAllText(debugFile, $"{mouseStruct.pt.x},{mouseStruct.pt.y},{elapsedPartialSeconds}\n");

                        _fileDataProcessor.AddNewActionToLastTestCaseFile(new TestCaseAction
                        {
                            x = mouseStruct.pt.x,
                            y = mouseStruct.pt.y,
                            t = elapsedPartialSeconds<1?1: elapsedPartialSeconds,
                        });

                        elapsedPartialSeconds = 0;
                    }
                }
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }
        private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (isRecordingClicks)
            {
                if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
                {
                    var key = Marshal.ReadInt32(lParam);
                    if (key  == VK_RETURN) // Enter key
                    {
                        isRecordingClicks = false;
                        _instanceForm1.WindowState = FormWindowState.Normal;
                        _globalTimer.Enabled = false;
                        elapsedPartialSeconds = 0;

                        //TODO: Make this a promise to casePege to to saving

                        if (_fileData != null)
                        {
                            var testCase = _fileDataProcessor.GetLastTestCase();
                            testCase.id = _fileData.Testcases.Any() ? _fileData.Testcases.Last().id + 1 : 0;
                            _fileData.Testcases.Add(testCase);
                        }

                    }
                    else if (key == VK_R || key == VK_P)
                    {
                        isRecordingClicks = false;
                        _globalTimer.Enabled = false;


                        var fileSpecialAction = new SpecialAction();
                        fileSpecialAction.id = _fileData.SpecialActions.Any() ? _fileData.SpecialActions.Last().id + 1 : 0;
                        if(key == VK_R)
                            fileSpecialAction.actionName = SpecialActionsEnum.Parse.ToString();
                        else if (key == VK_P)
                            fileSpecialAction.actionName = SpecialActionsEnum.Photo.ToString();
                        else
                            fileSpecialAction.actionName = SpecialActionsEnum.Pause.ToString();

                        EditSpecialActionPageForm editForm = new EditSpecialActionPageForm(fileSpecialAction);

                        if (editForm.ShowDialog() != DialogResult.OK)
                        {
                            MessageBox.Show("Entered Data were lost");
                        }

                        _fileData.SpecialActions.Add(fileSpecialAction);

                        _fileDataProcessor.AddSpecialActionToLastTestCaseFile(fileSpecialAction);
                        isRecordingClicks = true;
                        _globalTimer.Enabled = true;
                    }
                }
            }
            return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);

        }

        /// </HOOK_LOGISTIC>

        public MouseAndKeyboardProcessor(Form1 instanceForm1, FileData fileData)
        {
            _instanceForm1 = instanceForm1;
            _globalTimer = new System.Windows.Forms.Timer();
            _fileData = fileData;

            _mouseHookID = SetMouseHook(_mouseProc);
            _keyboardHookID = SetKeyboardHook(KeyboardHookCallback);
            _fileDataProcessor = new FileDataProcessor();
        }

        //public IntPtr HookMouse()
        //{
        //    _mouseHookID = SetMouseHook(_mouseProc);
        //    return _mouseHookID;
        //}        

        //public IntPtr HookKeyboard()
        //{
        //    _keyboardHookID = SetKeyboardHook(KeyboardHookCallback);
        //    return _keyboardHookID;
        //}

        public void UnhookAll()
        {
            if (_mouseHookID != IntPtr.Zero)
                UnhookWindowsHookEx(_mouseHookID);
            if (_keyboardHookID != IntPtr.Zero)
                UnhookWindowsHookEx(_keyboardHookID);
        }



        public void StartRecording()
        {
            isRecordingClicks = true;
            _instanceForm1.WindowState = FormWindowState.Minimized;
            StartPartialTimer();
            _fileDataProcessor.ClearLastTestCaseFile();
        }
        private void StartPartialTimer()
        {
            _globalTimer.Enabled = true;
            _globalTimer.Interval = 1000;
            _globalTimer.Tick += (sender, e) => { elapsedPartialSeconds++; };// Calculate elapsed seconds beetwen mouse clicks
        }

    }
}
