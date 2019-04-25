﻿using System;
using System.Json;

namespace Berrysoft.Tsinghua.Net
{
    /// <summary>
    /// The response of Login or Logout.
    /// </summary>
    public class LogResponse
    {
        /// <summary>
        /// Initialize a new instance of <see cref="LogResponse"/> class.
        /// </summary>
        /// <param name="succeed">Whether the command is succeed.</param>
        /// <param name="message">The formatted response message.</param>
        public LogResponse(bool succeed, string message)
        {
            Succeed = succeed;
            Message = message;
        }

        /// <summary>
        /// Shows whether the command is succeed.
        /// </summary>
        public bool Succeed { get; }
        /// <summary>
        /// The formatted response message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Determines whether the two <see cref="LogResponse"/> are equal.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns><see langword="true"/> if they're equal; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is LogResponse other)
            {
                return Succeed == other.Succeed && Message == other.Message;
            }
            return false;
        }
        /// <summary>
        /// Returns the hash value of this object.
        /// </summary>
        /// <returns>The hash value.</returns>
        public override int GetHashCode()
        {
            return Succeed.GetHashCode() ^ (Message?.GetHashCode() ?? 0);
        }

        internal static LogResponse ParseFromNet(string response)
        {
            return new LogResponse(response == "Login is successful.", response);
        }

        internal static LogResponse ParseFromAuth(string response)
        {
            try
            {
                string jsonstr = response.Substring(9, response.Length - 10);
                JsonValue json = JsonValue.Parse(jsonstr);
                return new LogResponse(json["error"] == "ok", $"error: {json["error"]}\nerror_msg: {json["error_msg"]}");
            }
            catch (Exception)
            {
                return new LogResponse(false, response);
            }
        }

        internal static LogResponse ParseFromUsereg(string response)
        {
            return new LogResponse(response == "ok", response);
        }
    }
}