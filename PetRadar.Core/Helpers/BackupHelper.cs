using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PetRadar.Core.Helpers
{
    public static class BackupHelper
    {
        public static async Task<int> BackupDataBase(string connectionString, string backupFilePath, CancellationToken token)
        {
            var builder = new System.Data.Common.DbConnectionStringBuilder();
            builder.ConnectionString = connectionString;

            string host = builder.ContainsKey("Host") ? builder["Host"].ToString() : string.Empty;  
            string port = builder.ContainsKey("Port") ? builder["Port"].ToString() : string.Empty;
            string db = builder.ContainsKey("Database") ? builder["Database"].ToString() : string.Empty;
            string user = builder.ContainsKey("Username") ? builder["Username"].ToString() : string.Empty;
            string pass = builder.ContainsKey("Password") ? builder["Password"].ToString() : string.Empty;

            using var process = new Process();

            var startInfo = new ProcessStartInfo
            {
                FileName = "pg_dump",
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            if (!string.IsNullOrEmpty(host)) { startInfo.ArgumentList.Add("-h"); startInfo.ArgumentList.Add(host); }
            if (!string.IsNullOrEmpty(port)) { startInfo.ArgumentList.Add("-p"); startInfo.ArgumentList.Add(port); }
            if (!string.IsNullOrEmpty(user)) { startInfo.ArgumentList.Add("-U"); startInfo.ArgumentList.Add(user); }

            startInfo.ArgumentList.Add("-F");
            startInfo.ArgumentList.Add("c");
            startInfo.ArgumentList.Add("-v");
            startInfo.ArgumentList.Add("-f");
            startInfo.ArgumentList.Add(backupFilePath); // No manual quotes needed, .NET handles escaping natively

            if (!string.IsNullOrEmpty(db)) { startInfo.ArgumentList.Add("-d"); startInfo.ArgumentList.Add(db); }

            if (!string.IsNullOrEmpty(pass))
            {
                startInfo.Environment["PGPASSWORD"] = pass;
            }

            process.StartInfo = startInfo;
            process.Start();

            await process.WaitForExitAsync(token);

            return process.ExitCode;
        }
    }
}
