using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class HardwareInformation
{
    /// <summary>
    /// 获取本机IP
    /// </summary>
    /// <returns></returns>
    public static string GetLocalIP()
    {
        try
        {
            //获取本机名
            string hostName = Dns.GetHostName();
            //获取本机IP
            IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
            var ipAddress = ipEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            return ipAddress?.ToString() ?? "";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    /// <summary>
    /// 异步获取IP地址
    /// </summary>
    /// <param name="host"></param>
    /// <param name="callback"></param>
    public static async Task<IPAddress> GetIpAddress(string host, Action<IPAddress> callback = null)
    {
        if (IPAddress.TryParse(host, out IPAddress ipAddress))
        {
            return ipAddress;
        }
        else
        {
            var addresses = await Dns.GetHostAddressesAsync(host);
            return addresses.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}
