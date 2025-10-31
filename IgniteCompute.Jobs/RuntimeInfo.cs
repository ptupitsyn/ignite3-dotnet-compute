using System.Runtime.InteropServices;

namespace IgniteCompute.Jobs;

public record RuntimeInfo(
    OperatingSystem Os,
    Architecture OsArch,
    Architecture CpuArсh,
    string MachineName,
    string FrameworkDescription);
