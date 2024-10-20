using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace RuneDocMVVM;

public static class Inject
{
    public static unsafe void MapModule(uint pid, string modulePath = ".\\DeOppressoLiber.dll")
    {
        modulePath = Directory.GetCurrentDirectory() + modulePath;
        var handle = Kernel32.OpenProcess(
            Kernel32.PROCESS_ALL_ACCESS,
            false,
            pid);

        if (handle == IntPtr.Zero)
        {
            // Shit's fucked.
            throw new System.ComponentModel.Win32Exception();
        }

        var remote = Kernel32.VirtualAllocEx(
            handle,
            IntPtr.Zero,
            (uint)modulePath.Length,
            Kernel32.MEM_RESERVE | Kernel32.MEM_COMMIT,
            Kernel32.PAGE_READWRITE);
        
        Kernel32.WriteProcessMemory(
            handle, 
            remote, 
            Encoding.UTF8.GetBytes(modulePath), 
            (uint)modulePath.Length, 
            // This is kinda nice.
            out _);
        
        
        var k32ModuleHandle = Kernel32.GetModuleHandleA("kernel32.dll");
        var loadLibAddress = Kernel32.GetProcAddress(k32ModuleHandle, "LoadLibraryA");
        Kernel32.CreateRemoteThread(
            handle, 
            IntPtr.Zero, 
            0,
            loadLibAddress, 
            remote, 
            0, 
            out _);
        
        Kernel32.CloseHandle(handle);
    }

    public static unsafe List<uint> GetAllClients()
    {
        List<uint> clients = new List<uint>();
        uint pid;
        PROCESSENTRY32 entry = new PROCESSENTRY32();
        entry.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));

        var snapshotHandle = ToolHelp32.CreateToolhelp32Snapshot(0x00000002, 0);

        if (!ToolHelp32.Process32First(snapshotHandle, ref entry))
        {
            Console.WriteLine(Marshal.GetLastWin32Error());
            // Shit's fucked
            throw new System.ComponentModel.Win32Exception();
        }

        while (ToolHelp32.Process32Next(snapshotHandle, ref entry))
        {
            if (entry.szExeFile == "rs2client.exe")
            {
                clients.Add(entry.th32ProcessID);
            }
        }
        return clients;
    }
}