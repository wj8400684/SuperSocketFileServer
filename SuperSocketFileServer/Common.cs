namespace SuperSocketFileServer;

internal static class Common
{
    /// <summary>
    /// 获取相对路径 比如 D:\.net\git\SuperSocketFileServer\SuperSocketFileServer\bin\Release\net7.0\runtimes 则或者 .net\git\SuperSocketFileServer\SuperSocketFileServer\bin\Release\net7.0\runtimes
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetRelativePath(string path)
    {
        var directoryName = Path.GetDirectoryName(path);

        ArgumentNullException.ThrowIfNull(directoryName);

        var root = Path.GetPathRoot(directoryName);

        ArgumentNullException.ThrowIfNull(root);

        return directoryName.Replace(root, string.Empty);
    }
}
