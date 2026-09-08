using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace SmartformOcxPreview;

public static class OcxRegistration
{
    /// <summary>
    /// Ensures that the specified OCX file is registered.
    /// </summary>
    /// <param name="ocxPath">Full path to the OCX file.</param>
    /// <returns>
    /// True if the OCX was already registered or was successfully registered.
    /// </returns>
    public static bool EnsureRegistered(string ocxPath)
    {
        if (string.IsNullOrWhiteSpace(ocxPath))
        {
            throw new ArgumentException(
                "The OCX path cannot be null or empty.",
                nameof(ocxPath));
        }

        string fullPath = Path.GetFullPath(ocxPath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException(
                "The specified OCX file was not found.",
                fullPath);
        }

        OcxArchitecture architecture = GetArchitecture(fullPath);
        RegistryView registryView = GetRegistryView(architecture);

        if (IsRegistered(fullPath, registryView))
        {
            return true;
        }

        Register(fullPath, architecture);

        return IsRegistered(fullPath, registryView);
    }

    /// <summary>
    /// Checks whether the specified OCX file is registered.
    /// </summary>
    public static bool IsRegistered(string ocxPath)
    {
        if (string.IsNullOrWhiteSpace(ocxPath))
        {
            throw new ArgumentException(
                "The OCX path cannot be null or empty.",
                nameof(ocxPath));
        }

        string fullPath = Path.GetFullPath(ocxPath);
        OcxArchitecture architecture = GetArchitecture(fullPath);

        return IsRegistered(fullPath, GetRegistryView(architecture));
    }

    private static bool IsRegistered(
        string fullPath,
        RegistryView registryView)
    {
        using RegistryKey classesRoot = RegistryKey.OpenBaseKey(
            RegistryHive.ClassesRoot,
            registryView);

        using RegistryKey? clsidRoot = classesRoot.OpenSubKey("CLSID");

        if (clsidRoot == null)
        {
            return false;
        }

        foreach (string clsid in clsidRoot.GetSubKeyNames())
        {
            using RegistryKey? inprocServer = clsidRoot.OpenSubKey(
                $@"{clsid}\InprocServer32");

            string? registeredPath =
                inprocServer?.GetValue(null) as string;

            if (string.IsNullOrWhiteSpace(registeredPath))
            {
                continue;
            }

            registeredPath = NormalizeRegisteredPath(registeredPath);

            if (string.Equals(
                    fullPath,
                    registeredPath,
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static void Register(
        string ocxPath,
        OcxArchitecture architecture)
    {
        string regsvr32Path = GetRegsvr32Path(architecture);

        var startInfo = new ProcessStartInfo
        {
            FileName = regsvr32Path,
            Arguments = $"/s \"{ocxPath}\"",
            UseShellExecute = true,

            // Displays UAC prompt if the application is not elevated.
            Verb = "runas"
        };

        try
        {
            using Process? process = Process.Start(startInfo);

            if (process == null)
            {
                throw new InvalidOperationException(
                    "Unable to start regsvr32.exe.");
            }

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"OCX registration failed. " +
                    $"regsvr32.exe returned exit code {process.ExitCode}.");
            }
        }
        catch (Win32Exception ex)
            when (ex.NativeErrorCode == 1223)
        {
            throw new OperationCanceledException(
                "OCX registration was cancelled by the user.",
                ex);
        }
    }

    private static string GetRegsvr32Path(
        OcxArchitecture architecture)
    {
        string windowsDirectory =
            Environment.GetFolderPath(
                Environment.SpecialFolder.Windows);

        if (!Environment.Is64BitOperatingSystem)
        {
            return Path.Combine(
                windowsDirectory,
                "System32",
                "regsvr32.exe");
        }

        if (architecture == OcxArchitecture.X86)
        {
            return Path.Combine(
                windowsDirectory,
                "SysWOW64",
                "regsvr32.exe");
        }

        // A 32-bit process must use Sysnative to access
        // the real 64-bit System32 directory.
        string systemDirectory = Environment.Is64BitProcess
            ? "System32"
            : "Sysnative";

        return Path.Combine(
            windowsDirectory,
            systemDirectory,
            "regsvr32.exe");
    }

    private static RegistryView GetRegistryView(
        OcxArchitecture architecture)
    {
        if (!Environment.Is64BitOperatingSystem)
        {
            return RegistryView.Default;
        }

        return architecture == OcxArchitecture.X86
            ? RegistryView.Registry32
            : RegistryView.Registry64;
    }

    private static string NormalizeRegisteredPath(string path)
    {
        string normalizedPath = path
            .Trim()
            .Trim('"');

        normalizedPath =
            Environment.ExpandEnvironmentVariables(normalizedPath);

        try
        {
            return Path.GetFullPath(normalizedPath);
        }
        catch
        {
            return normalizedPath;
        }
    }

    private static OcxArchitecture GetArchitecture(string filePath)
    {
        using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);

        using var reader = new BinaryReader(stream);

        if (reader.ReadUInt16() != 0x5A4D)
        {
            throw new BadImageFormatException(
                "The file does not contain a valid DOS header.",
                filePath);
        }

        stream.Position = 0x3C;
        int peHeaderOffset = reader.ReadInt32();

        stream.Position = peHeaderOffset;

        if (reader.ReadUInt32() != 0x00004550)
        {
            throw new BadImageFormatException(
                "The file does not contain a valid PE header.",
                filePath);
        }

        ushort machine = reader.ReadUInt16();

        return machine switch
        {
            0x014C => OcxArchitecture.X86,
            0x8664 => OcxArchitecture.X64,

            _ => throw new NotSupportedException(
                $"Unsupported OCX architecture: 0x{machine:X4}.")
        };
    }

    private enum OcxArchitecture
    {
        X86,
        X64
    }
}