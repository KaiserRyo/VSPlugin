﻿using System;
using System.Collections.Generic;
using System.Text;
using BlackBerry.NativeCore.Diagnostics;
using BlackBerry.NativeCore.QConn.Model;

namespace BlackBerry.NativeCore.QConn.Services
{
    public sealed class TargetServiceFile : TargetService
    {
        public const int ModeOpenNone = 0;
        public const int ModeOpenReadOnly= 1;
        public const int ModeOpenWriteOnly = 2;
        public const int ModeOpenReadWrite = 3;

        public TargetServiceFile(Version version, QConnConnection connection)
            : base(version, connection)
        {
        }

        public override string ToString()
        {
            return "FileService";
        }

        /// <summary>
        /// Sends a command to the target and returns its parsed representation.
        /// </summary>
        private Token[] Send(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentNullException("command");

            // send:
            var rawResponse = Connection.Send(command);
            if (string.IsNullOrEmpty(rawResponse))
                throw new QConnException("Invalid response received for command: \"" + command + "\"");

            // parse:
            var response = Token.Parse(rawResponse);
            if (response == null || response.Length == 0)
                throw new QConnException("Unable to parse response: \"" + rawResponse + "\" for command: \"" + command + "\"");

            return response;
        }

        /// <summary>
        /// Opens specified path with specified mode.
        /// </summary>
        private TargetFileDescriptor Open(string path, uint mode, uint permissions)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            Token[] response;

            if (mode == ModeOpenNone)
            {
                response = Send(string.Concat("oc:\"", path, "\":0"));
            }
            else
            {
                uint qmode = (mode & 0xFFFFFFFC) | ((mode - 1) & 3);

                if (permissions != uint.MaxValue)
                {
                    response = Send(string.Concat("o:\"", path, "\":", qmode.ToString("X"), ":", permissions.ToString("X")));
                }
                else
                {
                    response = Send(string.Concat("o:\"", path, "\":", qmode.ToString("X")));
                }
            }

            if (response[0].StringValue == "e")
                throw new QConnException("Opening-handle failed: " + response[1].StringValue);
            if (response.Length < 5)
                throw new QConnException("Opening-handle response has invalid format");

            return new TargetFileDescriptor(this, response[1].StringValue, response[2].UInt32Value, response[3].UInt64Value, 0, response[4].StringValue, path);
        }

        internal void Close(TargetFileDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            if (!descriptor.IsClosed)
            {
                var response = Send("c:" + descriptor.Handle);
                descriptor.Closed();

                if (response[0].StringValue == "e")
                    throw new QConnException("Closing-handle failed: " + response[1].StringValue);
            }
        }

        internal byte[] Read(TargetFileDescriptor descriptor, ulong offset, ulong length)
        {
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");
            if (descriptor.IsClosed)
                throw new ArgumentOutOfRangeException("descriptor");
            if (length > int.MaxValue)
                throw new ArgumentOutOfRangeException("length", "Unable to load so much data at once");

            // ask for the raw data:
            var reader = Connection.Request(string.Concat("r:", descriptor.Handle, ":", offset.ToString("X"), ":", length.ToString("X")));

            // read and parse the header part:
            var responseHeader = reader.ReadString('\n');
            if (string.IsNullOrEmpty(responseHeader))
                throw new QConnException("Unable to retrieve response header");
            var response = Token.Parse(responseHeader);
            if (response[0].StringValue != "o")
                throw new QConnException("Reading failed: " + response[1].StringValue);

            ulong contentLength = response[1].UInt64Value;
            // ulong contentOffset = response[2].UInt64Value;
            var buffer = reader.ReadBytes((int)contentLength);
            if (buffer == null || buffer.Length != (int)contentLength)
                throw new QConnException("Invalid number of content bytes read");

            return buffer;
        }

        /// <summary>
        /// Gets the info about specified path.
        /// It throws exceptions in case of any errors or permission denies.
        /// </summary>
        private TargetFileDescriptor Stat(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            using(var descriptor = Open(path, ModeOpenNone, uint.MaxValue))
            {
                // ask for full description:
                var response = Send("s:" + descriptor.Handle);
                if (response[0].StringValue == "e")
                    throw new QConnException("Stat failed: " + response[1].StringValue);

                // update creation-date, type and size:
                var creationTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(response[9].UInt64Value).ToLocalTime();
                descriptor.Update(response[5].UInt32Value, response[6].UInt32Value, creationTime, response[10].UInt32Value, response[2].UInt64Value);
                return descriptor;
            }
        }

        /// <summary>
        /// Lists files and folders at specified location.
        /// </summary>
        public TargetFile[] List(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            var descriptor = Stat(path);
            if (descriptor == null)
                throw new QConnException("Unable to determine path properties");
            return List(descriptor);
        }

        /// <summary>
        /// Lists files and folders at specified location.
        /// </summary>
        public TargetFile[] List(TargetFile location)
        {
            if (location.IsDirectory)
            {
                using (var directory = Open(location.Path, ModeOpenReadOnly, 0x4000))
                {
                    var data = Read(directory, 0, directory.Size);
                    if (data == null)
                        throw new QConnException("Invalid directory listing read");
                    string listing = Encoding.UTF8.GetString(data);

                    // parse names, as each is in separate line:
                    List<TargetFile> result = new List<TargetFile>();
                    foreach (var item in listing.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (item != "." && item != "..")
                        {
                            string itemPath = location.CreateItemPath(item);
                            try
                            {
                                // and try to load detailed info:
                                result.Add(Stat(itemPath));
                            }
                            catch (Exception ex)
                            {
                                // add a stub, just to keep the path only (as might lack permissions to read info even):
                                result.Add(new TargetFile(itemPath));
                                QTraceLog.WriteException(ex, "Unable to load info about path: \"" + itemPath + "\"");
                            }
                        }
                    }

                    return result.ToArray();
                }
            }

            throw new QConnException("Not a folder, unable to perform listing");
        }

        /// <summary>
        /// Create a folder at specified location.
        /// </summary>
        public TargetFile CreateFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            using (var descriptor = Open(path, 0x100, 0xFFF | 0x4000))
            {
                return descriptor;
            }
        }
    }
}
