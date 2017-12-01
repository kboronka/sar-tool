/* Copyright (C) 2017 Kevin Boronka
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.IO;

using sar.Base;
using sar.Tools;
using sar.Encryption;

namespace sar.Commands
{
    public class FileEncrypt : Command
    {
        public FileEncrypt(Base.CommandHub parent)
            : base(parent, "File - Encrypt",
                 new List<string> { "file.encrypt", "f.encrypt" },
                 "-f.encrypt [filepattern] [pretag] [key] [salt] [deletedUnencrytped]",
                 new List<string> { @"-f.encrypt \Jobs\Test\*.* encrypt ColeIsAwesome@123 salty true" })
        {
        }

        public override int Execute(string[] args)
        {
            // sanity check
            if (args.Length != 6)
            {
                throw new ArgumentException("incorrect number of arguments");
            }

            Progress.Message = "Searching";
            var filePattern = args[1];
            var pretag = args[2];
            var key = args[3];
            var salt = args[4];
            bool deleteUnencrypted = true;
            bool.TryParse(args[5], out deleteUnencrypted);

            var root = Directory.GetCurrentDirectory();
            IO.CheckRootAndPattern(ref root, ref filePattern);
            var files = IO.GetAllFiles(root, filePattern);

            if (!this.commandHub.NoWarning)
            {
                if (this.commandHub.Debug)
                {
                    foreach (string file in files)
                    {
                        ConsoleHelper.Write("found: ", ConsoleColor.Cyan);
                        ConsoleHelper.WriteLine(StringHelper.TrimStart(file, root.Length));
                    }
                }

                ConsoleHelper.WriteLine(files.Count.ToString() + " file" + ((files.Count != 1) ? "s" : "") + " found");
            }

            int counter = 0;
            if (files.Count > 0)
            {
                foreach (string file in files)
                {
                    Progress.Message = "Encrpyting " + StringHelper.TrimStart(file, root.Length);

                    try
                    {
                        var fileName = Path.GetFileName(file);
                        var fileRoot = Path.GetDirectoryName(file);
                        var encryptedName = pretag + "." + fileName;
                        var encryptedPath = Path.Combine(fileRoot, encryptedName);
                        EncryptionHelper.EncryptFile(file, encryptedPath, key, salt);
                        counter++;
                        ConsoleHelper.WriteLine("File: " + encryptedName + " encrypted", ConsoleColor.DarkYellow);

                        if (deleteUnencrypted)
                        {
                            File.Delete(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        ConsoleHelper.Write("failed: ", ConsoleColor.Red);
                        ConsoleHelper.WriteLine(StringHelper.TrimStart(file, root.Length));

                        if (this.commandHub.Debug)
                        {
                            ConsoleHelper.WriteException(ex);
                        }
                    }

                }
            }

            ConsoleHelper.WriteLine(counter.ToString() + " File" + ((counter != 1) ? "s" : "") + " Encryped", ConsoleColor.DarkYellow);
            return ConsoleHelper.EXIT_OK;
        }
    }
}

